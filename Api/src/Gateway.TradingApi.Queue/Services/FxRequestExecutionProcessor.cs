using System;
using System.Threading;
using MassTransit;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Domain.BestExecution;
using Trading.Foundation.Dtos;
using Trading.Foundation.Protocol;

namespace Gateway.TradingApi.Queue.Services
{
    /// <summary>
    /// It is here that we execute the request and then send response back
    /// </summary>
    public interface IFxRequestExecutionProcessor
    {
        Task Execute(IRequest<FxBestExecutionRequest> fxOrderRequest);
    }

    public class FxRequestExecutionProcessor : IFxRequestExecutionProcessor
    {
        private readonly ILogger<FxRequestExecutionProcessor> logger;
        private readonly IBus bus;
        private readonly IFxBestExecutionService bestExecutionService;

        public FxRequestExecutionProcessor(ILogger<FxRequestExecutionProcessor> logger, 
                                            IBus bus, 
                                            IFxBestExecutionService bestExecutionService)
        {
            this.logger = logger;
            this.bus = bus;
            this.bestExecutionService = bestExecutionService;
        }

        public async Task Execute(IRequest<FxBestExecutionRequest> fxOrderRequest)
        {
            try
            {
                var response = await bestExecutionService.Create(fxOrderRequest);
                await bus.Publish(response, CancellationToken.None);
            }
            catch (Exception exc)
            {
                logger.LogError(exc, $"Call to {nameof(FxBestExecutionService)} has failed when executing the request: {fxOrderRequest}");
            }
        }
    }
}
