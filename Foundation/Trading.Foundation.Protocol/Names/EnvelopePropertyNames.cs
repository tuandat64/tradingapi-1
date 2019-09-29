using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Trading.Foundation.Protocol.Names
{
    public static class EnvelopePropertyNames
    {
        public const string Errors = nameof(Errors);
        public const string CorrelationId = nameof(CorrelationId);
        public const string GatewayId = nameof(GatewayId);
        public const string CustomerId = nameof(CustomerId);
        public const string BusinessTransactionId = nameof(BusinessTransactionId);

        private static Lazy<ISet<string>> all => new Lazy<ISet<string>>(CreateAll);

        public static ISet<string> CreateAll()
        {
            return new HashSet<string>(typeof(EnvelopePropertyNames).GetFields(BindingFlags.Public | BindingFlags.Static)
                                                                                .Select(fi => fi.Name));
        }

        public static ISet<string> All => all.Value;
    }
}
