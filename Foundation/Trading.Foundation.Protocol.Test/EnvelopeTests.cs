using System;
using System.Collections.Generic;
using Trading.Foundation.Dtos;
using Trading.Foundation.Protocol.Extensions;
using Xunit;

namespace Trading.Foundation.Protocol.Test
{
    public class EnvelopeTests
    {
        [Fact]
        public void ShouldThrowWhenCurrencyPairBaseLengthIsZero()
        {
            Assert.Throws<InvalidCurrencyPairException>(() => CurrencyPair.Parse("-USD"));
        }

        [Fact]
        public void ShouldThrowWhenCurrencyPairQuoteLengthIsZero()
        {
            Assert.Throws<InvalidCurrencyPairException>(() => CurrencyPair.Parse("BTC-"));
        }

        [Fact]
        public void ShouldThrowWhenCurrencyPairDelimeterIsInvalid()
        {
            Assert.Throws<InvalidCurrencyPairException>(() => CurrencyPair.Parse("BTC&USD"));
        }

        [Fact]
        public void ShouldParseCorrectlyFormedCurrencyPair()
        {
            var pair = CurrencyPair.Parse("BTC-USD");

            Assert.Equal("BTC", pair.Base);
            Assert.Equal("USD", pair.Quote);
        }

        [Fact]
        public void ShouldParseCorrectlyFormedCurrencyPairWithForwardSlashSeparator()
        {
            var pair = CurrencyPair.Parse("BTC/USD");

            Assert.Equal("BTC", pair.Base);
            Assert.Equal("USD", pair.Quote);
        }

        [Fact]
        public void TestValidation()
        {
            var r = new Response<string>();
            r.AddError("1");
            r.AddError("2");

            Assert.True(r.ValidateEnvelope().HasError);

            int n = r.GetErrors().Length;

            r.ValidateEnvelope();

            Assert.Equal(n, r.GetErrors().Length);

            Assert.Throws<ArgumentNullException>(() => ((Response<string>) null).ValidateEnvelope().HasError);
        }

        [Fact]
        public void TestShouldNotBeValidWithoutCustomerId()
        {
            var r = new RequestBuilder<string>().AddPayload("payload").AddGatewayId(Guid.NewGuid()).AddBusinessTransactionId(Guid.NewGuid()).ToEnvelope();
            r.AddCorrelationId(Guid.NewGuid());

            Assert.True(r.ValidateEnvelope().HasError);

            Assert.Single(r.GetErrors());
        }

        [Fact]
        public void TestShouldNotBeValidWithoutGatewayId()
        {
            var r = new RequestBuilder<string>().AddPayload("payload").AddCustomerId(Guid.NewGuid()).AddBusinessTransactionId(Guid.NewGuid()).ToEnvelope();
            
            r.AddCorrelationId(Guid.NewGuid());

            Assert.True(r.ValidateEnvelope().HasError);

            Assert.Single(r.GetErrors());
        }

        [Fact]
        public void TestShouldNotBeValidWithoutBusinessTransactionId()
        {
            var r = new RequestBuilder<string>().AddPayload("payload").AddCustomerId(Guid.NewGuid()).AddGatewayId(Guid.NewGuid()).ToEnvelope();

            r.AddCorrelationId(Guid.NewGuid());

            Assert.True(r.ValidateEnvelope().HasError);

            Assert.Single(r.GetErrors());
        }

        [Fact]
        public void TestCorrelationIdInvariance()
        {
            var r = new Request<string>();
            var n = Guid.NewGuid();
            r.AddCorrelationId(n);
            r.AddCorrelationId(Guid.NewGuid());
            Assert.Equal(n, r.GetCorrelationId());
        }

        [Fact]
        public void TestShouldStoreAndRetrieveCustomPropertyInRequest()
        {
            var r = new Request<string>();
            r.AddValue("custom", 10);

            var storedValue = r.GetValue<string, int>("custom");
            Assert.Equal(10, storedValue);
        }

        [Fact]
        public void TestShouldThrowWhenCustomPropertyKeyIsNull()
        {
            var r = new Request<string>();

            Assert.Throws<ArgumentNullException>(() => r.AddValue(null, 10));
        }

        [Fact]
        public void TestShouldThrowWhenCustomPropertyValueIsNull()
        {
            var r = new Request<string>();

            Assert.Throws<ArgumentNullException>(() => r.AddValue("foo", (object) null));
        }

        [Fact]
        public void TestShouldCustomPropertyBeVariant()
        {
            var r = new Request<string>();
            r.AddValue("custom", 10);
            r.AddValue("custom", 20);

            var storedValue = r.GetValue<string, int>("custom");

            Assert.Equal(20, storedValue);
        }

        public class TestFixture
        {
            public int Age;
        }

        public class TestFixture2
        {
            public string AgeAsString;
        }

        [Fact]
        public void ShouldNotGoExplodeyBoom()
        {
            var r = new Request<string>();
            r.AddValue("custom", 10);
            r.AddError("a bad thing just happened");

            if (r.ValidateEnvelope().HasError)
            {
                var response = r.CreateErrorResponse("oops", new TestFixture());
            }
        }

        [Fact]
        public void TestShouldCreateResponseClone()
        {
            var correlationId = Guid.NewGuid();
            var customerId = Guid.NewGuid();
            var gatewayId = Guid.NewGuid();
            var businessTransactionId = Guid.NewGuid();

            var r = new Request<string>();
            r.AddValue("custom", 10);
            r.AddError("a bad thing just happened");
            r.AddCorrelationId(correlationId);
            r.AddCustomerId(customerId);
            r.AddGatewayId(gatewayId);
            r.AddBusinessTransactionId(businessTransactionId);

            var response = r.CreateResponse(new TestFixture {Age = 10});

            Assert.Empty(response.GetErrors());
            Assert.Equal(correlationId, response.GetCorrelationId());
            Assert.NotEqual(Guid.Empty, response.GetCustomerId());
            Assert.Equal(gatewayId, response.GetGatewayId());
            Assert.Equal(businessTransactionId, response.GetBusinessTransactionId());
        }

        [Fact]
        public void TestShouldCreateErrorResponseClone()
        {
            var correlationId = Guid.NewGuid();
            var customerId = Guid.NewGuid();
            var gatewayId = Guid.NewGuid();
            var businessTransactionId = Guid.NewGuid();

            var r = new Request<FxBestExecutionRequest>();
            r.AddValue("custom", 10);
            r.AddError("a bad thing just happened");
            r.AddCorrelationId(correlationId);
            r.AddCustomerId(customerId);
            r.AddGatewayId(gatewayId);
            r.AddBusinessTransactionId(businessTransactionId);

            var response = r.CreateErrorResponse<FxBestExecutionRequest, FxBestExecutionResponse>("its life Jim but not as we know it");

            Assert.Equal("a bad thing just happened", response.GetErrors()[0].Message);
            Assert.Equal("its life Jim but not as we know it", response.GetErrors()[1].Message);
            Assert.Equal(correlationId, response.GetCorrelationId());
            Assert.NotEqual(Guid.Empty, response.GetCustomerId());
            Assert.Equal(gatewayId, response.GetGatewayId());
            Assert.Equal(businessTransactionId, response.GetBusinessTransactionId());
            Assert.Equal(10, response.GetValue<FxBestExecutionResponse, int>("custom"));
        }

        [Fact]
        public void TestShouldBuildRequest()
        {
            var correlationId = Guid.NewGuid();
            var customerId = Guid.NewGuid();
            var gatewayId = Guid.NewGuid();
            var businessTransactionId = Guid.NewGuid();
            var randomId = Guid.NewGuid();

            var rb = new RequestBuilder<string>();
            var rq = rb.AddPayload("payload")
                .AddCorrelationId(correlationId)
                .AddCustomerId(customerId)
                .AddGatewayId(gatewayId)
                .AddBusinessTransactionId(businessTransactionId)
                .AddValue("life_the_universe_and_everything", 42)
                .AddValue("random", randomId)
                .ToEnvelope();

            Assert.Equal("payload", rq.Payload);
            Assert.Equal(correlationId, rq.GetCorrelationId());
            Assert.Equal(customerId, rq.GetCustomerId());
            Assert.Equal(gatewayId, rq.GetGatewayId());
            Assert.Equal(businessTransactionId, rq.GetBusinessTransactionId());
            Assert.Equal(42, rq.GetValue<string, int>("life_the_universe_and_everything"));
            Assert.Equal(randomId, rq.GetValue<string, Guid>("random"));
        }

        

        [Fact]
        public void ShouldMapRequestPayload()
        {
            var correlationId = Guid.NewGuid();
            var customerId = Guid.NewGuid();
            var gatewayId = Guid.NewGuid();
            var businessTransactionId = Guid.NewGuid();

            var r = new Request<int> {Payload = 10};
            r.AddValue("custom", 10);
            r.AddError("a bad thing just happened");

            var request = r.CreateRequest(correlationId, gatewayId, customerId, businessTransactionId);

            var mappedRequest = request.Map(v => (v * v).ToString());

            Assert.Equal("100", mappedRequest.Payload);
            Assert.Single(mappedRequest.GetErrors());
            Assert.Equal(correlationId, mappedRequest.GetCorrelationId());
            Assert.Equal(customerId, mappedRequest.GetCustomerId());
            Assert.Equal(gatewayId, mappedRequest.GetGatewayId());
            Assert.Equal(businessTransactionId, mappedRequest.GetBusinessTransactionId());
            Assert.Equal(10, mappedRequest.GetValue<string, int>("custom"));
        }

        [Fact]
        public void ShouldMapResponsePayload()
        {
            var correlationId = Guid.NewGuid();
            var customerId = Guid.NewGuid();
            var gatewayId = Guid.NewGuid();
            var businessTransactionId = Guid.NewGuid();

            var r = new Request<TestFixture> { Payload = new TestFixture{ Age = 10 } };
            r.AddValue("custom", 10);
            r.AddError("a bad thing just happened");

            var request = r.CreateRequest(correlationId, gatewayId, customerId, businessTransactionId);

            var response = request.CreateResponse(new TestFixture2 {AgeAsString = "22"});

            var mappedResponse = response.Map(v => (v.AgeAsString + v.AgeAsString).ToString());

            Assert.Equal("2222", mappedResponse.Payload);
            Assert.Empty(mappedResponse.GetErrors());
            Assert.Equal(correlationId, mappedResponse.GetCorrelationId());
            Assert.Equal(customerId, mappedResponse.GetCustomerId());
            Assert.Equal(gatewayId, mappedResponse.GetGatewayId());
            Assert.Equal(businessTransactionId, mappedResponse.GetBusinessTransactionId());
            Assert.Throws<KeyNotFoundException>(() => mappedResponse.GetValue<string, int>("custom"));
        }

        [Fact]
        public void ShouldCreateMappedResponse()
        {
            var correlationId = Guid.NewGuid();
            var customerId = Guid.NewGuid();
            var gatewayId = Guid.NewGuid();
            var businessTransactionId = Guid.NewGuid();

            var r = new Request<TestFixture> { Payload = new TestFixture { Age = 10 } };
            r.AddValue("custom", 10);
            r.AddError("a bad thing just happened");

            var request = r.CreateRequest(correlationId, gatewayId, customerId, businessTransactionId);

            var response = request.CreateMappedResponse(p => new TestFixture2 { AgeAsString = (p.Age * 4).ToString() });

            Assert.Equal("40", response.Payload.AgeAsString);
            Assert.Empty(response.GetErrors());
            Assert.Equal(correlationId, response.GetCorrelationId());
            Assert.Equal(customerId, response.GetCustomerId());
            Assert.Equal(gatewayId, response.GetGatewayId());
            Assert.Equal(businessTransactionId, response.GetBusinessTransactionId());
            Assert.Throws<KeyNotFoundException>(() => response.GetValue<TestFixture2, int>("custom"));
        }

        [Fact]
        public void ShouldParseBuy()
        {
            Assert.Equal(Side.Buy, Side.Parse("bUy"));
        }

        [Fact]
        public void ShouldParseSell()
        {
            Assert.Equal(Side.Sell, Side.Parse("sEll"));
        }

        [Fact]
        public void ShouldThrowOnParseWhenNeitherSellNorBuy()
        {
            Assert.Throws<ArgumentException>(() => Side.Parse("bang!"));
        }
    }
}
