using FaceDetector.Objects;

namespace FaceDetector.Services;

public interface IYandexVisionResponseTreeTraversal
{
    IEnumerable<Face> Traverse(List<Result> results);
}

public sealed class YandexVisionResponseTreeTraversal : IYandexVisionResponseTreeTraversal
{
    public IEnumerable<Face> Traverse(List<Result> results)
    {
        foreach (var result in results)
        {
            if (result.FaceDetection?.Faces != null)
            {
                foreach (var face in result.FaceDetection.Faces)
                {
                    yield return face;
                }
            }

            if (result.Results is null)
            {
                continue;
            }

            foreach (var face in Traverse(result.Results))
            {
                yield return face;
            }
        }
    }
}
