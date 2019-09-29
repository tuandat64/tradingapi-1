using Gateway.Middleware;
using Gateway.TradingApi.Web.Models.SwaggerExamples;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Examples;
using System.Threading.Tasks;
using Domain;
using Domain.BestExecution;
using Trading.Foundation.Dtos;
using Trading.Foundation.Protocol;

namespace Gateway.TradingApi.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FxBestExecutionController : BaseController<FxBestExecutionController>
    {
        private readonly ILogger<FxBestExecutionController> logger;
        private readonly IFxBestExecutionService fxBestExecutionService;

        public FxBestExecutionController(
            ILogger<FxBestExecutionController> logger,
            IFxBestExecutionService fxBestExecutionService)
        {
            this.logger = logger;
            this.fxBestExecutionService = fxBestExecutionService;
        }

        [AllowAnonymous]
        [HttpGet("Ping")]
        public string Ping()
        {
            return "fx best execution service";
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
        [SwaggerRequestExample(typeof(Request<FxBestExecutionRequest>), typeof(OrderDtosExamples.BuyOrderCcyBestExRequest))]
        public async Task<ActionResult> Create([FromBody] Request<FxBestExecutionRequest> model)
        {
            model = ServiceBase.Enrich(model, "buy or sell currency", logger);
            return Ok(await fxBestExecutionService.Create(model));
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
            return Ok(await fxBestExecutionService.Track(model));
        }

    }
}