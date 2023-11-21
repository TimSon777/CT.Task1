using System.Text.Json.Serialization;

namespace FaceDetector.Objects;

public sealed class Input
{
    [JsonPropertyName("messages")]
    public Message[] Messages { get; set; } = default!;
}

public sealed class Message
{
    [JsonPropertyName("details")]
    public Detail Detail { get; set; } = default!;
}

public sealed class Detail
{
    [JsonPropertyName("bucket_id")]
    public string BucketId { get; set; } = default!;

    [JsonPropertyName("object_id")]
    public string ObjectId { get; set; } = default!;
}
