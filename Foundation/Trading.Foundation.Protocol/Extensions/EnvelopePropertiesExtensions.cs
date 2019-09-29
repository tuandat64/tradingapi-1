using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Newtonsoft.Json.Linq;
using Trading.Foundation.Protocol.Names;

namespace Trading.Foundation.Protocol.Extensions
{
    public static class EnvelopePropertiesExtensions
    {
        private static TValue ExtractPropertyValue<TValue>(EnvelopeProperties envelopeProperties, string key)
        {
            if (envelopeProperties == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (!envelopeProperties.TryGetValue(key, out var entity))
            {
                throw new KeyNotFoundException($"Missing {key} Property");
            }

            var jArray = entity as Newtonsoft.Json.Linq.JArray;

            if (jArray == null)
            {
                return TypeDescriptor.GetConverter(typeof(TValue)).CanConvertFrom(entity.GetType()) ?
                                    (TValue) TypeDescriptor.GetConverter(typeof(TValue)).ConvertFrom(entity) :
                                    (TValue)Convert.ChangeType(entity, typeof(TValue));
            }

            return jArray.ToObject<TValue>();
        }

        // custom value
        public static TValue AddValue<TValue>(this EnvelopeProperties envelopeProperties, string key, TValue value)
        {
            if (envelopeProperties == null)
            {
                throw new ArgumentNullException(nameof(envelopeProperties));
            }

            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            envelopeProperties.AddOrUpdate(key, value, (s, o) => value);

            return value;
        }

        public static TValue GetValue<TValue>(this EnvelopeProperties envelopeProperties, string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            return ExtractPropertyValue<TValue>(envelopeProperties, key);
        }

        private static Guid AddImmutableProperty(this EnvelopeProperties envelopeProperties, string propertyName, Guid propertyValue)
        {
            if (envelopeProperties == null)
            {
                throw new ArgumentNullException(nameof(envelopeProperties));
            }

            envelopeProperties.TryAdd(propertyName, propertyValue);

            return propertyValue;
        }

        //CorrelationId
        public static Guid GetCorrelationId(this EnvelopeProperties envelopeProperties)
        {
            return ExtractPropertyValue<Guid>(envelopeProperties, EnvelopePropertyNames.CorrelationId);
        }

        public static Guid AddCorrelationId(this EnvelopeProperties envelopeProperties, Guid correlationId)
        {
            return AddImmutableProperty(envelopeProperties, EnvelopePropertyNames.CorrelationId, correlationId);
        }

        //GatewayId
        public static Guid GetGatewayId(this EnvelopeProperties envelopeProperties)
        {
            return ExtractPropertyValue<Guid>(envelopeProperties, EnvelopePropertyNames.GatewayId);
        }

        public static Guid AddGatewayId(this EnvelopeProperties envelopeProperties, Guid gatewayId)
        {
            return AddImmutableProperty(envelopeProperties, EnvelopePropertyNames.GatewayId, gatewayId);
        }

        //CustomerId
        public static Guid GetCustomerId(this EnvelopeProperties envelopeProperties)
        {
            return ExtractPropertyValue<Guid>(envelopeProperties, EnvelopePropertyNames.CustomerId);
        }

        public static Guid AddCustomerId(this EnvelopeProperties envelopeProperties, Guid customerId)
        {
            return AddImmutableProperty(envelopeProperties, EnvelopePropertyNames.CustomerId, customerId);
        }

        //BusinessTransactionId
        public static Guid GetBusinessTransactionId(this EnvelopeProperties envelopeProperties)
        {
            return ExtractPropertyValue<Guid>(envelopeProperties, EnvelopePropertyNames.BusinessTransactionId);
        }

        public static Guid AddBusinessTransactionId(this EnvelopeProperties envelopeProperties, Guid businessTransactionId)
        {
            return AddImmutableProperty(envelopeProperties, EnvelopePropertyNames.BusinessTransactionId, businessTransactionId);
        }

        //Errors
        public static EnvelopeError[] GetErrors(this EnvelopeProperties envelopeProperties, Func<EnvelopeError, bool> filter = null)
        {
            filter = filter ?? (_ => true);

            try
            {
                return ExtractPropertyValue<EnvelopeError[]>(envelopeProperties, EnvelopePropertyNames.Errors).Where(filter).ToArray();
            }
            catch (KeyNotFoundException)
            {
                return new EnvelopeError[0];
            }
        }

        public static EnvelopeError[] AddErrors(this EnvelopeProperties envelopeProperties, params EnvelopeError[] errors)
        {
            if (envelopeProperties == null)
            {
                throw new ArgumentNullException(nameof(envelopeProperties));
            }

            if (errors == null)
            {
                throw new ArgumentNullException(nameof(errors));
            }

            EnvelopeError[] Combine(object source, EnvelopeError[] additional)
            {
                EnvelopeError[] existing;

                if (source is JArray jArray)
                {
                    existing = jArray.ToObject<EnvelopeError[]>();
                }
                else
                {
                    existing = (EnvelopeError[]) Convert.ChangeType(source, typeof(EnvelopeError[]));
                }

                return existing.Concat(additional).Distinct().ToArray();
            }

            envelopeProperties.AddOrUpdate(EnvelopePropertyNames.Errors, errors, (k, v) => Combine(v, errors));

            return errors;
        }

        public static EnvelopeError AddError(this EnvelopeProperties envelopeProperties, string error)
        {
            if (error == null)
            {
                throw new ArgumentNullException(nameof(error));
            }

            var resultError = new EnvelopeError(error);

            envelopeProperties.AddErrors(resultError);

            return resultError;
        }
    }
}
