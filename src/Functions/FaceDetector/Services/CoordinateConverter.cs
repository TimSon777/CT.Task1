using FaceDetector.Objects;

namespace FaceDetector.Services;

public interface ICoordinateConverter
{
    Coordinate<int> MapToIntCoordinate(Coordinate<string> src);
}

public sealed class CoordinateConverter : ICoordinateConverter
{
    public Coordinate<int> MapToIntCoordinate(Coordinate<string> src) => new()
    {
        X = int.Parse(src.X!),
        Y = int.Parse(src.Y!)
    };
}
