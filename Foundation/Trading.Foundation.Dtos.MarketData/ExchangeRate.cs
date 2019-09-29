using System.Collections.Generic;

namespace Trading.Foundation.Dtos
{
    public class ExchangeRate
    {
        public string FromCurrency { get; set; }
        public string ToCurrency { get; set; }
        public decimal FromAmount { get; set; }
        public decimal ToAmount { get; set; }
        public string Source { get; set; }
        /// <summary>
        /// In unix time. 100ns precision
        /// </summary>
        public long Timestamp { get; set; }
        public ICollection<ExchangeRate> SubRates { get; set; }
    }
}
