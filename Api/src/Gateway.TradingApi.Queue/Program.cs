using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;

namespace Gateway.TradingApi.Queue
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .MinimumLevel.Information()
                .WriteTo.ColoredConsole(
                    LogEventLevel.Information,
                    "{NewLine}{Timestamp:HH:mm:ss} [{Level}] ({CorrelationToken}) {Message}{NewLine}{Exception}")
                .WriteTo.File(@"c:\logs\Gateway.TradingApi.Queue\log.txt", rollingInterval: RollingInterval.Day)
                .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", LogEventLevel.Warning)
                .CreateLogger();

            try
            {
                CreateWebHostBuilder(args).Build().Run();
            }
            catch (Exception exc)
            {
                Log.Logger.Fatal(exc, "Fatal error shutting down");
            }
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            var webHost = WebHost.CreateDefaultBuilder(args);

            webHost.ConfigureAppConfiguration((context, config) =>
            {
                var builtConfig = config.Build();
                config.AddJsonFile("appsettings.sensitive.json");

            });

            return webHost.UseSerilog()
                .UseStartup<Startup>();
        }
    }
}
