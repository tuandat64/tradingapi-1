using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Gateway.TradingApi.Web.Controllers
{
    public class BaseController<T> : Controller where T : class
    {
        protected readonly JsonSerializerSettings _serializerSettings;

        public BaseController() 
        {
            _serializerSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore
            };
        }
    }
}
