using Newtonsoft.Json;

namespace Trading.Foundation.Dtos
{
    public class FxExposureRequest
    {
        [JsonRequired]
        public Side Side { get; set; }

        [JsonRequired]
        public CurrencyPair Pair { get; set; }

        [JsonRequired]
        public string BankCurrency { get; set; }

        [JsonRequired]
        public decimal BaseAmount { get; set; }

        [JsonRequired]
        public decimal QuoteAmount { get; set; }
    }
}
