using Yandex.Cloud.Functions;

namespace FaceDetector;

public sealed class Handler : YcFunction<int, int>
{
    public int FunctionHandler(int request, Context context)
    {
        return request + 1;
    }
}
