using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Trading.Foundation.Protocol.Names;

namespace Trading.Foundation.Protocol.Extensions
{
    public static class EnvelopeExtensions
    {
        private static EnvelopeBuilderBase<Response<TResponseProperty>, TResponseProperty> CreateInternalResponseBuilder<TProperty, TResponseProperty>(this IEnvelope<TProperty> source, TResponseProperty payload)
        {
            var responseBuilder = new ResponseBuilder<TResponseProperty>();
            var builder = responseBuilder.AddPayload(payload)
                                            .AddCorrelationId(source.GetCorrelationId())
                                            .AddGatewayId(source.GetGatewayId())
                                            .AddBusinessTransactionId(source.GetBusinessTransactionId())
                                            .AddCustomerId(source.GetCustomerId());

            return builder;
        }

        private static EnvelopeBuilderBase<Response<TResponseProperty>, TResponseProperty> CreateInternalErroredResponseBuilder<TProperty, TResponseProperty>(this IEnvelope<TProperty> source, TResponseProperty payload)
        {
            var responseBuilder = new ResponseBuilder<TResponseProperty>();
            var builder = responseBuilder.AddPayload(payload);

            return builder;
        }

        public static IResponse<TResponseProperty> CreateResponse<TProperty, TResponseProperty>(this IEnvelope<TProperty> source, TResponseProperty payload)
            where TResponseProperty : class, new()
        {
            return source.CreateInternalResponseBuilder(payload ?? new TResponseProperty()).ToEnvelope();
        }

        public static IResponse<TResponseProperty> CreateMappedResponse<TProperty, TResponseProperty>(this IEnvelope<TProperty> source, Func<TProperty, TResponseProperty> payloadConverter)
            where TResponseProperty : class, new()
        {
            return source.CreateInternalResponseBuilder(payloadConverter(source.Payload) ?? new TResponseProperty()).ToEnvelope();
        }

        public static IResponse<TResponseProperty> CreateErrorResponse<TProperty, TResponseProperty>(this IEnvelope<TProperty> source, string message, TResponseProperty response = default)
            where TResponseProperty : class, new()
        {
            var builder = source.CreateInternalErroredResponseBuilder(new TResponseProperty());
            builder.AddProperties(source.Properties);
            builder.AddError(message);

            return builder.ToEnvelope();
        }

        /// <summary>
        /// Is the envelope valid?
        /// </summary>
        public static IEnvelope<TProperty> ValidateEnvelope<TProperty>(this IEnvelope<TProperty> envelope)
        {
            try
            {
                if (envelope == null)
                {
                    throw new ArgumentNullException(nameof(envelope));
                }

                if (envelope.Payload == null)
                {
                    envelope.AddError(EnvelopeErrorMessageNames.AbsentPayload, ErrorType.Envelope);
                }

                if (envelope.GetCustomerId() == Guid.Empty)
                {
                    envelope.AddError(EnvelopeErrorMessageNames.AbsentCustomerId, ErrorType.Envelope);
                }

                if (envelope.GetGatewayId() == Guid.Empty)
                {
                    envelope.AddError(EnvelopeErrorMessageNames.AbsentGatewayId, ErrorType.Envelope);
                }

                if (envelope.GetBusinessTransactionId() == Guid.Empty)
                {
                    envelope.AddError(EnvelopeErrorMessageNames.AbsentBusinessTransactionId, ErrorType.Envelope);
                }

                if (envelope.GetCorrelationId() == Guid.Empty)
                {
                    envelope.AddError(EnvelopeErrorMessageNames.AbsentCorrelationId, ErrorType.Envelope);
                }
            }
            catch (KeyNotFoundException)
            {
                return envelope;
            }

            return envelope;
        }

        public static Guid AddCustomerId<TProperty>(this IEnvelope<TProperty> envelope, Guid customerId)
        {
            if (envelope == null)
            {
                throw new ArgumentNullException(nameof(envelope));
            }

            if (customerId == Guid.Empty)
            {
                throw new ArgumentOutOfRangeException(nameof(customerId));
            }

            envelope.Properties.AddCustomerId(customerId);

            return customerId;
        }

        public static Guid GetCustomerId<TProperty>(this IEnvelope<TProperty> envelope)
        {
            if (envelope == null)
            {
                throw new ArgumentNullException(nameof(envelope));
            }

            try
            {
                return envelope.Properties.GetCustomerId();
            }
            catch (KeyNotFoundException)
            {
                envelope.AddError(EnvelopeErrorMessageNames.AbsentCustomerId, ErrorType.Envelope);
            }

            return Guid.Empty;
        }

        public static Guid AddGatewayId<TProperty>(this IEnvelope<TProperty> envelope, Guid gatewayId)
        {
            if (envelope == null)
            {
                throw new ArgumentNullException(nameof(envelope));
            }

            if (gatewayId == Guid.Empty)
            {
                throw new ArgumentOutOfRangeException(nameof(gatewayId));
            }

            envelope.Properties.AddGatewayId(gatewayId);

            return gatewayId;
        }

        public static Guid GetGatewayId<TProperty>(this IEnvelope<TProperty> envelope)
        {
            if (envelope == null)
            {
                throw new ArgumentNullException(nameof(envelope));
            }

            try
            {
                return envelope.Properties.GetGatewayId();
            }
            catch (KeyNotFoundException)
            {
                envelope.AddError(EnvelopeErrorMessageNames.AbsentGatewayId, ErrorType.Envelope);
            }

            return Guid.Empty;
        }

        public static Guid AddBusinessTransactionId<TProperty>(this IEnvelope<TProperty> envelope, Guid businessTransactionId)
        {
            if (envelope == null)
            {
                throw new ArgumentNullException(nameof(envelope));
            }

            if (businessTransactionId == Guid.Empty)
            {
                throw new ArgumentOutOfRangeException(nameof(businessTransactionId));
            }

            envelope.Properties.AddBusinessTransactionId(businessTransactionId);

            return businessTransactionId;
        }

        public static Guid GetBusinessTransactionId<TProperty>(this IEnvelope<TProperty> envelope)
        {
            if (envelope == null)
            {
                throw new ArgumentNullException(nameof(envelope));
            }

            try
            {
                return envelope.Properties.GetBusinessTransactionId();
            }
            catch (KeyNotFoundException)
            {
                envelope.AddError(EnvelopeErrorMessageNames.AbsentBusinessTransactionId, ErrorType.Envelope);
            }

            return Guid.Empty;
        }

        public static Guid AddCorrelationId<TProperty>(this IEnvelope<TProperty> envelope, Guid correlationId)
        {
            if (envelope == null)
            {
                throw new ArgumentNullException(nameof(envelope));
            }

            if (correlationId == Guid.Empty)
            {
                throw new ArgumentOutOfRangeException(nameof(correlationId));
            }

            envelope.Properties.AddCorrelationId(correlationId);

            return correlationId;
        }

        public static Guid GetCorrelationId<TProperty>(this IEnvelope<TProperty> envelope)
        {
            if (envelope == null)
            {
                throw new ArgumentNullException(nameof(envelope));
            }

            try
            {
                return envelope.Properties.GetCorrelationId();
            }
            catch (KeyNotFoundException)
            {
                envelope.AddError(EnvelopeErrorMessageNames.AbsentCorrelationId, ErrorType.Envelope);
            }

            return Guid.Empty;
        }

        // errors
        public static EnvelopeError[] GetErrors<TProperty>(this IEnvelope<TProperty> envelope, Func<EnvelopeError, bool> filter = null)
        {
            if (envelope == null)
            {
                throw new ArgumentNullException(nameof(envelope));
            }

            return envelope.Properties.GetErrors(filter);
        }

        public static EnvelopeError[] GetPayloadErrors<TProperty>(this IEnvelope<TProperty> envelope)
        {
            return envelope.GetErrors(e => e.ErrorType == ErrorType.Payload);
        }

        public static EnvelopeError[] GetEnvelopeErrors<TProperty>(this IEnvelope<TProperty> envelope)
        {
            return envelope.GetErrors(e => e.ErrorType == ErrorType.Envelope);
        }

        public static EnvelopeError[] AddErrors<TProperty>(this IEnvelope<TProperty> envelope, params EnvelopeError[] errors)
        {
            if (envelope == null)
            {
                throw new ArgumentNullException(nameof(envelope));
            }

            if (errors == null)
            {
                throw new ArgumentNullException(nameof(errors));
            }

            envelope.Properties.AddErrors(errors);

            return errors;
        }

        public static EnvelopeError AddError<TProperty>(this IEnvelope<TProperty> envelope, EnvelopeError error)
        {
            if (envelope == null)
            {
                throw new ArgumentNullException(nameof(envelope));
            }

            if (error == null)
            {
                throw new ArgumentNullException(nameof(error));
            }

            envelope.AddErrors(error);

            return error;
        }

        public static EnvelopeError AddError<TProperty>(this IEnvelope<TProperty> envelope, string error, ErrorType errorType = ErrorType.Payload)
        {
            return envelope.AddError(new EnvelopeError(error, errorType));
        }

        public static TValue AddValue<TProperty, TValue>(this IEnvelope<TProperty> envelope, string key, TValue value)
        {
            if (envelope == null)
            {
                throw new ArgumentNullException(nameof(envelope));
            }

            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            return envelope.Properties.AddValue(key, value);
        }

        public static TValue GetValue<TProperty, TValue>(this IEnvelope<TProperty> envelope, string key)
        {
            if (envelope == null)
            {
                throw new ArgumentNullException(nameof(envelope));
            }

            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            return envelope.Properties.GetValue<TValue>(key);
        }

        internal static IEnvelope<TProperty> AddProperties<TProperty>(this IEnvelope<TProperty> envelope, EnvelopeProperties source)
        {
            var json = JsonConvert.SerializeObject(source);
            var clone = JsonConvert.DeserializeObject<EnvelopeProperties>(json);

            foreach (var kvp in clone)
            {
                envelope.Properties.TryAdd(kvp.Key, kvp.Value);
            }

            return envelope;
        }

        public static IRequest<TProperty> CreateRequest<TProperty>(this IEnvelope<TProperty> envelope, Guid correlationId, Guid gatewayId, Guid customerId, Guid businessTransactionId)
        {
            var requestBuilder = new RequestBuilder<TProperty>();
            var builder = requestBuilder.AddPayload(envelope.Payload)
                                        .AddProperties(envelope.Properties)
                                        .AddCorrelationId(correlationId)
                                        .AddGatewayId(gatewayId)
                                        .AddBusinessTransactionId(businessTransactionId)
                                        .AddCustomerId(customerId);

            return builder.ToEnvelope();
        }
    }
}
