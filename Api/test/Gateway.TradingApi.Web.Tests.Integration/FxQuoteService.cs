using System;
using Gateway.Middleware;
using System.Text;
using System.Threading.Tasks;
using Trading.Foundation.Dtos;
using Trading.Foundation.Protocol;
using TradingApi.Web.Client;
using Xunit;

namespace Gateway.TradingApi.Web.Tests.Integration
{
    public class FxQuoteService : ApiWebClientBase
    {
        //public FxQuoteService() : base(VsUrl) {}

        [Fact]
        public async Task Ping()
        {
            var response = await Client.Ping4Async();
            Assert.Equal("fx quote service", response);
        }

        private async Task<Response<FxExecuteQuoteResponse>> TrackOrder(Request<TrackOrder> request)
        {
            Log($"tracking order: {request.Payload.TrackingNumber}");
            var cfgRequest = Configure(request);
            await Client.TrackAsync(cfgRequest);
            var response = this.ExpectValidResponse<FxExecuteQuoteResponse>();
            Assert.Equal(request.Payload.TrackingNumber, response.Payload.TrackingNumber);
            return response;
        }

        // helpers END

        private StringBuilder ValidateOrderTransaction(Response<FxBestExecutionResponse> response, string transaction)
        {
            var ret = new StringBuilder();

            if (transaction.ToLower() != response.Payload.Side.ToString().ToLower())
            {
                ret.Append($"(ValidateOrderTransaction: {transaction}!={response.Payload.Side.ToString()}");
            }

            if (ret.Length > 0)
            {
                Log(ret.ToString());
            }

            return ret;
        }

        private async Task<(FxExecuteQuoteResponse, StringBuilder)> ExecuteQuoteUsingQuoteIdFromQuote(IResponse<FxQuoteResponse> response, int action = 0)
        {
            var errors = new StringBuilder();
            var eqReq = OrdersExamples.CreateExecuteQuoteRequest(response.Payload.QuoteId);
            var tuple = await ExecuteQuote(eqReq, action);
            errors.Append(tuple.Item2);

            return (tuple.Item1, errors);
        }

        private async Task<(FxExecuteQuoteResponse, StringBuilder)> ExecuteQuote(Request<FxExecuteQuoteRequest> request, int action = 0)
        {
            Configure(request);
            await Client.ExecuteAsync(request);
            var response = this.ExpectValidResponse<FxExecuteQuoteResponse>();
            return (response.Payload, ValidateOrderState(response, "Filled"));
        }

        private StringBuilder ValidateOrderState(IResponse<FxExecuteQuoteResponse> response, string state)
        {
            var ret = new StringBuilder();
            if (state.ToLower() != response.Payload.State.ToLower())
            {
                ret.Append($"(ValidateOrderState: {state}!={response.Payload.State}");
            }
            return ret;
        }

        // helpers END
               
        public StringBuilder ValidateQuoteTransaction(IResponse<FxQuoteResponse> response, string transaction)
        {
            var ret = new StringBuilder();

            if (!String.Equals(transaction, response.Payload.Side.ToString(), StringComparison.CurrentCultureIgnoreCase))
            {
                ret.Append($"(ValidateQuoteTransaction: {transaction} != {response.Payload.Side.ToString()}");
            }

            if (ret.Length > 0)
            {
                Log(ret.ToString());
            }

            return ret;
        }

        public StringBuilder ValidateQuoteExecutionTransaction(FxExecuteQuoteResponse response, string transaction)
        {
            var ret = new StringBuilder();

            if (transaction.ToLower() != response.Side.ToString().ToLower())
            {
                ret.Append($"(ValidateQuoteTransaction: {transaction} != {response.Side.ToString()}");
            }

            if (ret.Length > 0)
            {
                Log(ret.ToString());
            }

            return ret;
        }

        [Fact]
        public async Task BuyOrder()
        {
            var model = Configure(OrdersExamples.BuyOrderCcyRfqRequest);
            await Client.Create2Async(model);
            var quote = this.ExpectValidResponse<FxQuoteResponse>();
            var errors = ValidateQuoteTransaction(quote, "Buy");
            Assert.False(errors.Length > 0, errors.ToString());

            Assert.Equal(model.Payload.Quantity, quote.Payload.BaseQuantity);
            Assert.Equal(model.Payload.Side, quote.Payload.Side);

            Assert.NotEqual(0, quote.Payload.CommissionPercentage);
        }

        [Fact]
        public async Task BuyOrderBadPair()
        {
            var model = Configure(OrdersExamples.BuyOrderCcyRfqRequest);
            model.Payload.Pair = CurrencyPair.Parse("oogle-boogle");
            await Client.Create2Async(model);
            this.ExpectResponseHasOnlyPayloadErrors<FxQuoteResponse>();
        }

        [Fact]
        public async Task BuyOrderExecute()
        {
            //var comment = "slice and dice, throw it into a preheated wok for 3 minutes, serve.";
            var model = Configure(OrdersExamples.BuyOrderCcyRfqRequest);
            //model.Payload.Comment = comment;
            await Client.Create2Async(model);
            var quote = this.ExpectValidResponse<FxQuoteResponse>();
            var errors = ValidateQuoteTransaction(quote, "Buy");
            Assert.False(errors.Length > 0, errors.ToString());

            Assert.Equal(model.Payload.Quantity, quote.Payload.BaseQuantity);
            Assert.Equal(model.Payload.Side, quote.Payload.Side);

            var tuple = await ExecuteQuoteUsingQuoteIdFromQuote(quote);
            Assert.False(tuple.Item2.Length > 0, tuple.Item2.ToString());
            Assert.Equal(model.Payload.Quantity, tuple.Item1.BaseQuantity);
            errors = ValidateQuoteExecutionTransaction(tuple.Item1, "Buy");
            Assert.False(errors.Length > 0, errors.ToString());
            Assert.Equal(model.Payload.Side, tuple.Item1.Side);
            Assert.NotEqual(0, tuple.Item1.CommissionPercentage);
        }

        [Fact]
        public async Task SellOrder()
        {
            var model = Configure(OrdersExamples.SellOrderCcyRfqRequest);
            await Client.Create2Async(model);
            var quote = this.ExpectValidResponse<FxQuoteResponse>();
            var errors = ValidateQuoteTransaction(quote, "Sell");
            Assert.False(errors.Length > 0, errors.ToString());

            Assert.Equal(model.Payload.Quantity, quote.Payload.BaseQuantity);
            Assert.Equal(model.Payload.Side, quote.Payload.Side);

            Assert.NotEqual(0, quote.Payload.CommissionPercentage);
        }

        [Fact]
        public async Task SellOrderBadPair()
        {
            var model = Configure(OrdersExamples.SellOrderCcyRfqRequest);
            model.Payload.Pair = CurrencyPair.Parse("oogle-boogle");
            await Client.Create2Async(model);
            this.ExpectResponseHasOnlyPayloadErrors<FxQuoteResponse>();
        }

        [Fact]
        public async Task SellOrderExececute()
        {
            var model = Configure(OrdersExamples.SellOrderCcyRfqRequest);
            await Client.Create2Async(model);
            var quote = this.ExpectValidResponse<FxQuoteResponse>();
            var errors = ValidateQuoteTransaction(quote, "Sell");
            Assert.False(errors.Length > 0);

            Assert.Equal(model.Payload.Quantity, quote.Payload.BaseQuantity);
            Assert.Equal(model.Payload.Side, quote.Payload.Side);

            var tuple = await ExecuteQuoteUsingQuoteIdFromQuote(quote);
            Assert.False(tuple.Item2.Length > 0, tuple.Item2.ToString());
            Assert.Equal(model.Payload.Quantity, tuple.Item1.BaseQuantity);
            errors = ValidateQuoteExecutionTransaction(tuple.Item1, "Sell");
            Assert.False(errors.Length > 0, errors.ToString());
            Assert.Equal(model.Payload.Side, tuple.Item1.Side);
            Assert.NotEqual(0, tuple.Item1.CommissionPercentage);
        }

        [Fact]
        public async Task SellOrderExecuteTrack()
        { 
            var model = Configure(OrdersExamples.SellOrderCcyRfqRequest);

            await Client.Create2Async(model);
            var quote = this.ExpectValidResponse<FxQuoteResponse>();
            var errors = ValidateQuoteTransaction(quote, "Sell");
            Assert.False(errors.Length > 0);

            Assert.Equal(model.Payload.Quantity, quote.Payload.BaseQuantity);
            Assert.Equal(model.Payload.Side, quote.Payload.Side);

            var tuple = await ExecuteQuoteUsingQuoteIdFromQuote(quote);
            Assert.False(tuple.Item2.Length > 0, tuple.Item2.ToString());
            Assert.Equal(model.Payload.Side, tuple.Item1.Side);

            Assert.Equal(model.Payload.Quantity, tuple.Item1.BaseQuantity);
            errors = ValidateQuoteExecutionTransaction(tuple.Item1, "Sell");
            Assert.False(errors.Length > 0, errors.ToString());

            var trackedOrder = await TrackOrder(OrdersExamples.CreateTrackOrderRequest(tuple.Item1.TrackingNumber));
            Assert.Equal(model.Payload.Quantity, trackedOrder.Payload.BaseQuantity);
            errors = ValidateQuoteExecutionTransaction(trackedOrder.Payload, "Sell");
            Assert.False(errors.Length > 0, errors.ToString());
            Assert.NotEqual(0, trackedOrder.Payload.CommissionPercentage);
        }

        [Fact]
        public async Task BuyOrderExecuteTrack()
        {

            var model = Configure(OrdersExamples.BuyOrderCcyRfqRequest);

            await Client.Create2Async(model);
            var quote = this.ExpectValidResponse<FxQuoteResponse>();
            var errors = ValidateQuoteTransaction(quote, "Buy");
            Assert.False(errors.Length > 0);

            Assert.Equal(model.Payload.Quantity, quote.Payload.BaseQuantity);
            Assert.Equal(model.Payload.Side, quote.Payload.Side);

            var tuple = await ExecuteQuoteUsingQuoteIdFromQuote(quote);
            Assert.False(tuple.Item2.Length > 0, tuple.Item2.ToString());
            Assert.Equal(model.Payload.Side, tuple.Item1.Side);

            Assert.Equal(model.Payload.Quantity, tuple.Item1.BaseQuantity);
            errors = ValidateQuoteExecutionTransaction(tuple.Item1, "Buy");
            Assert.False(errors.Length > 0, errors.ToString());

            var trackedOrder = await TrackOrder(OrdersExamples.CreateTrackOrderRequest(tuple.Item1.TrackingNumber));
            Assert.Equal(model.Payload.Quantity, trackedOrder.Payload.BaseQuantity);
            errors = ValidateQuoteExecutionTransaction(trackedOrder.Payload, "Buy");
            Assert.False(errors.Length > 0, errors.ToString());
            Assert.NotEqual(0, trackedOrder.Payload.CommissionPercentage);
        }

        [Fact]
        public async Task BuyOrderNullPayload()
        {
            var model = Configure(OrdersExamples.BuyOrderCcyRfqRequest);
            model.Payload = null;
            await Client.Create2Async(model);
            this.ExpectInvalidResponse<FxQuoteResponse>();
        }

        [Fact]
        public async Task SellOrderNullPayload()
        {
            var model = Configure(OrdersExamples.SellOrderCcyRfqRequest);
            model.Payload = null;
            await Client.Create2Async(model);
            this.ExpectInvalidResponse<FxQuoteResponse>();
        }

        [Fact]
        public async Task BuyOrderMissingAmount()
        {
            var model = Configure(OrdersExamples.BuyOrderCcyRfqRequest);
            model.Payload.Quantity = 0;
            await Client.Create2Async(model);
            this.ExpectResponseHasOnlyPayloadErrors<FxExecuteQuoteResponse>();
        }

        [Fact]
        public async Task SellOrderMissingAmount()
        {
            var model = Configure(OrdersExamples.SellOrderCcyRfqRequest);
            model.Payload.Quantity = 0;
            await Client.Create2Async(model);
            this.ExpectResponseHasOnlyPayloadErrors<FxExecuteQuoteResponse>();
        }

        [Fact]
        public async Task ExecuteQuoteFail()
        {
            var result = await Task.Factory.StartNew(() => ExecuteQuote(OrdersExamples.CreateExecuteQuoteRequest(), -1));
        }

        [Fact]
        public async Task TrackNonExistentOrder()
        {
            var request = Configure(OrdersExamples.CreateTrackOrderRequest("Ronald-McDonald"));
            await Client.TrackAsync(request);
            this.ExpectInvalidResponse<FxQuoteResponse>();
        }

        [Fact]
        public async Task BuyOrderNotEnoughFunds_BTC_USD()
        {
            var model = Configure(OrdersExamples.BuyOrderCcyRfqRequest);
            model.Payload.Pair = CurrencyPair.Parse("BTC-USD");
            model.Payload.Quantity = 5000000;
            await Client.Create2Async(model);
            this.ExpectResponseHasOnlyPayloadErrors<FxBestExecutionResponse>();
        }

        [Fact]
        public async Task SellOrderNotEnoughFunds_BTC_USD()
        {
            var model = Configure(OrdersExamples.SellOrderCcyRfqRequest);
            model.Payload.Pair = CurrencyPair.Parse("BTC-USD");
            model.Payload.Quantity = 5000000;
            await Client.Create2Async(model);
            this.ExpectResponseHasOnlyPayloadErrors<FxBestExecutionResponse>();
        }

        [Fact]
        public async Task BuyOrderNotEnoughFunds_USD_BTC()
        {
            var model = Configure(OrdersExamples.BuyOrderCcyRfqRequest);
            model.Payload.Pair = CurrencyPair.Parse("USD-BTC");
            model.Payload.Quantity = 5000000;
            await Client.Create2Async(model);
            this.ExpectResponseHasOnlyPayloadErrors<FxBestExecutionResponse>();
        }

        [Fact]
        public async Task SellOrderNotEnoughFunds_USD_BTC()
        {
            var model = Configure(OrdersExamples.SellOrderCcyRfqRequest);
            model.Payload.Pair = CurrencyPair.Parse("USD-BTC");
            model.Payload.Quantity = 5000000;
            await Client.Create2Async(model);
            this.ExpectResponseHasOnlyPayloadErrors<FxBestExecutionResponse>();
        }
    }
}
