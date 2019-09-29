using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Domain.Dtos;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Trading.Foundation.Dtos;
using Trading.Foundation.Protocol;
using Trading.Foundation.Protocol.Extensions;

namespace Domain.Currency
{
    /// <summary>
    /// Implements <see cref="ICurrencyService"/>, returns the live view of traded currency pairs in used exchanges.
    /// </summary>
    public class CurrencyService : ServiceBase, ICurrencyService
    {
        private static readonly string CURRENCY_PAIRS_MEMORY_KEY = "CurrencyService.CurrencyPairs";
        private static readonly string INDICATIVE_EXCHANGE_RATE_MEMORY_KEY = "CurrencyService.IndicativeExchangeRate";
        
        private readonly string exchangeRateServiceUrl;

        private readonly int currencyPairCacheExpSeconds;
        private readonly int rateCacheExpSeconds;
        private readonly IMemoryCache memoryCache;

        public CurrencyService(ILogger<CurrencyService> logger, 
                                    IConfigurationRoot configuration, 
                                    IMemoryCache memoryCache) 
            : base(logger)
        {
            exchangeRateServiceUrl = configuration.GetSection("AppSettings:ExchangeRateServiceUri").Value;
            currencyPairCacheExpSeconds = int.Parse(configuration.GetSection("AppSettings:CurrencyPair.Cache.ExpirationSeconds").Value);
            rateCacheExpSeconds = int.Parse(configuration.GetSection("AppSettings:IndicativeExchangeRate.Cache.ExpirationSeconds").Value);
            this.memoryCache = memoryCache;
        }

        /// <summary>
        /// Returns  response with live traded list of <see cref="CurrencyPair"/>>.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<IResponse<List<CurrencyPair>>> GetCurrencyPairs(IRequest<NullPayload> request)
        {
            await Task.Yield();
            return GetResponse(request, () =>
                {
                    return memoryCache.GetOrCreate(CURRENCY_PAIRS_MEMORY_KEY, entry =>
                    {
                        entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(currencyPairCacheExpSeconds);
                        return LoadMarketDataRates()
                            .SelectMany(x => new[] {CurrencyPair.Parse($"{x.FromCurrencyCode}-{x.ToCurrencyCode}"), CurrencyPair.Parse($"{x.ToCurrencyCode}-{x.FromCurrencyCode}") })
                            .Distinct()
                            .ToList();
                    });
                }
            );
        }

        /// <summary>
        /// Returns  response with live traded list of <see cref="CurrencyPair"/>>.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<IResponse<List<CurrencyPair>>> GetCurrencyPairsX(IRequest<NullPayload> request)
        {
            await Task.Yield();

            if ((request.ValidateEnvelope().HasEnvelopeError))
            {
                return InvalidRequest<List<CurrencyPair>, NullPayload>(Logger, request);
            }

            var responsePayload = new List<CurrencyPair>
            {
                CurrencyPair.Parse("BTC-USD"),
                CurrencyPair.Parse("BTC-CHF"),
                CurrencyPair.Parse("USD-BTC"),
                CurrencyPair.Parse("CHF-BTC")
            };

            return request.CreateResponse(responsePayload);
        }

        /// <summary>
        /// Returns response with indicative fx exchange rate based on live exchange orders/trade.
        /// </summary>
        /// <param name="request"></param>
        /// <returns>the list of <see cref="FxIndicativeExchangeRate"/>></returns>
        public async Task<IResponse<List<FxIndicativeExchangeRate>>> GetIndicativeFxExchangeRates(IRequest<NullPayload> request)
        {
            await Task.Yield();
            return GetResponse(request, () =>
                {
                    return memoryCache.GetOrCreate(INDICATIVE_EXCHANGE_RATE_MEMORY_KEY, entry =>
                    {
                        entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(rateCacheExpSeconds);
                        return GetIndicativeRates(LoadMarketDataRates()).ToList();
                    });
                }
            );
        }

        private IEnumerable<FxIndicativeExchangeRate> GetIndicativeRates(IEnumerable<Rate> rates)
        {
            bool ShouldBeInverted(Rate rate) => 
                rate.FromCurrencyType == "Fiat" && rate.ToCurrencyType != "Fiat" 
                || rate.FromCurrencyType != "Fiat" && rate.ToCurrencyType != "Fiat" && (rate.Value > 1 || rate.Ask > 1 || rate.Bid > 1);

            (CurrencyPair, decimal) GetIndicativeRate(Rate rate) =>
                ShouldBeInverted(rate)
                    ? (CurrencyPair.Parse($"{rate.ToCurrencyCode}-{rate.FromCurrencyCode}"), rate.Ask == 0m || rate.Bid == 0m ? 1.0m / rate.Value : 2.0m / (rate.Ask + rate.Bid))
                    : (CurrencyPair.Parse($"{rate.FromCurrencyCode}-{rate.ToCurrencyCode}"), rate.Ask == 0m || rate.Bid == 0m ? rate.Value : (rate.Ask + rate.Bid) / 2.0m);
            CurrencyPair InverseCurrencyPair(CurrencyPair pair) => CurrencyPair.Parse($"{pair.Quote}-{pair.Base}");

            var indicativeRates = rates.Select(GetIndicativeRate);
            Dictionary<CurrencyPair, List<decimal>> currencyPairAggregatedRate = new Dictionary<CurrencyPair, List<decimal>>();
            foreach (var indicativeRate in indicativeRates)
            {
                if (currencyPairAggregatedRate.TryGetValue(indicativeRate.Item1, out List<decimal> list))
                {
                    list.Add(indicativeRate.Item2);
                } else if (currencyPairAggregatedRate.TryGetValue(InverseCurrencyPair(indicativeRate.Item1), out list))
                {
                    list.Add(1.0m/indicativeRate.Item2);
                }
                else
                {
                    currencyPairAggregatedRate.Add(indicativeRate.Item1, new List<decimal> { indicativeRate.Item2 });
                }
            }

            return currencyPairAggregatedRate.Select(x => new FxIndicativeExchangeRate() {Pair = x.Key, Rate = x.Value.Average()});
        }

        private IResponse<T> GetResponse<T>(IRequest<NullPayload> request, Func<T> loadDataFunction) 
            where T : class, new()
        {
            if (request.ValidateEnvelope().HasEnvelopeError)
            {
                return InvalidRequest<T, NullPayload>(Logger, request);
            }
            try
            {
                return ProcessResponse(request.CreateResponse(loadDataFunction()));
            }
            catch (Exception e)
            {
                Logger.LogError(e.ToString());
                var failed = request.CreateResponse(new T());
                failed.AddError("Failed to get exchange rate from exchange service (internal error)");
                return ProcessResponse(failed);
            }
        }

        private IEnumerable<Rate> LoadMarketDataRates()
        {
            string responseString = string.Empty;
            try
            {
                using (var client = new HttpClient())
                {
                    var getResponse = client.GetAsync(exchangeRateServiceUrl + "/ExchangeRate/Rates");
                    getResponse.Wait();

                    var streamReader = new StreamReader(getResponse.Result.Content.ReadAsStreamAsync().Result);
                    responseString = streamReader.ReadToEnd();

                    ExchangeResponse ratesResponse =
                        JsonConvert.DeserializeObject<ExchangeResponse>(responseString);
                    return ratesResponse.Rates
                        .Where(x => x.IsTradableSource 
                                    && x.FromCurrencyType != "Unknown" && x.ToCurrencyType != "Unknown" 
                                    && (x.Value > 0 || x.Ask > 0 && x.Bid > 0))
                        .OrderBy(x => x.FromCurrencyCode)
                        .ThenBy(x => x.ToCurrencyCode);

                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Failed to parse response: {responseString}");
                throw ex;
            }
        }
    }
}
