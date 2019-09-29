using System.Collections.Generic;
using System.Threading.Tasks;
using Trading.Foundation.Dtos;
using TradingApi.Web.Client;
using Xunit;

namespace Gateway.TradingApi.Web.Tests.Integration
{
    public class AccountService : ApiWebClientBase
    {
        //public AccountService() : base(VsUrl) { }

        [Fact]
        public async Task Ping()
        {
            var response = await Client.PingAsync();
            Assert.Equal("account service", response);
        }

        [Fact]
        public async Task GetTradeableAccounts()
        {
            await Client.GetTradeableAccountsAsync(ConfiguredNullRequest);
            this.ExpectValidResponse<List<TradeableAccount>>();
        }
    }
}
