using Newtonsoft.Json;

namespace Trading.Foundation.Dtos
{
    public class FxExecuteQuoteRequest
    {
        [JsonRequired]
        public string QuoteId { get; set; }
    }
}
