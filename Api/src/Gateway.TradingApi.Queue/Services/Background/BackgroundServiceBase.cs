using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Gateway.TradingApi.Queue.Services.Background
{
    /// <summary>
    /// Base class for any long running services. We handle:
    /// 1. Unhandled exceptions (logging them and rethrowing them)
    /// 2. Logging of start up / shutdown
    /// </summary>
    public abstract class BackgroundServiceBase<TState> : BackgroundService
    {
        protected readonly ILogger Logger;

        protected abstract string ServiceName { get; }
        protected abstract Task<TState> OnStartup(CancellationToken shutdownToken);
        protected virtual Task OnShutdown(TState state) { return Task.CompletedTask; }

        protected BackgroundServiceBase(ILogger logger)
        {
            Logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken shutdownToken)
        {
            try
            {
                Logger.LogInformation($"{ServiceName} is starting up");

                TState state;

                try
                {
                    state = await OnStartup(shutdownToken);
                }
                catch (Exception exc)
                {
                    Logger.LogError(exc, $"{ServiceName} has failed during startup");
                    throw;
                }

                Logger.LogInformation($"{ServiceName} is up and running");

                await Task.WhenAny(Task.Delay(Timeout.Infinite, shutdownToken));

                Logger.LogInformation($"{ServiceName} is shutting down");

                try
                {
                    await OnShutdown(state);
                }
                catch (Exception exc)
                {
                    Logger.LogWarning(exc, $"{ServiceName} has failed during shutdown. Will shutdown normally");
                }

                Logger.LogInformation($"{ServiceName} has shutdown");
            }
            catch(Exception exc)
            {
                Logger.LogError(exc, $"Fatal exception in service {ServiceName}. Shutting down");
                throw;
            }
        }
    }
}
