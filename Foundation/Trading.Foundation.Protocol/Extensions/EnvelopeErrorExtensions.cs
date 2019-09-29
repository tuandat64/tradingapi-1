using System.Linq;

namespace Trading.Foundation.Protocol.Extensions
{
    public static class EnvelopeErrorExtensions
    {
        public static EnvelopeError[] Combine(this EnvelopeError[] source, params EnvelopeError [] additional)
        {
            return source.Concat(additional).Distinct().ToArray();
        }
    }
}
