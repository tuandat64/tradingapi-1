using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace channel.http.Models
{
    public class SlicingOrders
    {
        public class BuySliceSpotRequest
        {
            public string Value { get; set; }
        }

        public class SellSliceSpotRequest
        {
            public string Value { get; set; }
        }

        public class SliceSpotResponse
        {
            public string Value { get; set; }
        }

        public class Status
        {
            public string Value { get; set; }
        }
    }
}