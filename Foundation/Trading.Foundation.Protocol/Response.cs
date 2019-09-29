using System.Linq;
using Trading.Foundation.Protocol.Extensions;

namespace Trading.Foundation.Protocol
{
    /// <summary>
    /// Message queue discriminator
    /// </summary>
    public interface IResponse<TPayload> : IEnvelope<TPayload>
    {
    }

    public class Response<TPayload> : Envelope<TPayload>, IResponse<TPayload>
    {
        public Response()
        {
        }
    }
}
