using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backEnd
{
    
    public static class Home
    {
        private static HomeCasualObj[] homecasuals = new HomeCasualObj[]{ 
            new HomeCasualObj(1, "http://img60.ddimg.cn/cuxiao/dajiag750315.jpg", "http://book.dangdang.com/20201222_zhuc"), 
            new HomeCasualObj(2, "http://img58.ddimg.cn/9002820164627088.jpg", "http://store.dangdang.com/gys_0015875_x0d2"),
        };
        private static CategoryObj[] categoryObjs = new CategoryObj[]{
            new CategoryObj(1, "人文社科图书", "el-icon-reading",3),
            new CategoryObj(2, "经管图书", "el-icon-box",4),
            new CategoryObj(3, "科技图书", "el-icon-mobile-phone",5),

        };
        public static HomeCasualObj[] GetHomeCasual()
        {
            /*foreach(var i in homecasual) {
                System.Console.WriteLine(i.ToString());
            }*/

            return homecasuals;
        }
        public static CategoryObj[] GetCategory()
        {
            var content = new myShopContext();
            for (int i1 = 0; i1 < categoryObjs.Length; i1++)
            {
                var cate = categoryObjs[i1].cate_id;
                categoryObjs[i1].cate_counts=content.Recommend.Where(b=>b.category==cate).Count();
            }
            return categoryObjs;
        }

    }
    public class CategoryObj
    {
        public int cate_id { get; set; }
        public string cate_name { get; set; }
        public string cate_icon { get; set; }
        public int cate_counts { get; set; }

        public CategoryObj(int cate_id, string cate_name, string cate_icon, int cate_counts)
        {
            this.cate_id = cate_id;
            this.cate_name = cate_name;
            this.cate_icon = cate_icon;
            this.cate_counts = cate_counts;
        }
    }
    public class HomeCasualObj
    {
        public int id { get; set; }
        public string imgurl { get; set; }
        public string detail { get; set; }
        public HomeCasualObj(int id, string imgurl, string detail)
        {
            this.id = id;
            this.imgurl = imgurl;
            this.detail = detail;
        }
    }
}
