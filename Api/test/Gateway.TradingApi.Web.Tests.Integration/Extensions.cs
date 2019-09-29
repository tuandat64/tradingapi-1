using System.Linq;
using Trading.Foundation.Protocol;
using Trading.Foundation.Protocol.Extensions;
using TradingApi.Web.Client;
using Xunit;

namespace Gateway.TradingApi.Web.Tests.Integration
{
    static class Extensions
    {
        public static Response<T> ExpectValidResponse<T>(this ApiWebClientBase webClient)
        {
            var response = webClient.Client.Response<Response<T>>();
            response.ValidateEnvelope();
            var errors = string.Join(",", response.GetErrors().Select(p => p.ToString()).ToArray());
            Assert.False(response.HasError, $"response should have no errors - {errors}");
            return response;
        }

        public static Response<T> ExpectInvalidResponse<T>(this ApiWebClientBase webClient)
        {
            var response = webClient.Client.Response<Response<T>>();
            response.ValidateEnvelope();
            Assert.True(response.HasError, "response should have errors");
            return response;
        }

        public static Response<T> ExpectResponseHasOnlyPayloadErrors<T>(this ApiWebClientBase webClient)
        {
            var response = webClient.Client.Response<Response<T>>();
            response.ValidateEnvelope();
            Assert.True(response.HasPayloadError, "response should have payload errors");

            var errors = string.Join(",", response.GetEnvelopeErrors().Select(p => p.ToString()).ToArray());
            Assert.False(response.HasEnvelopeError, $"response should not have envelope errors - {response.GetEnvelopeErrors().ToString()}");
            return response;
        }
    }
}
