using Gateway.Middleware;
using Gateway.TradingApi.Web.Models.SwaggerExamples;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Examples;
using System.Threading.Tasks;
using Domain;
using Domain.Currency;
using Trading.Foundation.Protocol;

namespace Gateway.TradingApi.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CurrencyController : BaseController<CurrencyController>
    {
        private readonly ILogger<CurrencyController> logger;
        private readonly ICurrencyService currencyService;

        public CurrencyController(ILogger<CurrencyController> logger, ICurrencyService currencyService)
        {
            this.logger = logger;
            this.currencyService = currencyService;
        }

        [AllowAnonymous]
        [HttpGet("Ping")]
        public string Ping()
        {
            return "currency service";
        }

        /// <summary>
        /// Returns all the tradable currencies which can be used when placing orders.
        /// </summary>
        /// <remarks>
        /// Returns all the tradable currencies which can be used when placing orders,
        /// see ‘/order/create’ endpoint.
        /// </remarks>
        [HttpPost]
        [Route("GetCurrencyPairs")]
        [NoAuthN]
        [SwaggerRequestExample(typeof(Request<NullPayload>), typeof(NullRequestExample))]
        public async Task<ActionResult> GetCurrencyPairs(Request<NullPayload> request)
        {
            request = ServiceBase.Enrich(request, "get tradeable currency pairs", logger);
            return Ok(await currencyService.GetCurrencyPairs(request));
        }

        /// <summary>
        /// Returns all the indicative rates for tradeable currencies.
        /// </summary>
        /// <remarks>
        /// </remarks>
        [HttpPost]
        [Route("GetIndicativeFxExchangeRates")]
        [NoAuthN]
        [SwaggerRequestExample(typeof(Request<NullPayload>), typeof(NullRequestExample))]
        public async Task<ActionResult> GetIndicativeFxExchangeRates(Request<NullPayload> request)
        {
            request = ServiceBase.Enrich(request, "get indicative exchange rates", logger);
            return Ok(await currencyService.GetIndicativeFxExchangeRates(request));
        }
    }
}