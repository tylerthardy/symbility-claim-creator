using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace SymbilityClaimCreator
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using IHost host = CreateHostBuilder(args).Build();
            await host.RunAsync();

            Console.WriteLine("Creator finished! Press any key to continue...");
            Console.ReadKey();
        }

        static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureHostConfiguration(config =>
                {
                    if (args != null)
                    {
                        // arguments passed from command line
                        // e.g.: dotnet run --environment "Staging"
                        config.AddCommandLine(args);
                    }
                })
                .ConfigureAppConfiguration((hostingContext, configuration) =>
                {
                    configuration.Sources.Clear();

                    var env = hostingContext.HostingEnvironment;

                    configuration
                        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                        .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true, true);

                    var configurationRoot = configuration.Build();
                })
                .ConfigureServices((hosting, services) =>
                {
                    services.Configure<ClaimCreatorConfiguration>(hosting.Configuration);
                    services.AddHostedService<ClaimCreator>();
                });
    }
}
