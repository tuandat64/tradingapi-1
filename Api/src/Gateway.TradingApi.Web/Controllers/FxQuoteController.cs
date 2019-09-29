using Gateway.Middleware;
using Gateway.TradingApi.Web.Models.SwaggerExamples;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Examples;
using System.Threading.Tasks;
using Domain;
using Domain.Quote;
using Trading.Foundation.Dtos;
using Trading.Foundation.Protocol;

namespace Gateway.TradingApi.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FxQuoteController : BaseController<FxQuoteController>
    {
        private readonly ILogger<FxQuoteController> logger;
        private readonly IFxQuoteService quoteService;

        public FxQuoteController(ILogger<FxQuoteController> logger, IFxQuoteService quoteService)
        {
            this.logger = logger;
            this.quoteService = quoteService;
        }

        [AllowAnonymous]
        [HttpGet("Ping")]
        public string Ping()
        {
            return "fx quote service";
        }

        /// <summary>
        /// Create buy orders using currency pairs
        /// the orders will be processed according to the specified execution strategy
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Create")]
        [NoAuthN]
        [SwaggerRequestExample(typeof(Request<FxQuoteRequest>), typeof(OrderDtosExamples.SellOrderCcyRfqRequest))]
        public async Task<ActionResult> Create([FromBody] Request<FxQuoteRequest> model)
        {
            model = ServiceBase.Enrich(model, "buy or sell currency", logger);
            return Ok(await quoteService.Create(model));
        }

        [HttpPost]
        [Route("Execute")]
        [NoAuthN]
        [SwaggerRequestExample(typeof(Request<FxExecuteQuoteRequest>), typeof(OrderDtosExamples.ExecuteQuoteRequest))]
        public async Task<ActionResult> Execute([FromBody] Request<FxExecuteQuoteRequest> model)
        {
            model = ServiceBase.Enrich(model, "execute quote", logger);
            return Ok(await quoteService.Execute(model));
        }

        /// <summary>
        /// For tracking orders
        /// </summary>
        /// <remarks>
        /// Supplies detailed information about specific orders.
        /// Multiple order details can be sent in a single request.
        /// </remarks>
        [HttpPost]
        [Route("Track")]
        [NoAuthN]
        [SwaggerRequestExample(typeof(Request<TrackOrder>), typeof(OrderDtosExamples.TrackOrderRequest))]
        public async Task<ActionResult> Track([FromBody] Request<TrackOrder> model)
        {
            model = ServiceBase.Enrich(model, "track order", logger);
            return Ok(await quoteService.Track(model));
        }
    }
}