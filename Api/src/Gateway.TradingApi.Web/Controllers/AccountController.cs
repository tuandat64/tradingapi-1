using Gateway.Middleware;
using Gateway.TradingApi.Web.Middleware;
using Gateway.TradingApi.Web.Models.SwaggerExamples;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Examples;
using System.Threading.Tasks;
using Domain;
using Domain.Account;
using Trading.Foundation.Protocol;

namespace Gateway.TradingApi.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : BaseController<AccountController>
    {
        private readonly ILogger<AccountController> logger;
        private readonly IAccountService channelAccountService;

        public AccountController(ILogger<AccountController> logger,
                                 IAccountService channelAccountService)
        {
            this.logger = logger;
            this.channelAccountService = channelAccountService;
        }

        [AllowAnonymous]
        [HttpGet("Ping")]
        public string Ping()
        {
            return "account service";
        }

        /// <summary>
        /// Returns all tradeable accounts associated with the client of the api key being used.
        /// </summary>
        /// <remarks>
        /// Returns all tradeable accounts associated with the client of the api key being used.
        /// </remarks>
        [HttpPost]
        [Route("GetTradeableAccounts")]
        [NoAuthN]
        [SwaggerRequestExample(typeof(Request<NullPayload>), typeof(NullRequestExample))]
        public async Task<ActionResult> GetTradeableAccounts(Request<NullPayload> request)
        {
            request = ServiceBase.Enrich(request, "get tradeable accounts", logger);
            var response = await channelAccountService.GetTradeableAccounts(request);
            return Ok(response);
        }
    }
}