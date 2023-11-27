using System.Text.Json.Serialization;

namespace FaceDetector.Objects;

public sealed class TaskQueueMessage
{
    [JsonPropertyName("source_image_id")]
    public string SourceImageId { get; set; } = default!;

    [JsonPropertyName("source_bucket_id")]
    public string SourceBucketId { get; set; } = default!;

    [JsonPropertyName("rectangle")]
    public IEnumerable<Coordinate<int>> Rectangle { get; set; } = default!;
}
