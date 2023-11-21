using System.Text.Json;
using FaceDetector.Objects;
using FaceDetector.Services;
using Microsoft.Extensions.DependencyInjection;
using Yandex.Cloud.Functions;

namespace FaceDetector;

public sealed class Handler : YcFunction<string, Task>
{
    private readonly IServiceProvider _serviceProvider = new Startup().CreateServiceProvider();

    public async Task FunctionHandler(string request, Context context)
    {
        var input = JsonSerializer.Deserialize<Input>(request)!;
        var photoProcessor = _serviceProvider.GetRequiredService<IPhotoProcessor>();
        var detail = input.Messages.First().Detail;
        await photoProcessor.ProcessAsync(detail);
    }
}
