using Amazon;
using Amazon.S3;
using Amazon.SQS;
using FaceDetector.Objects;
using FaceDetector.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FaceDetector;

public sealed class Startup
{
    public IServiceProvider CreateServiceProvider(bool validateOnBuild = false)
    {
        var services = new ServiceCollection();
        Configuration = CreateConfiguration();

        services.AddSingleton(Configuration);
        services.AddSingleton<IPhotoProcessor, PhotoProcessor>();
        services.AddSingleton<ICoordinateConverter, CoordinateConverter>();
        services.AddSingleton<IYandexVisionResponseTreeTraversal, YandexVisionResponseTreeTraversal>();
        services.AddSingleton<IMessageQueueSender, MessageQueueSender>();

        services.Configure<YandexSettings>(Configuration.GetSection(YandexSettings.SectionName));

        services.AddSingleton<IAmazonS3, AmazonS3Client>(sp =>
        {
            var yandexSettings = sp.GetRequiredService<IOptions<YandexSettings>>().Value;
            return new AmazonS3Client(
                new AmazonS3Config
                {
                    ServiceURL = yandexSettings.StorageApiUri.ToString(),
                    ForcePathStyle = true
                });
        });

        services.AddSingleton<IAmazonSQS, AmazonSQSClient>(sp =>
        {
            var yandexSettings = sp.GetRequiredService<IOptions<YandexSettings>>().Value;
            return new AmazonSQSClient(
                new AmazonSQSConfig
                {
                    ServiceURL = yandexSettings.QueueApiUri.ToString(),
                    AuthenticationRegion = yandexSettings.Region
                });
        });

        services.AddLogging(logging =>
        {
            logging.AddConfiguration(Configuration);

            logging.AddLambdaLogger(new LambdaLoggerOptions
            {
                IncludeException = true
            });
        });

        return services.BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = validateOnBuild });
    }

    private IConfiguration Configuration { get; set; } = default!;

    private static IConfiguration CreateConfiguration()
    {
        var currentDirectory = Directory.GetCurrentDirectory();

        var config = new ConfigurationBuilder()
            .SetBasePath(currentDirectory)
            .AddJsonFile("appsettings.json", true)
            .AddEnvironmentVariables()
            .Build();

        return config;
    }
}
