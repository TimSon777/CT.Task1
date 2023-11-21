using System.Text.Json.Serialization;

namespace FaceDetector.Objects;

public sealed class YandexVisionRequest
{
    public string FolderId { get; set; } = default!;

    [JsonPropertyName("analyze_specs")]
    public List<AnalyzeSpec> AnalyzeSpecs { get; set; } = default!;
}

public sealed class AnalyzeSpec
{
    public string Content { get; set; } = default!;

    public List<Feature> Features { get; set; } = default!;
}

public sealed class Feature
{
    public string Type { get; set; } = default!;
}
