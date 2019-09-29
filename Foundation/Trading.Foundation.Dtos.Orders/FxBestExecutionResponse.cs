using Newtonsoft.Json;

namespace Trading.Foundation.Dtos
{
    public class FxBestExecutionResponse
    {
        public string State { get; set; }

        public Side Side { get; set; }

        public CurrencyPair Pair { get; set; }

        public string TrackingNumber { get; set; }

        public decimal BaseQuantity { get; set; }

        public decimal QuoteQuantity { get; set; }

        public decimal CommissionPercentage { get; set; }

        public string Description { get; set; }

        public string Comment { get; set; }

        public override string ToString()
        {
            return $"{nameof(State)}: {State}, " +
                   $"{nameof(Side)}: {Side}, {nameof(Pair)}: {Pair}, {nameof(TrackingNumber)}: {TrackingNumber}, " +
                   $"{nameof(BaseQuantity)}: {BaseQuantity}, {nameof(QuoteQuantity)}: {QuoteQuantity}, " +
                   $"{nameof(CommissionPercentage)}: {CommissionPercentage}, {nameof(Description)}: {Description}, " +
                   $"{nameof(Comment)}: {Comment}";
        }
    }
}
