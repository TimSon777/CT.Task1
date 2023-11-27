namespace FaceDetector.Objects;

public sealed class YandexSettings
{
    public const string SectionName = "Yandex";

    public Uri VisionApiUri { get; set; } = default!;

    public Uri StorageApiUri { get; set; } = default!;

    public string FolderId { get; set; } = default!;

    public string AccessKey { get; set; } = default!;

    public string SecretKey { get; set; } = default!;

    public string ApiKey { get; set; } = default!;
}
