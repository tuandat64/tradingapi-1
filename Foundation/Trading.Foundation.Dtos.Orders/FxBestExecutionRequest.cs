using Newtonsoft.Json;

namespace Trading.Foundation.Dtos
{
    public class FxBestExecutionRequest
    {
        [JsonRequired]
        public Side Side { get; set; }

        [JsonRequired]
        public CurrencyPair Pair { get; set; }

        [JsonRequired]
        public decimal Quantity { get; set; }

        [JsonRequired]
        public string Comment { get; set; }

        public override string ToString()
        {
            return $"{nameof(Side)}: {Side}, {nameof(Pair)}: {Pair}, {nameof(Quantity)}: {Quantity}, {nameof(Comment)}: {Comment}";
        }
    }
}
