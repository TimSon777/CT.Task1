using System.Net.Http.Headers;
using System.Net.Http.Json;
using Amazon.S3;
using Amazon.S3.Model;
using FaceDetector.Objects;
using Microsoft.Extensions.Options;

namespace FaceDetector.Services;

public interface IPhotoProcessor
{
    Task ProcessAsync(Detail detail);
}

public sealed class PhotoProcessor(
    IIamTokenService iamTokenService,
    IAmazonS3 amazonS3,
    IOptions<YandexSettings> yandexSettingsOptions) : IPhotoProcessor
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
        await response.Content.ReadFromJsonAsync<YandexVisionResponse>();
    }
}
