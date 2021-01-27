using System;
using System.Collections.Generic;

namespace backEnd
{
    public partial class OrderLcx
    {
        public int id { get; set; }
        public int goods_id { get; set; }
        public string goods_name { get; set; }
        public string thumb_url { get; set; }
        public decimal? price { get; set; }
        public int? buy_count { get; set; }
        public string is_pay { get; set; }
        public string user_id { get; set; }
        public int? counts { get; set; }
    }
}
