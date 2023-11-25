using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Amazon.S3;
using Amazon.S3.Model;
using FaceDetector.Objects;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FaceDetector.Services;

public interface IPhotoProcessor
{
    Task ProcessAsync(Detail detail);
}

public sealed class PhotoProcessor(
    IIamTokenService iamTokenService,
    ICoordinateConverter coordinateConverter,
    IYandexVisionResponseTreeTraversal yandexVisionResponseTreeTraversal,
    IAmazonS3 amazonS3,
    IMessageQueueSender messageQueueSender,
    IOptions<YandexSettings> yandexSettingsOptions,
    ILogger<PhotoProcessor> logger) : IPhotoProcessor
{
    public async Task ProcessAsync(Detail detail)
    {
        var yandexSettings = yandexSettingsOptions.Value;

        var iamToken = await iamTokenService.GetTokenAsync();

        var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", iamToken);
        client.DefaultRequestHeaders.Add("x-folder-id", yandexSettings.FolderId);

        var image = await amazonS3.GetObjectAsync(new GetObjectRequest
        {
            BucketName = detail.BucketId,
            Key = detail.ObjectId
        });

        logger.LogInformation("The object was successfully retrieved from bucket");

        await using var responseStream = image.ResponseStream;
        using var memoryStream = new MemoryStream();
        await responseStream.CopyToAsync(memoryStream);
        var imageBytes = memoryStream.ToArray();
        var base64String = Convert.ToBase64String(imageBytes);

        var request = new YandexVisionRequest
        {
            FolderId = yandexSettings.FolderId,
            AnalyzeSpecs = new List<AnalyzeSpec>
            {
                new()
                {
                    Content = base64String,
                    Features = new List<Feature> { new() { Type = "FACE_DETECTION" } }
                }
            }
        };

        var response = await client.PostAsJsonAsync(yandexSettings.VisionApiUri, request);

        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            logger.LogCritical("The request to Yandex Vision was not successful\nCode: {Code}\n{Content}",
                response.StatusCode,
                content);

            return;
        }

        var result = await response.Content.ReadFromJsonAsync<YandexVisionResponse>();

        var faces = yandexVisionResponseTreeTraversal
            .Traverse(result!.Results)
            .ToArray();

        logger.LogInformation("Yandex Vision returned the result, count faces: {Faces Count}", faces.Length);

        var faceCutTaskQueueMessages = faces
            .Where(f => f.BoundingBox.Vertices
                .All(v => v.X is not null && v.Y is not null))
            .Select(f => new TaskQueueMessage
            {
                SourceImageId = detail.ObjectId,
                SourceBucketId = detail.BucketId,
                Rectangle = f.BoundingBox.Vertices
                    .Select(coordinateConverter.MapToIntCoordinate)
            });

        await messageQueueSender.SendBatchAsync(faceCutTaskQueueMessages);

        logger.LogInformation("Data sent to queue successfully");
        logger.LogInformation("The function completed its work successfully");
    }
}
