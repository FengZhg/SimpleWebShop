using System;
using System.Collections.Generic;

namespace backEnd
{
    public partial class RecommendLcx
    {
        public long GoodsId { get; set; }
        public string GoodsName { get; set; }
        public string ShortName { get; set; }
        public string ThumbUrl { get; set; }
        public string HdThumbUrl { get; set; }
        public string ImageUrl { get; set; }
        public int? Price { get; set; }
        public int? NormalPrice { get; set; }
        public int? MarketPrice { get; set; }
        public string SalesTip { get; set; }
        public int? Category { get; set; }
        public int? Counts { get; set; }
        public int? CommentsCount { get; set; }
    }
}
