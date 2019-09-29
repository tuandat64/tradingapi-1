using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Gateway.TradingApi.Queue.Services.Background
{
    /// <summary>
    /// Here we are managing the creation, configuration and ownership of the rabbitMq infrastructure
    /// </summary>
    public class RabbitMqService : BackgroundServiceBase<IBusControl>
    {
        private readonly IBusControl busControl;

        public RabbitMqService(ILogger<RabbitMqService> logger, 
                                IBusControl busControl) 
            : base(logger)
        {
            this.busControl = busControl;
        }

        protected override string ServiceName { get; } = nameof(RabbitMqService);
        
        protected override async Task<IBusControl> OnStartup(CancellationToken shutdownToken)
        {
            await Task.Yield();

            busControl.Start();

            return busControl;
        }

        protected override async Task OnShutdown(IBusControl bus)
        {
            await Task.Yield();
            bus.Stop();
        }
    }
}
