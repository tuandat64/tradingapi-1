using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trading.Foundation.Dtos;
using TradingApi.Web.Client;
using Xunit;

namespace Gateway.TradingApi.Web.Tests.Integration
{
    public class CurrencyService : ApiWebClientBase
    {
        //public CurrencyService() : base(VsUrl) { }

        [Fact]
        public async Task Ping()
        {
            var response = await Client.Ping2Async();
            Assert.Equal("currency service", response);
        }

        [Fact]
        public async Task GetCurrencyPairs()
        {
            await Client.GetCurrencyPairsAsync(ConfiguredNullRequest);
            this.ExpectValidResponse<List<CurrencyPair>>();
        }

        [Fact]
        public async Task GetIndicativeFxExchangeRates()
        {
            await Client.GetIndicativeFxExchangeRatesAsync(ConfiguredNullRequest);
            this.ExpectValidResponse<List<FxIndicativeExchangeRate>>();
        }

        [Fact]
        public async Task CheckValidCurrencies()
        {
            await Client.GetCurrencyPairsAsync(ConfiguredNullRequest);
            var response = this.ExpectValidResponse<List<CurrencyPair>>();

            var usdBtc = from pairs in response.Payload
                          where pairs.Equals(CurrencyPair.Parse("USD-BTC"))
                          select pairs;

            Assert.True(usdBtc.Count() == 1, "Expected to find one currency pair of USD-BTC");

            var btcUsd = from pairs in response.Payload
                         where pairs.Equals(CurrencyPair.Parse("BTC-USD"))
                         select pairs;

            Assert.True(btcUsd.Count() == 1, "Expected to find one currency pair of BTC-USD");

            var itsFriday = from pairs in response.Payload
                         where pairs.Equals(CurrencyPair.Parse("CryptoFelix-CryptoJeppe"))
                         select pairs;

            Assert.True(itsFriday.Count() == 0, "Did not want to find a currency pair of CryptoFelix-CryptoJeppe");
        }
    }
}
