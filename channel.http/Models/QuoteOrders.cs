using System;

namespace channel.http.Models
{
    public class QuoteOrders
    {
        public enum QuoteRequestType
        {
            GetIndicativePrice, CreateContract, CreateContractAndPlaceOrder 
        }

        public class BuyQuoteRequest
        {
            public string Amount { get; set; }
            public string Currency { get; set; }
            public string PaymentMethod { get; set; }
            public QuoteRequestType QuoteRequestType { get; set; }
        }

        public class SellQuoteRequest
        {
            public string Amount { get; set; }
            public string Currency { get; set; }
            public string PaymentMethod { get; set; }
            public QuoteRequestType QuoteRequestType { get; set; }
        }

        public class QuoteResponse
        {
            public readonly string Amount;
            public readonly string Currency;
            public readonly string PaymentMethod;
            public readonly QuoteRequestType QuoteRequestType;
            public readonly Guid Id;
        }

        public class Status
        {
            public string Value { get; set; }
        }
    }
}