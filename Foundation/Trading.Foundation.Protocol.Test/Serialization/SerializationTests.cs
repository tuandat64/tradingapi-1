using Newtonsoft.Json;
using System;
using System.Linq;
using Trading.Foundation.Dtos;
using Trading.Foundation.Protocol.Extensions;
using Xunit;

namespace Trading.Foundation.Protocol.Test.Serialization
{
    public class SerializationTests
    {
        [Fact]
        public void ShouldSerializeSingleError()
        {
            var r = new Request<NullPayload>();
            r.AddError("the cat sat on the mat");

            var json = r.ToString();

            var o = JsonConvert.DeserializeObject<Request<NullPayload>>(json);

            Assert.Equal(r.GetErrors().Length, o.GetErrors().Length);
            Assert.True(r.GetErrors().SequenceEqual(o.GetErrors()));
        }

        [Fact]
        public void ShouldSerializeMultipleErrors()
        {
            var r = new Response<string>();
            r.AddError("1");
            r.AddError("2");

            var json = JsonConvert.SerializeObject(r);
            var o = JsonConvert.DeserializeObject<Response<string>>(json);
            Assert.Equal(r.GetErrors().Length, o.GetErrors().Length);
            Assert.True(r.GetErrors().SequenceEqual(o.GetErrors()));
        }

        [Fact]
        public void ShouldSerializeCompositePair()
        {
            var pairBefore = CurrencyPair.Parse("BTC-USD");
            var b = pairBefore.Base;

            var json = JsonConvert.SerializeObject(pairBefore);
            var pairAfter = JsonConvert.DeserializeObject<CurrencyPair>(json);

            Assert.Equal(pairBefore.Base, pairAfter.Base);
            Assert.Equal(pairBefore.Quote, pairAfter.Quote);
        }

        [Fact]
        public void ShouldSerializeCustomProperty()
        {
            var r = new Response<string>();
            r.AddValue("custom", 10);

            var json = JsonConvert.SerializeObject(r);
            var o = JsonConvert.DeserializeObject<Response<string>>(json);
            Assert.Equal(r.GetValue<string, int>("custom"), o.GetValue<string, int>("custom"));
        }

        [Fact]
        public void ShouldSerializeOrderRequest()
        {
            var correlationId = Guid.NewGuid();
            var customerId = Guid.NewGuid();
            var gatewayId = Guid.NewGuid();
            var businessTransactionId = Guid.NewGuid();
            var randomId = Guid.NewGuid();

            var rb = new RequestBuilder<FxBestExecutionRequest>();

            var rq1 = rb.AddPayload(new FxBestExecutionRequest
                {
                    Side = Side.Buy,
                    Pair = CurrencyPair.Parse("BTC-USD"),
                    Quantity = 10.41m,
                    Comment = "foo"
                })
                .AddCorrelationId(correlationId)
                .AddCustomerId(customerId)
                .AddGatewayId(gatewayId)
                .AddBusinessTransactionId(businessTransactionId)
                .AddValue("life_the_universe_and_everything", 42)
                .AddValue("random", randomId)
                .ToEnvelope();

            var json = JsonConvert.SerializeObject(rq1);
            var rq2 = JsonConvert.DeserializeObject<Request<FxBestExecutionRequest>>(json);

            Assert.Equal(Side.Buy, rq2.Payload.Side);
            Assert.Equal(CurrencyPair.Parse("BTC-USD"), rq2.Payload.Pair);
            Assert.Equal(correlationId, rq2.GetCorrelationId());
            Assert.Equal(customerId, rq2.GetCustomerId());
            Assert.Equal(gatewayId, rq2.GetGatewayId());
            Assert.Equal(businessTransactionId, rq2.GetBusinessTransactionId());
            Assert.Equal(42, rq2.GetValue<FxBestExecutionRequest, int>("life_the_universe_and_everything"));
            Assert.Equal(randomId, rq2.GetValue<FxBestExecutionRequest, Guid>("random"));
        }

        [Fact]
        public void CheckToStringFormatting()
        {
            var r1 = new FxBestExecutionRequest
            {
                Side = Side.Buy,
                Pair = CurrencyPair.Parse("USD-BTC"),
                Quantity = 100.0M,
                Comment = "this is a comment"
            };

            Assert.Equal("Side: buy, Pair: USD-BTC, Quantity: 100.0, Comment: this is a comment", r1.ToString());

            var s1 = r1.ToString();

            var r2 = new FxQuoteRequest
            {
                Side = Side.Buy,
                Pair = CurrencyPair.Parse("USD-BTC"),
                Quantity = 100.0M,
            };

            Assert.Equal("Side: buy, Pair: USD-BTC, Quantity: 100.0", r2.ToString());

            var r3 = new FxBestExecutionResponse
            {
                State = "Processing",
                Side = Side.Buy,
                Pair = CurrencyPair.Parse("USD-BTC"),
                TrackingNumber = "QWERTY01",
                BaseQuantity = 100.0M,
                QuoteQuantity = 0.01M,
                CommissionPercentage = 1.75M,
                Description = "Sell BTC for X",
                Comment = "this is a comment"
            };

            var expected = "State: Processing, Side: buy, Pair: USD-BTC, TrackingNumber: QWERTY01, BaseQuantity: 100.0, QuoteQuantity: 0.01, " +
                "CommissionPercentage: 1.75, Description: Sell BTC for X, Comment: this is a comment";

            Assert.Equal(expected, r3.ToString());

            var r4 = new FxExecuteQuoteResponse
            {
                State = "Processing",
                Side = Side.Buy,
                Pair = CurrencyPair.Parse("USD-BTC"),
                TrackingNumber = "QWERTY01",
                BaseQuantity = 100.0M,
                QuoteQuantity = 0.01M,
                CommissionPercentage = 1.75M,
                Description = "Sell BTC for X",
            };

            expected = "State: Processing, Side: buy, Pair: USD-BTC, TrackingNumber: QWERTY01, BaseQuantity: 100.0, QuoteQuantity: 0.01, " +
                "CommissionPercentage: 1.75, Description: Sell BTC for X";

            Assert.Equal(expected, r4.ToString());

            var r5 = new FxQuoteResponse
            {
                QuoteId = Guid.Parse("E8F598C2-74C9-432F-992C-5FE7B84F37E0").ToString().ToUpper(),
                Side = Side.Buy,
                Pair = CurrencyPair.Parse("USD-BTC"),
                BaseQuantity = 100.0M,
                QuoteQuantity = 0.01M,
                CommissionPercentage = 1.75M,
                ExpiryTimestamp = 123456789,
            };

            expected = "QuoteId: E8F598C2-74C9-432F-992C-5FE7B84F37E0, Side: buy, Pair: USD-BTC, BaseQuantity: 100.0, QuoteQuantity: 0.01, " +
                "CommissionPercentage: 1.75, ExpiryTimestamp: 123456789";

            Assert.Equal(expected, r5.ToString());
        }

        [Fact]
        public void CheckDefaultErrorResponseSerialisation()
        {
            // Decorating the attributes with JsonRequired causes default objects to go bang
            // Add return DTOs here

            var json1 = JsonConvert.SerializeObject(new FxBestExecutionResponse());
            var pairAfter1 = JsonConvert.DeserializeObject<FxBestExecutionResponse>(json1);

            var json2 = JsonConvert.SerializeObject(new FxExecuteQuoteResponse());
            var pairAfter2 = JsonConvert.DeserializeObject<FxExecuteQuoteResponse>(json2);

            var json3 = JsonConvert.SerializeObject(new FxQuoteResponse());
            var pairAfter3 = JsonConvert.DeserializeObject<FxQuoteResponse>(json3);

            var json4 = JsonConvert.SerializeObject(new TradeableAccount());
            var pairAfter4 = JsonConvert.DeserializeObject<TradeableAccount>(json4);

            var json5 = JsonConvert.SerializeObject(new TradeableAccountsSummary());
            var pairAfter5 = JsonConvert.DeserializeObject<TradeableAccountsSummary>(json5);
        }
    }
}
