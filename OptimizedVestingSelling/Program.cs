using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ApplicationTemplate
{
    public class Program
    {
        /// <summary>
        ///     Configuration for the application
        /// </summary>
        public static IConfiguration Configuration { get; private set; }

        /// <summary>
        ///     Entry point of the project.
        ///     Notice the method is returning async task instead of void in order to support async programming.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static async Task Main(string[] args)
        {
            try
            {
                Configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true, reloadOnChange: true)
                    .AddCommandLine(args)
                    .AddEnvironmentVariables()
                    .Build();

                // configure serilog
                Log.Logger = new LoggerConfiguration()
                    .ReadFrom.Configuration(Configuration)
                    .Enrich.FromLogContext()
                    .Enrich.WithMachineName()
                    .CreateLogger();

                Log.Information("Starting up...");

                // Create service collection and configure our services
                var services = ConfigureServices();
                // Generate a provider
                var serviceProvider = services.BuildServiceProvider();

                // Kick off the actual application
                await serviceProvider.GetService<App>().Run();

                Log.Information("Shutting down...");
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        /// <summary>
        ///     Configure the services using ASP.NET CORE built-in Dependency Injection.
        /// </summary>
        /// <returns></returns>
        private static IServiceCollection ConfigureServices()
        {
            IServiceCollection services = new ServiceCollection();

            // Configuration should be singleton as the entire application should use one
            services.AddSingleton(Configuration);
            services.AddOptions();

            // Register the services
            // services.AddTransient<IExampleService, ExampleService>();

            // Configure EmailSettings so IOption<EmailSettings> can be injected 
            // services.Configure<ExampleSettings>(Configuration.GetSection("ExampleSettings"));

            // Register the actual application entry point
            services.AddSingleton<App>();

            return services;
        }
    }
}
