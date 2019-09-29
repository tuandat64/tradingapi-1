using System.Collections.Generic;

namespace Trading.Foundation.Dtos
{
    public class TradeableAccountsSummary
    {
        public List<TradeableAccount> Accounts { get; set; }

        public decimal Balance { get; set; }

        public string Currency { get; set; }
    }
}
