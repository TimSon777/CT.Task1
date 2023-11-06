using Yandex.Cloud.Functions;

namespace FaceDetector;

public sealed class Handler : YcFunction<byte[], int>
{
    public int FunctionHandler(byte[] request, Context context)
    {
        return 1;
    }
}
