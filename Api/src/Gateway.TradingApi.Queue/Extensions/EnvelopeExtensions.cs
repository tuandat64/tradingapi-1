using Trading.Foundation.Protocol;
using Trading.Foundation.Protocol.Extensions;

namespace Gateway.TradingApi.Queue.Extensions
{
    public static class EnvelopeExtensions
    {
        public static string ToGatewayString<TPayload>(this IEnvelope<TPayload> source)
        {
            return $"CorrelationId:{source.GetCorrelationId()}, GatewayId: {source.GetGatewayId()}, GatewayPayloadId: {source.GetBusinessTransactionId()}, CustomerId:{source.GetCustomerId()}, Payload:{source.Payload}";
        }
    }
}
