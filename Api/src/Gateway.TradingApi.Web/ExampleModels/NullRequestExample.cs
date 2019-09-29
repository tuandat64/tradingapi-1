using Swashbuckle.AspNetCore.Examples;
using Trading.Foundation.Protocol;

namespace Gateway.TradingApi.Web.Models.SwaggerExamples
{
    public class NullRequestExample : IExamplesProvider
    {
        public object GetExamples()
        {
            return new Request<NullPayload>
            {
                Payload = NullPayload.NullObject
            };
        }
    }
}