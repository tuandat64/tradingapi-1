using System;

namespace Trading.Foundation.Protocol
{
    /// <summary>
    /// Message queue discriminator
    /// </summary>
    public interface IRequest<TPayload> : IEnvelope<TPayload>
    {
    }

    public class Request<TPayload> : Envelope<TPayload>, IRequest<TPayload>
    {
        public Request()
        {
        }
    }
}
