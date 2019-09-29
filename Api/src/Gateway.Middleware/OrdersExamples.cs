using Trading.Foundation.Dtos;
using Trading.Foundation.Protocol;

namespace Gateway.Middleware
{
    public static class OrdersExamples
    {
        // BEST_EXECUTION

        public static readonly string BitcoinAccountName = "BTCS-ACC-BTC-711421";
        public static readonly string BitcoinAccountId = "0FFFEB78-074F-44C7-A84B-129C7553CC0C";

        public static readonly string UsdAccountName = "BTCS-ACC-USD-711418";
        public static readonly string UsdAccountId = "881C5726-6E72-4D87-891C-B5F9B94AC6A2";

        // TODO read JSON from sample file
        public static object Get(string name, string correlationId = null)
        {
            return null;
        }

        public static Request<FxBestExecutionRequest> BuyOrderCcyBestExRequest
        {
            get
            {
                var model = new Request<FxBestExecutionRequest>
                {
                    Payload = new FxBestExecutionRequest
                    {
                        Comment = "best execution currency pair buy order",
                        Quantity = 0.01M,
                        Pair = CurrencyPair.Parse("BTC-USD"),
                        Side = Side.Buy
                    }
                };
                return model;
            }
        }

        public static Request<FxBestExecutionRequest> SellOrderCcyBestExRequest
        {
            get
            {
                var model = new Request<FxBestExecutionRequest>
                {
                    Payload = new FxBestExecutionRequest
                    {
                        Comment = "best execution currency pair sell order",
                        Quantity = 100,
                        Pair = CurrencyPair.Parse("USD-BTC"),
                        Side = Side.Sell
                    }
                };
                return model;
            }
        }

        // RFQs

        public static Request<FxQuoteRequest> BuyOrderCcyRfqRequest
        {
            get
            {
                var model = new Request<FxQuoteRequest>
                {
                    Payload = new FxQuoteRequest
                    {
                        Quantity = 0.01M,
                        Pair = CurrencyPair.Parse("BTC-USD"),
                        Side = Side.Buy
                    }
                };
                return model;
            }
        }

        public static Request<FxQuoteRequest> SellOrderCcyRfqRequest
        {
            get
            {
                var model = new Request<FxQuoteRequest>
                {
                    Payload = new FxQuoteRequest
                    {
                        Quantity = 100,
                        Pair = CurrencyPair.Parse("USD-BTC"),
                        Side = Side.Sell
                    }
                };
                return model;
            }
        }

        public static Request<FxExecuteQuoteRequest> ExecuteQuoteRequest
        {
            get
            {
                return CreateExecuteQuoteRequest();
            }
        }

        public static Request<FxExecuteQuoteRequest> CreateExecuteQuoteRequest(string quoteId = "0000-0000-0000-0000")
        {
            var model = new Request<FxExecuteQuoteRequest>
            {
                Payload = new FxExecuteQuoteRequest
                {
                    QuoteId = quoteId,
                }
            };

            return model;
        }

        // track order
        public static Request<TrackOrder> TrackOrderRequest
        {
            get
            {
                return CreateTrackOrderRequest();
            }
        }

        public static Request<TrackOrder> CreateTrackOrderRequest(string trackingNumber = "TestNumber1")
        {
            var model = new Request<TrackOrder>
            {
                Payload = new TrackOrder
                {
                    TrackingNumber = trackingNumber,
                }
            };

            return model;
        }
    }
}
