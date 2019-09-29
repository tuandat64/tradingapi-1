using System.Collections.Generic;
using System.Threading.Tasks;
using Trading.Foundation.Dtos;
using TradingApi.Web.Client;

namespace TradeDesk.ServiceProxies
{
    public class PricesService : ApiWebClientBase
    {
        public PricesService() : base(LocalUrl) { }

        public async Task<List<FxIndicativeExchangeRate>> GetRatesAsync()
        {
            await Client.GetIndicativeFxExchangeRatesAsync(ConfiguredNullRequest);
            return GetResponse<List<FxIndicativeExchangeRate>>().Payload;
        }
    }
}
