using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Serilog;
using Serilog.Events;
using System;
using System.Diagnostics;
using System.Reflection;

namespace Gateway.TradingApi.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var assyName = $"{Assembly.GetExecutingAssembly().GetName().Name}";
            // var filePath = $"C:\\logs\\{assyName}\\{DateTime.Now:yyyy-MM-dd_HH:mm:ss}.txt";

            bool isIIS = String.Compare(Process.GetCurrentProcess().ProcessName, "w3wp") == 0;
            string logFoldername;
            if (isIIS)
            {
                logFoldername = $"IIS-{assyName}";
            }
            else
            {
                logFoldername = assyName;
            }

            //var logFilepath = $"C:\\logs\\{logFoldername}\\{DateTime.Now:yyyy-MM-dd_HH:mm:ss}.txt";
            var logFilepath = $"C:\\logs\\{logFoldername}\\log.txt";

            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .MinimumLevel.Information()
                .WriteTo.ColoredConsole(
                    LogEventLevel.Information,
                    "{NewLine}{Timestamp:HH:mm:ss} [{Level}] ({CorrelationToken}) {Message}{NewLine}{Exception}")
                .WriteTo.File(logFilepath, rollingInterval: RollingInterval.Day)
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
            return WebHost.CreateDefaultBuilder(args)
                            .UseSerilog()
                            .UseStartup<Startup>();
        }
    }
}
