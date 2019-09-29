using System;
using System.Threading.Tasks;
using Gateway.TradingApi.Queue.Extensions;
using Gateway.TradingApi.Queue.Services;
using MassTransit;
using Microsoft.Extensions.Logging;
using Trading.Foundation.Dtos;
using Trading.Foundation.Protocol;

namespace Gateway.TradingApi.Queue.Consumers
{
    public class FxOrderRequestConsumer : IConsumer<IRequest<FxBestExecutionRequest>>
    {
        private readonly ILogger<FxOrderRequestConsumer> logger;
        private readonly IFxRequestExecutionProcessor fxRequestExecutionProcessor;

        public FxOrderRequestConsumer(ILogger<FxOrderRequestConsumer> logger,
                                        IFxRequestExecutionProcessor fxRequestExecutionProcessor)
        {
            this.logger = logger;
            this.fxRequestExecutionProcessor = fxRequestExecutionProcessor;
        }

        public async Task Consume(ConsumeContext<IRequest<FxBestExecutionRequest>> context)
        {
            var fxOrderRequest = context.Message.Payload;

            logger.LogInformation($"Queue message: received inbound {nameof(FxBestExecutionRequest)} message. {context.Message.ToGatewayString()} with payload: {fxOrderRequest}");
            await fxRequestExecutionProcessor.Execute(context.Message);
        }
    }
}
