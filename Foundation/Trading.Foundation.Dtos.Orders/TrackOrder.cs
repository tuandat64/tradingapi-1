using Newtonsoft.Json;

namespace Trading.Foundation.Dtos
{
    public class TrackOrder
    {
        [JsonRequired]
        public string TrackingNumber { get; set; }
    }
}
