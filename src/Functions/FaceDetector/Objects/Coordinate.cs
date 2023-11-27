using System.Text.Json.Serialization;

namespace FaceDetector.Objects;

public sealed class Coordinate<T>
{
    [JsonPropertyName("x")]
    public T? X { get; set; }

    [JsonPropertyName("y")]
    public T? Y { get; set; }
}
