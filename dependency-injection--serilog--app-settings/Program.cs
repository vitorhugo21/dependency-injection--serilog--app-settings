using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System.IO;

namespace dependency_injection__serilog__app_settings
{
    class Program
    {
        static void Main(string[] args)
        {
            var host = AppStartup();

            var dataService = ActivatorUtilities.CreateInstance<DataService>(host.Services);

            dataService.Connect();
        }

        static void ConfigSetup(IConfigurationBuilder builder)
        {
            builder
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();
        }

        static IHost AppStartup()
        {
            var builder = new ConfigurationBuilder();
            ConfigSetup(builder);

            // Defining Serilog configs
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Build())
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();

            // Initiated the dependency injection container
            var host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    services.AddTransient<IDataService, DataService>();
                })
                .UseSerilog()
                .Build();

            return host;
        }
    }
}
