using channel.http.Models;

namespace channel.http.Services
{
    class OrdersService
    {
        public QuoteOrders.QuoteResponse Create(QuoteOrders.BuyQuoteRequest request)
        {
            return null;
        }

        public QuoteOrders.QuoteResponse Create(QuoteOrders.SellQuoteRequest request)
        {
            return null;
        }

        public QuoteOrders.Status QuoteOrderStatus(string id)
        {
            return null;
        }

        public SlicingOrders.SliceSpotResponse Create(SlicingOrders.BuySliceSpotRequest request)
        {
            return null;
        }

        public SlicingOrders.SliceSpotResponse Create(SlicingOrders.SellSliceSpotRequest request)
        {
            return null;
        }

        public SlicingOrders.Status SliceOrderStatus(string id)
        {
            return null;
        }
    }
}


