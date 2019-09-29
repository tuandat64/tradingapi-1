using Microsoft.AspNetCore.Mvc.Filters;
using System;
using Trading.Foundation.Protocol;

namespace Gateway.Middleware
{
    public class Attribution
    {
        public static readonly Guid GatewayId = Guid.Parse("C5EECBA1-CD1A-46DA-B34E-223578BEFD33");

        public static void InjectGatewaySettings(Guid customerId, ActionExecutingContext actionContext)
        {
            foreach (var arg in actionContext.ActionArguments)
            {
                if (arg.Value != null)
                {
                    Type baseType = arg.Value.GetType();
                    var props = baseType.GetProperty("Properties");
                    if (props == null) continue;
                    var envelopeProps = (EnvelopeProperties)props.GetValue(arg.Value);
                    envelopeProps["CustomerId"] = customerId;
                    envelopeProps["GatewayId"] = GatewayId;
                }
            }
        }
    }
}
