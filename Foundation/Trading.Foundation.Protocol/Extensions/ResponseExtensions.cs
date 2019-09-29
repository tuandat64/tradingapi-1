using System;

namespace Trading.Foundation.Protocol.Extensions
{
    public static class ResponseExtensions
    {
        public static IResponse<TNewPropertyType> Map<TProperty, TNewPropertyType>(this IResponse<TProperty> envelope, Func<TProperty, TNewPropertyType> mapOp)
        {
            var requestBuilder = new ResponseBuilder<TNewPropertyType>();
            var builder = requestBuilder.AddPayload(mapOp(envelope.Payload))
                                        .AddProperties(envelope.Properties)
                                        .AddCorrelationId(envelope.GetCorrelationId())
                                        .AddGatewayId(envelope.GetGatewayId())
                                        .AddBusinessTransactionId(envelope.GetBusinessTransactionId())
                                        .AddCustomerId(envelope.GetCustomerId());

            return builder.ToEnvelope();
        }
    }
}
