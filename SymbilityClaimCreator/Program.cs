using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using MockDataUtils;

namespace SymbilityClaimCreator
{
    class Program
    {
        static void Main(string[] args)
        {
            using var host = CreateHostBuilder(args).Build();

            var addressGenerator = new MockAddressGenerator();
            var address = addressGenerator.GetRandomAddress();
            Console.WriteLine(address);
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

                    var configurationSettings = new SymbilityClaimCreatorConfiguration();
                    configurationRoot.Bind(configurationSettings);
                });
    }
}
