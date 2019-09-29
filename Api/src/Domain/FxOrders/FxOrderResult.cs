using BTCS.Bank.Services.Core.Model;

namespace Domain.FxOrders
{
    internal class FxOrderResult
    {
        public Order Order { get; set; }
        public string StatusMessage { get; set; }
        public int StatusCode { get; set; }
    }
}
