using System;
using Trading.Foundation.Dtos;

namespace Domain.FxOrders
{
    internal class FxOrderRequest
    {
        public TradeableAccount FromAccount { get; set; }
        public TradeableAccount ToAccount { get; set; }
        public DateTime ExpiryDate { get; set; }
        public decimal Amount { get; set; }
        public string Comments { get; set; }
        public decimal CommissionPercentage { get; set; }
        public int BuySell { get; set; }
        public decimal Total { get; set; }
    }
}
