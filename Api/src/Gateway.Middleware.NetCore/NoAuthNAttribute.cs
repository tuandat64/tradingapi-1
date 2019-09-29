
using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace Gateway.Middleware
{
    public class NoAuthNAttribute : ActionFilterAttribute
    {
        public static Guid CustomerId = Guid.Parse("0726DEF5-3E84-4FA7-93C0-18B4EFD379CD");

        public override void OnActionExecuting(ActionExecutingContext actionContext)
        {
            // Inject customer id and API id into downstream request
            Attribution.InjectGatewaySettings(CustomerId, actionContext);
        }
    }
}
