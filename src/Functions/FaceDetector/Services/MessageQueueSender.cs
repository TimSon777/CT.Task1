using System.Net;
using System.Text.Json;
using Amazon.SQS;
using Amazon.SQS.Model;
using FaceDetector.Objects;
using Microsoft.Extensions.Options;

namespace FaceDetector.Services;

public interface IMessageQueueSender
{
    Task SendBatchAsync<TMessage>(IEnumerable<TMessage> messages)
        where TMessage : class;
}

public sealed class MessageQueueSender(
    IAmazonSQS amazonSQS,
    IOptions<YandexSettings> yandexSettingsOptions) : IMessageQueueSender
{
    public async Task SendBatchAsync<TMessage>(IEnumerable<TMessage> messages)
        where TMessage : class
    {
        var tasks = messages
            .Chunk(10)
            .Select(chunk => new SendMessageBatchRequest
            {
                QueueUrl = yandexSettingsOptions.Value.TaskQueueUri.ToString(),
                Entries = chunk
                    .Select((command, index) => new SendMessageBatchRequestEntry
                    {
                        Id = index.ToString(),
                        MessageBody = SerializeBody(command),

                    })
                    .ToList()
            })
            .Select(async request =>
            {
                var response = await amazonSQS.SendMessageBatchAsync(request);

                if (response.HttpStatusCode >= HttpStatusCode.OK && (int)response.HttpStatusCode <= 299)
                {
                    return;
                }

                throw new HttpRequestException(null, null, response.HttpStatusCode);
            });

        var task = Task.WhenAll(tasks);

        try
        {
            await task;
        }
        catch (Exception)
        {
            throw task.Exception!;
        }
    }

    private static string SerializeBody<TCommand>(TCommand command)
    {
        return JsonSerializer.Serialize(command);
    }
}
