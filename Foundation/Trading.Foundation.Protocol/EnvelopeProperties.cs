using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Trading.Foundation.Protocol
{
    public class EnvelopeProperties : ConcurrentDictionary<string, object>
    {
        public EnvelopeProperties() { }
        public EnvelopeProperties(IDictionary<string, object> assignFrom) : base(assignFrom) { }
        public EnvelopeProperties(IEnumerable<KeyValuePair<string, object>> assignFrom) : base(assignFrom) { }
        public EnvelopeProperties(EnvelopeProperties assignFrom) : base(assignFrom) { }
    }
}
