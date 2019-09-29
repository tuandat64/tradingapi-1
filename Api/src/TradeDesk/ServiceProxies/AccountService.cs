using System.Collections.Generic;
using System.Threading.Tasks;
using Trading.Foundation.Dtos;
using TradingApi.Web.Client;

namespace TradeDesk.ServiceProxies
{
    public class AccountService : ApiWebClientBase
    {
        public AccountService() : base(LocalUrl){}

        public async Task<List<TradeableAccount>> GetTradeableAccountsAsync()
        {
            await Client.GetTradeableAccountsAsync(ConfiguredNullRequest);
            return GetResponse<List<TradeableAccount>>().Payload;
        }
    }
}
