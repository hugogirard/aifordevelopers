using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Contoso;
using DocumentIntelligent;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices(services => {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        services.AddSingleton<IDocumentIntelligentService, DocumentIntelligentService>();
        services.AddSingleton<IStorageSevice,StorageSevice>();
    })
    .Build();

host.Run();
