namespace Trading.Foundation.Dtos
{
    public class FxQuoteResponse
    {
        public string QuoteId { get; set; }

        public Side Side { get; set; }

        public CurrencyPair Pair { get; set; }

        public decimal BaseQuantity { get; set; }

        public decimal QuoteQuantity { get; set; }

        public decimal CommissionPercentage { get; set; }

        public long ExpiryTimestamp { get; set; }

        public override string ToString()
        {
            return $"{nameof(QuoteId)}: {QuoteId}, {nameof(Side)}: {Side}, {nameof(Pair)}: {Pair}, " +
                   $"{nameof(BaseQuantity)}: {BaseQuantity}, {nameof(QuoteQuantity)}: {QuoteQuantity}, " +
                   $"{nameof(CommissionPercentage)}: {CommissionPercentage}, " +
                   $"{nameof(ExpiryTimestamp)}: {ExpiryTimestamp}";
        }
    }
}
