namespace FaceDetector.Objects;

public sealed class YandexVisionResponse
{
    public List<Result> Results { get; set; } = new();
}

public sealed class BoundingBox
{
    public List<Coordinate<string>> Vertices { get; set; } = default!;
}

public sealed class Face
{
    public BoundingBox BoundingBox { get; set; } = default!;
}

public sealed class FaceDetection
{
    public List<Face>? Faces { get; set; } = default!;
}

public sealed class Result
{
    public List<Result>? Results { get; set; } = default!;

    public FaceDetection? FaceDetection { get; set; } = default!;
}
