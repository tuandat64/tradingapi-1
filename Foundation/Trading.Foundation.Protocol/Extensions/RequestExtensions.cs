using System;

namespace Trading.Foundation.Protocol.Extensions
{
    public static class RequestExtensions
    {
        public static EnvelopeError AddError<TPayload>(this Request<TPayload> request, EnvelopeError error)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (error == null)
            {
                throw new ArgumentNullException(nameof(error));
            }

            request.AddError(error);

            return error;
        }

        public static EnvelopeError AddError<TPayload>(this Request<TPayload> request, string error)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (error == null)
            {
                throw new ArgumentNullException(nameof(error));
            }

            var envelope = (IEnvelope<TPayload>) request;
            var addedError = envelope.AddError(error);

            return addedError;
        }

        public static TValue AddValue<TPayload, TValue>(this Request<TPayload> request, string key, TValue value)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            var envelope = (IEnvelope<TPayload>) request;
            var addedValue = envelope.AddValue(key, value);

            return addedValue;
        }

        public static IRequest<TNewPropertyType> Map<TProperty, TNewPropertyType>(this IRequest<TProperty> envelope, Func<TProperty, TNewPropertyType> mapOp)
        {
            var requestBuilder = new RequestBuilder<TNewPropertyType>();
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
