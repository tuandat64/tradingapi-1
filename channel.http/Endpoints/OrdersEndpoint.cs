using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using channel.http.Models;
using channel.http.Services;
using Microsoft.AspNetCore.Http;


/// TODO
/// https://medium.com/@matteocontrini/consistent-error-responses-in-asp-net-core-web-apis-bb70b435d1f8
/// https://www.codingame.com/playgrounds/35462/creating-web-api-in-asp-net-core-2-0/part-1---web-api

namespace channel.http.Endpoints
{
    /// <summary>
    /// blah blah blah
    /// </summary>
    [Route("orders")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        readonly OrdersService service = new OrdersService();

        protected async Task<ActionResult> Out<T>(T response)
        {
            // TODO - fix status code
            //if (response == null)
            //{
            //    return Content(HttpStatusCode.BadRequest, response);
            //}

            var task = await Task.FromResult(response);
            return Ok(task);
        }

        /// <summary>
        /// blah
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("createquote/buy")]
        public async Task<ActionResult>
            Create([FromBody] QuoteOrders.BuyQuoteRequest request) =>
                await Out(service.Create(request));

        /// <summary>
        /// blah
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("createquote/sell")]
        public async Task<ActionResult>
            Create([FromBody] QuoteOrders.SellQuoteRequest request) =>
                await Out(service.Create(request));

        /// <summary>
        /// blah
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("createslicespot/buy")]
        public async Task<ActionResult>
            Create([FromBody] SlicingOrders.BuySliceSpotRequest request) =>
                await Out(service.Create(request));

        /// <summary>
        /// blah
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("createslicespot/sell")]
        public async Task<ActionResult>
            Create([FromBody] SlicingOrders.SellSliceSpotRequest request) =>
                await Out(service.Create(request));

        [HttpGet]
        [Route("quotestatus/{id}")]
        public async Task<ActionResult>
            QuoteStatus(string id) =>
                await Out(service.QuoteOrderStatus(id));

        [HttpGet]
        [Route("slicestatus/{id}")]
        public async Task<ActionResult>
            SliceStatus(string id) =>
                await Out(service.SliceOrderStatus(id));
    }
}