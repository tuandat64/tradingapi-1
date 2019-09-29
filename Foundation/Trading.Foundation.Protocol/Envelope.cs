using Newtonsoft.Json;
using System.Linq;
using Trading.Foundation.Protocol.Extensions;

namespace Trading.Foundation.Protocol
{
    public interface IEnvelope<TPayload>
    {
        TPayload Payload { get; set; }

        EnvelopeProperties Properties { get; set; }

        bool HasError { get; }

        bool HasEnvelopeError { get; }

        bool HasPayloadError { get;  }
    }

    /// <summary>
    /// Envelope for channel request /response DTOs
    /// </summary>
    /// <typeparam name="TPayload"></typeparam>
    public class Envelope<TPayload> : IEnvelope<TPayload>
    {
        /// <summary>
        /// Pretty printing serializer
        /// </summary>
        public override string ToString() => JsonConvert.SerializeObject(this, Formatting.Indented);

        /// <summary>
        /// Request Payload
        /// </summary>
        public TPayload Payload { get; set; }

        /// <summary>
        /// Properties Collection
        /// </summary>
        public EnvelopeProperties Properties { get; set; } = new EnvelopeProperties();

        public bool HasError => Properties.GetErrors().Any();

        public bool HasEnvelopeError => Properties.GetErrors(e => e.ErrorType == ErrorType.Envelope).Any();

        public bool HasPayloadError => Properties.GetErrors(e => e.ErrorType == ErrorType.Payload).Any();
    }
}
