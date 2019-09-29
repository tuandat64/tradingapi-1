using System;
using Trading.Foundation.Protocol.Extensions;

namespace Trading.Foundation.Protocol
{
    public abstract class EnvelopeBuilderBase<TEnvelopeType, TProperty> where TEnvelopeType : IEnvelope<TProperty>, new()
    {
        private TEnvelopeType underConstruction;

        protected EnvelopeBuilderBase()
        {
            underConstruction = new TEnvelopeType();
        }

        public EnvelopeBuilderBase<TEnvelopeType, TProperty> AddPayload(TProperty payload)
        {
            underConstruction.Payload = payload;

            return this;
        }

        public EnvelopeBuilderBase<TEnvelopeType, TProperty> AddCustomerId(Guid customerId)
        {
            underConstruction.AddCustomerId(customerId);

            return this;
        }

        public EnvelopeBuilderBase<TEnvelopeType, TProperty> AddGatewayId(Guid gatewayId)
        {
            underConstruction.AddGatewayId(gatewayId);

            return this;
        }

        public EnvelopeBuilderBase<TEnvelopeType, TProperty> AddBusinessTransactionId(Guid businessTransactionId)
        {
            underConstruction.AddBusinessTransactionId(businessTransactionId);

            return this;
        }

        public EnvelopeBuilderBase<TEnvelopeType, TProperty> AddCorrelationId(Guid correlationId)
        {
            underConstruction.AddCorrelationId(correlationId);

            return this;
        }

        public EnvelopeBuilderBase<TEnvelopeType, TProperty> AddValue<TValue>(string key, TValue value)
        {
            underConstruction.AddValue(key, value);

            return this;
        }

        public EnvelopeBuilderBase<TEnvelopeType, TProperty> AddError(string message)
        {
            underConstruction.AddError(message);

            return this;
        }

        public EnvelopeBuilderBase<TEnvelopeType, TProperty> AddErrors(EnvelopeError [] errors)
        {
            underConstruction.AddErrors(errors);

            return this;
        }

        public EnvelopeBuilderBase<TEnvelopeType, TProperty> AddProperties(EnvelopeProperties source)
        {
            underConstruction.AddProperties(source);

            return this;
        }

        public TEnvelopeType ToEnvelope()
        {
            return underConstruction;
        }
    }
}
