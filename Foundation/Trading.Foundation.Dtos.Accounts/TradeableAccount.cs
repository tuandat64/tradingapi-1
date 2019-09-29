using System;

namespace Trading.Foundation.Dtos
{
    public class TradeableAccount
    {
        public string AccountNumber { get; set; }

        public string Nickname { get; set; }

        public string Currency { get; set; }

        public decimal TotalBalance { get; set; }

        public decimal TradeableBalance { get; set; }

        public Guid AccountId { get; set; }
    }
}
