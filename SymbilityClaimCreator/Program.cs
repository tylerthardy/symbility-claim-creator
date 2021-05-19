using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace SymbilityClaimCreator
{
    class Program
    {
        static void Main(string[] args)
        {
            using var host = CreateHostBuilder(args).Build();

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

                    var claimSourceConfiguration = new SymbilityApiConfiguration();
                    var assigneeConfiguration = new SymbilityApiConfiguration();
                    var secondAssigneeConfiguration = new SymbilityApiConfiguration();
                    configurationRoot.GetSection("ClaimSourceConfiguration").Bind(claimSourceConfiguration);
                    configurationRoot.GetSection("AssigneeConfiguration").Bind(assigneeConfiguration);
                    configurationRoot.GetSection("SecondAssigneeConfiguration").Bind(secondAssigneeConfiguration);
                });
    }
}
