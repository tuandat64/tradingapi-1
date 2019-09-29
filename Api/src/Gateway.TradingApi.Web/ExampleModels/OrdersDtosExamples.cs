using Gateway.Middleware;
using Swashbuckle.AspNetCore.Examples;

namespace Gateway.TradingApi.Web.Models.SwaggerExamples
{
    public class OrderDtosExamples
    {
        // BestExecution

        public class BuyOrderCcyBestExRequest : IExamplesProvider
        {
            public object GetExamples()
            {
                return OrdersExamples.BuyOrderCcyBestExRequest;
            }
        }

        // RFQs

        public class SellOrderCcyRfqRequest : IExamplesProvider
        {
            public object GetExamples()
            {
                return OrdersExamples.SellOrderCcyRfqRequest;
            }
        }

        public class ExecuteQuoteRequest : IExamplesProvider
        {
            public object GetExamples()
            {
                return OrdersExamples.ExecuteQuoteRequest;
            }
        }

        // track order
        public class TrackOrderRequest : IExamplesProvider
        {
            public object GetExamples()
            {
                return OrdersExamples.CreateTrackOrderRequest();
            }
        }
    }
}