using Trading.Foundation.Protocol.Extensions;
using Xunit;

namespace Trading.Foundation.Protocol.Test
{
    public class ErrorTests
    {
        [Fact]
        public void ShouldStoreSinglePayloadError()
        {
            IRequest<NullPayload> r = new Request<NullPayload>();
            r.AddError("the cat sat on the mat");

            Assert.Single(r.GetPayloadErrors());
            Assert.True(r.HasPayloadError);

            Assert.Empty(r.GetEnvelopeErrors());
            Assert.False(r.HasEnvelopeError);
            Assert.True(r.HasError);
        }

        [Fact]
        public void ShouldStoreSingleEnvelopeError()
        {
            IRequest<NullPayload> r = new Request<NullPayload>();
            r.AddError("the cat sat on the mat", ErrorType.Envelope);

            Assert.Single(r.GetEnvelopeErrors());
            Assert.True(r.HasEnvelopeError);

            Assert.Empty(r.GetPayloadErrors());
            Assert.False(r.HasPayloadError);
            Assert.True(r.HasError);
        }

        [Fact]
        public void ShouldBeInvalidEnvelope()
        {
            IRequest<NullPayload> r = new Request<NullPayload>();

            Assert.True(r.ValidateEnvelope().HasEnvelopeError);
            Assert.False(r.HasPayloadError);

            Assert.True(r.HasError);
        }
    }
}
