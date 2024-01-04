using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using modelUtility.Services;

namespace modelUtility;

public enum Operation
{ 
    CreateModelDocument = 1,
    CreateSearchIndexingResource = 2
}

public class Utility
{
    public static IBootstrapper CreateBoostrapInstance() 
    {

        // Create configuration file
        var configuration = new ConfigurationBuilder()
                            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)     
                            .AddEnvironmentVariables()
                            .AddUserSecrets<Program>()
                            .Build();

        var serviceProvider = new ServiceCollection()
                                    .AddLogging(c => c.AddConsole())
                                    .AddSingleton(configuration)
                                    .AddTransient<IFormRecognizerService, FormRecognizerService>()
                                    .AddTransient<IStorageService, StorageService>()
                                    .AddSingleton<IBootstrapper, Bootstrapper>()
                                    .BuildServiceProvider();


        return serviceProvider.GetService<IBootstrapper>() 
                                ?? throw new NullReferenceException("IBootstrapper is not implemented");
        
    }

    public static Operation? ConvertStringToEnum(string value) => Enum.TryParse(typeof(Operation), value, true, out var result)
                                                                  ? (Operation)result
                                                                  : null;

}
