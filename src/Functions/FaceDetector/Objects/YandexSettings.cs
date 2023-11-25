namespace FaceDetector.Objects;

public sealed class YandexSettings
{
    public const string SectionName = "Yandex";

    public Uri VisionApiUri { get; set; } = default!;

    public Uri StorageApiUri { get; set; } = default!;

    public Uri QueueApiUri { get; set; } = default!;

    public Uri TaskQueueUri { get; set; } = default!;

    public string FolderId { get; set; } = default!;

    public string Region { get; set; } = default!;
}
