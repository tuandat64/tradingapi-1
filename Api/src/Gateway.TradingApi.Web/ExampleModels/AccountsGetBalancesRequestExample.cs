using Swashbuckle.AspNetCore.Examples;
using Trading.Foundation.Dtos;
using Trading.Foundation.Protocol;

namespace Gateway.TradingApi.Web.Models.SwaggerExamples
{
    public class AccountsGetBalancesRequestExample : IExamplesProvider
    {
        public object GetExamples()
        {
            var model = new Request<TradeableAccountsSummaryRequest>
            {
                Payload = 
                    new TradeableAccountsSummaryRequest
                    {
                        SummaryCurrency = "USD"
                    }
            };

            return model;
        }
    }
}