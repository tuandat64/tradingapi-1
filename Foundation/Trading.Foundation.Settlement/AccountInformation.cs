using Newtonsoft.Json;

namespace Trading.Foundation.Dtos
{
    public class AccountInformation
    {
        [JsonRequired]
        public string Currency { get; set; }

        [JsonRequired]
        public decimal Balance { get; set; }

        [JsonRequired]
        public decimal CreditStatus { get; set; }
    }
}
