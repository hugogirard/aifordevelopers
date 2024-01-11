using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Contoso;
using DocumentIntelligent;
using Microsoft.Extensions.Logging;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices(services => {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        services.AddLogging(loggingBuilder => 
        { 
            loggingBuilder.AddApplicationInsights();
            loggingBuilder.SetMinimumLevel(LogLevel.Debug);
        });
        services.AddSingleton<IDocumentIntelligentService, DocumentIntelligentService>();
        services.AddSingleton<IStorageSevice,StorageSevice>();
        services.AddHttpClient();
    })
    .Build();

host.Run();
