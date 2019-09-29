using System.Collections.Generic;
using System.Threading.Tasks;
using Trading.Foundation.Dtos;
using TradingApi.Web.Client;

namespace TradeDesk.ServiceProxies
{
    public class CurrencyService : ApiWebClientBase
    {
        public CurrencyService() : base(LocalUrl) { }

        public async Task<List<CurrencyPair>> GetCurrencyPairsAsync()
        {
            await Client.GetCurrencyPairsAsync(ConfiguredNullRequest);
            return GetResponse<List<CurrencyPair>>().Payload;
        }
    }
}
