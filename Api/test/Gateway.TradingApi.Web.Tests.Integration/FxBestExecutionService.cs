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
    public class FxBestExecutionService : ApiWebClientBase
    {
        //public FxBestExecutionService() : base(VsUrl) { }

        [Fact]
        public async Task Ping()
        {
            var response = await Client.Ping3Async();
            Assert.Equal("fx best execution service", response);
        }

        private async Task<Response<FxBestExecutionResponse>> TrackOrder(Request<TrackOrder> request)
        {
            Log($"tracking order: {request.Payload.TrackingNumber}");
            var cfgRequest = Configure(request);
            await Client.TrackAsync(cfgRequest);
            var response = this.ExpectValidResponse<FxBestExecutionResponse>();
            Assert.Equal(request.Payload.TrackingNumber, response.Payload.TrackingNumber);
            return response;
        }

        private StringBuilder ValidateOrderState(Response<FxBestExecutionResponse> response, string state)
        {
            var ret = new StringBuilder();
            if (!string.Equals(state, response.Payload.State, StringComparison.CurrentCultureIgnoreCase))
            {
                ret.Append($"(ValidateOrderState: {state} != {response.Payload.State}");
            }
            return ret;
        }

        private StringBuilder ValidateOrderTransaction(Response<FxBestExecutionResponse> response, string transaction)
        {
            var ret = new StringBuilder();

            if (!string.Equals(transaction, response.Payload.Side.ToString(), StringComparison.CurrentCultureIgnoreCase))
            {
                ret.Append($"(ValidateOrderTransaction: {transaction} != {response.Payload.Side.ToString()}");
            }

            if (ret.Length > 0)
            {
                Log(ret.ToString());
            }

            return ret;
        }

        // helpers END

        [Fact]
        public async Task BuyOrder()
        {
            var model = Configure(OrdersExamples.BuyOrderCcyBestExRequest);
            var comment = "slice and dice, throw it into a preheated wok for 3 minutes, serve.";
            model.Payload.Comment = comment;
            await Client.CreateAsync(model);
            var order = this.ExpectValidResponse<FxBestExecutionResponse>();
            var sideError = ValidateOrderTransaction(order, "Buy");
            Assert.False(sideError.Length > 0, sideError.ToString());
            var stateError = ValidateOrderState(order, "Processing");
            Assert.False(stateError.Length > 0, stateError.ToString());
            Assert.Equal(comment, order.Payload.Comment);
            Assert.Equal(model.Payload.Quantity, order.Payload.BaseQuantity);
            Assert.Equal(model.Payload.Side, order.Payload.Side);
            Assert.NotEqual(0, order.Payload.CommissionPercentage);
        }

        [Fact]
        public async Task BuyOrderBadPair()
        {
            var model = Configure(OrdersExamples.BuyOrderCcyBestExRequest);
            model.Payload.Pair = CurrencyPair.Parse("oogle-boogle");
            await Client.CreateAsync(model);
            this.ExpectResponseHasOnlyPayloadErrors<FxBestExecutionResponse>();
        }

        [Fact]
        public async Task BuyOrderTrack()
        {
            var model = Configure(OrdersExamples.BuyOrderCcyBestExRequest);
            var comment = "slice and dice, drop into boiling water for 5 minutes, drain and serve.";
            model.Payload.Comment = comment;
            await Client.CreateAsync(model);
            var order = this.ExpectValidResponse<FxBestExecutionResponse>();
            var sideError = ValidateOrderTransaction(order, "Buy");
            Assert.False(sideError.Length > 0, sideError.ToString());
            var stateError = ValidateOrderState(order, "Processing");
            Assert.False(stateError.Length > 0, stateError.ToString());
            var trackedOrder = await TrackOrder(OrdersExamples.CreateTrackOrderRequest(order.Payload.TrackingNumber));
            Assert.Equal(comment, trackedOrder.Payload.Comment);
            Assert.Equal(model.Payload.Quantity, trackedOrder.Payload.BaseQuantity);
            Assert.Equal(model.Payload.Side, trackedOrder.Payload.Side);
            Assert.NotEqual(0, trackedOrder.Payload.CommissionPercentage);
        }

        [Fact]
        public async Task SellOrder()
        {
            var model = Configure(OrdersExamples.SellOrderCcyBestExRequest);
            var comment = "slice, spice, serve.";
            model.Payload.Comment = comment;
            await Client.CreateAsync(model);
            var order = this.ExpectValidResponse<FxBestExecutionResponse>();
            var sideError = ValidateOrderTransaction(order, "Sell");
            Assert.False(sideError.Length > 0, sideError.ToString());
            var stateError = ValidateOrderState(order, "Processing");
            Assert.False(stateError.Length > 0, stateError.ToString());
            Assert.Equal(comment, order.Payload.Comment);
            Assert.Equal(model.Payload.Quantity, order.Payload.BaseQuantity);
            Assert.Equal(model.Payload.Side, order.Payload.Side);
            Assert.NotEqual(0, order.Payload.CommissionPercentage);
        }

        [Fact]
        public async Task SellOrderBadPair()
        {
            var model = Configure(OrdersExamples.SellOrderCcyBestExRequest);
            model.Payload.Pair = CurrencyPair.Parse("oogle-boogle");
            await Client.CreateAsync(model);
            this.ExpectResponseHasOnlyPayloadErrors<FxBestExecutionResponse>();
        }

        [Fact]
        public async Task SellOrderTrack()
        {
            var model = Configure(OrdersExamples.SellOrderCcyBestExRequest);
            var comment = "slice, serve.";
            model.Payload.Comment = comment;
            await Client.CreateAsync(model);
            var order = this.ExpectValidResponse<FxBestExecutionResponse>();
            var sideError = ValidateOrderTransaction(order, "Sell");
            Assert.False(sideError.Length > 0, sideError.ToString());
            var stateError = ValidateOrderState(order, "Processing");
            Assert.False(stateError.Length > 0, stateError.ToString());
            var trackedOrder = await TrackOrder(OrdersExamples.CreateTrackOrderRequest(order.Payload.TrackingNumber));
            Assert.Equal(comment, trackedOrder.Payload.Comment);
            Assert.Equal(model.Payload.Quantity, trackedOrder.Payload.BaseQuantity);
            Assert.Equal(model.Payload.Side, trackedOrder.Payload.Side);
            Assert.NotEqual(0, trackedOrder.Payload.CommissionPercentage);
        }

        [Fact]
        public async Task BuyOrderNullPayload()
        {
            var model = Configure(OrdersExamples.BuyOrderCcyBestExRequest);
            model.Payload = null;
            await Client.CreateAsync(model);
            this.ExpectInvalidResponse<FxBestExecutionResponse>();
        }

        [Fact]
        public async Task SellOrderNullPayload()
        {
            var model = Configure(OrdersExamples.SellOrderCcyBestExRequest);
            model.Payload = null;
            await Client.CreateAsync(model);
            this.ExpectInvalidResponse<FxBestExecutionResponse>();
        }

        [Fact]
        public async Task BuyOrderMissingAmount()
        {
            var model = Configure(OrdersExamples.BuyOrderCcyBestExRequest);
            model.Payload.Quantity = 0;
            await Client.CreateAsync(model);
            this.ExpectInvalidResponse<FxBestExecutionResponse>();
        }

        [Fact]
        public async Task SellOrderMissingAmount()
        {
            var model = Configure(OrdersExamples.SellOrderCcyBestExRequest);
            model.Payload.Quantity = 0;
            await Client.CreateAsync(model);
            this.ExpectResponseHasOnlyPayloadErrors<FxBestExecutionResponse>();
        }

        [Fact]
        public async Task TrackNonExistentOrder()
        {
            var request = Configure(OrdersExamples.CreateTrackOrderRequest("Ronald-McDonald"));
            await Client.TrackAsync(request);
            this.ExpectInvalidResponse<FxBestExecutionResponse>();
        }

        [Fact]
        public async Task BuyOrderNotEnoughFunds_BTC_USD()
        {
            var model = Configure(OrdersExamples.BuyOrderCcyBestExRequest);
            model.Payload.Pair = CurrencyPair.Parse("BTC-USD");
            model.Payload.Quantity = 5000000;
            await Client.CreateAsync(model);
            this.ExpectResponseHasOnlyPayloadErrors<FxBestExecutionResponse>();
        }

        [Fact]
        public async Task SellOrderNotEnoughFunds_BTC_USD()
        {
            var model = Configure(OrdersExamples.SellOrderCcyBestExRequest);
            model.Payload.Pair = CurrencyPair.Parse("BTC-USD");
            model.Payload.Quantity = 5000000;
            await Client.CreateAsync(model);
            this.ExpectResponseHasOnlyPayloadErrors<FxBestExecutionResponse>();
        }

        [Fact]
        public async Task BuyOrderNotEnoughFunds_USD_BTC()
        {
            var model = Configure(OrdersExamples.BuyOrderCcyBestExRequest);
            model.Payload.Pair = CurrencyPair.Parse("USD-BTC");
            model.Payload.Quantity = 5000000;
            await Client.CreateAsync(model);
            this.ExpectResponseHasOnlyPayloadErrors<FxBestExecutionResponse>();
        }

        [Fact]
        public async Task SellOrderNotEnoughFunds_USD_BTC()
        {
            var model = Configure(OrdersExamples.SellOrderCcyBestExRequest);
            model.Payload.Pair = CurrencyPair.Parse("USD-BTC");
            model.Payload.Quantity = 5000000;
            await Client.CreateAsync(model);
            this.ExpectResponseHasOnlyPayloadErrors<FxBestExecutionResponse>();
        }
    }
}
