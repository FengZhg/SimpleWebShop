using System;
using System.Collections.Generic;

namespace backEnd
{
    public partial class Recommend
    {
        public long goods_id { get; set; }
        public string goods_name { get; set; }
        public string short_name { get; set; }
        public string thumb_url { get; set; }
        public string hd_thumb_url { get; set; }
        public string image_url { get; set; }
        public int? price { get; set; }
        public int? normal_price { get; set; }
        public int? market_price { get; set; }
        public string sales_tip { get; set; }
        public int? category { get; set; }
        public int? counts { get; set; }
        public int? comments_count { get; set; }
    }
}
