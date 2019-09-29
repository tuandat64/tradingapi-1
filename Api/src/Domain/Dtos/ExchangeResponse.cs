namespace Domain.Dtos
{
    public class ExchangeResponse
    {
        public Rate[] Rates { get; set; }
    }


    public class Rate
    {
        public string FromCurrencyCode { get; set; }
        public string ToCurrencyCode { get; set; }
        public string FromCurrencyType { get; set; }
        public string ToCurrencyType { get; set; }
        public decimal Value { get; set; }
        public decimal Bid { get; set; }
        public decimal Ask { get; set; }
        public bool IsTradableSource { get; set; }
    }
}
