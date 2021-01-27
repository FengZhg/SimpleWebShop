using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Net.Mime;
using System.Text;
using System.Data;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net.Http;


namespace backEnd.Controllers
{
    [ApiController]
    public class ApplicationController : BaseControl
    {

        private readonly ILogger<ApplicationController> _logger;
        public ApplicationController(ILogger<ApplicationController> logger)
        {
            _logger = logger;
        }

        [HttpGet("/api/homecasual")]
        public GoodReturn<HomeCasualObj[]> GetHomeCasusal()
        {
            return new GoodReturn<HomeCasualObj[]>(Home.GetHomeCasual());
        }

        [HttpGet("/api/category")]
        public GoodReturn<CategoryObj[]> GetCategory()
        {
            return new GoodReturn<CategoryObj[]>(Home.GetCategory());
        }

        [HttpGet("/api/captcha")]
        public IActionResult GetCaptcha()
        {
            Task<CaptchaResult> GetCaptchaTask = Task<CaptchaResult>.Run(() =>
            {
                return Captcha.GetCaptchaResult();
            });
            var result = GetCaptchaTask.Result;
            HttpContext.Session.SetString("CaptchaCode", result.CaptchaCode);
            Console.WriteLine(HttpContext.Session.GetString("CaptchaCode") + " ");
            Stream s = new MemoryStream(result.CaptchaByteData);
            return new FileStreamResult(s, "image/png");
        }

        [HttpPost("/api/login_pwd")]
        public async Task<ReturnType> PostLoginCheck([FromBody] JObject jsonobj)
        {
            var content = new myShopContext();
            var jsonStr = JsonConvert.SerializeObject(jsonobj);
            var jsonParams = JsonConvert.DeserializeObject<dynamic>(jsonStr);
            String name = jsonParams.name, pwd = jsonParams.pwd, captcha = jsonParams.captcha;
            Console.WriteLine(HttpContext.Session.GetString("CaptchaCode") + " " + captcha);
            if (HttpContext.Session.GetString("CaptchaCode") != captcha)
            {
                return new BadReturn("Bad Captcha");
            }
            backEnd.UserLcx re = null;
            re = content.UserLcx.SingleOrDefault(b => b.Account.Equals(name));
            if (re != null && re.Password.Equals(Encryption.encrypt(pwd)))
            {
                HttpContext.Session.SetString("Account", name);
                return new GoodReturn<UserLCXReturn>(new UserLCXReturn(re));
            }
            else if (re != null)
            {
                return new BadReturn("LOGIN ERROR");
            }
            Task<GoodReturn<UserLCXReturn>> task = Task.Run(() =>
            {
                backEnd.UserLcx newUser = new backEnd.UserLcx();
                newUser.Account = name;
                newUser.Password = Encryption.encrypt(pwd);
                content.UserLcx.Add(newUser);
                content.SaveChanges();
                return new GoodReturn<UserLCXReturn>(new UserLCXReturn(newUser));
            });
            HttpContext.Session.SetString("Account", name);
            HttpContext.Session.SetString("CaptchaCode", "1000086");
            return task.Result;
        }
        [HttpGet("/api/homeshoplist")]
        public ReturnType GetHomeShopList()
        {
            var categoryObjs = Home.GetCategory();
            var result = new List<List<Recommend>>();
            var task = new List<Task<List<Recommend>>>();
            foreach (var i in categoryObjs)
            {
                task.Add(Task<List<Recommend>>.Run(() =>
                {
                    var content = new myShopContext();
                    var cateID = i.cate_id;
                    var tmp = content.Recommend.Where(b => (b.category == cateID)).ToList();
                    tmp = tmp.GetRange(0, Math.Min(3, tmp.Count()));
                    return tmp;
                }));
            }
            Task.WaitAll(task.ToArray());
            foreach (var i in task)
            {
                result.Add(i.Result.ToList());
            }
            Console.WriteLine(result);
            return new GoodReturn<List<List<Recommend>>>(result);
        }
        [HttpGet("/api/goodsdetail")]
        public ReturnType GetGoodDetail()
        {
            var content = new myShopContext();
            if (Request.Query.ContainsKey("goodsNo"))
            {
                long goodsNo = 0;
                try
                {
                    goodsNo = Convert.ToInt64(Request.Query["goodsNo"].ToString());
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return new BadReturn("商品不存在");
                }
                Console.WriteLine(goodsNo);
                var result = new List<Recommend>();
                result.Add(content.Recommend.SingleOrDefault(b => b.goods_id == goodsNo));
                if (result != null)
                    return new GoodReturn<List<Recommend>>(result);
                else
                    return new BadReturn("商品不存在");
            }
            else
            {
                return new BadReturn("商品不存在");
            }
        }
        [HttpGet("/api/goodscomment")]
        public ReturnType GetGoodComment()
        {
            String comment = new string("[{\"id\":5,\"user_name\":\"13666666666\",\"user_nickname\":\"\",\"comment_detail\":\"强烈推荐\",\"comment_id\":25,\"comment_rating\":5,\"goods_id\":621723438},{\"id\":9,\"user_name\":\"来来来\",\"user_nickname\":\"HS\",\"comment_detail\":\"毒鸡汤\",\"comment_id\":26,\"comment_rating\":2,\"goods_id\":621723438}]".Replace("\\", ""));
            if (Request.Query.ContainsKey("goodsId"))
            {
                try
                {
                    var tm = JsonConvert.DeserializeObject(comment.Replace("621723438", Request.Query["goodsId"].ToString()));
                    return new GoodReturn<object>(tm);

                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return new BadReturn("商品不存在");
                }
            }
            return new BadReturn("商品号错误");
        }
        [HttpPost("/api/admin_login")]
        public ReturnType GetAdminLogin([FromBody] JObject jsonobj)
        {
            var content = new myShopContext();
            var jsonStr = JsonConvert.SerializeObject(jsonobj);
            var jsonParams = JsonConvert.DeserializeObject<dynamic>(jsonStr);
            String name = "admin", pwd = jsonParams.pwd;
            if (content.UserLcx.SingleOrDefault(b => b.Account.Equals(name)) == null)
            {
                var newUser = new UserLcx();
                newUser.Account = "admin";
                newUser.Password = Encryption.encrypt("admin");
                newUser.UserPower = "1";
                content.UserLcx.Add(newUser);
                content.SaveChanges();
            }
            name = jsonParams.account;
            if (content.UserLcx.SingleOrDefault(b => (b.Account.Equals(name) && b.UserPower.Equals("1"))).Password == Encryption.encrypt(pwd))
            {
                HttpContext.Session.SetString("admin", "true");
                return new GoodReturn<String>("登录成功！");
            }
            else
                return new BadReturn("账号和密码错误！");
        }
        [HttpGet("/api/allgoods")]
        public ReturnType GetAllGoods()
        {
            if (HttpContext.Session.GetString("admin") != "true")
            {
                return new BadReturn("无权限");
            }
            var content = new myShopContext();
            var tmp = content.Recommend.Where(b => b.goods_id != null).ToList();
            return new GoodReturn<List<Recommend>>(tmp.ToList());
        }

        [HttpPost("/api/update_recom_goods")]
        public ReturnType GetUpdateRecomGoods([FromBody] JObject jsonobj)
        {
            if (HttpContext.Session.GetString("admin") != "true")
            {
                return new BadReturn("无权限");
            }
            var content = new myShopContext();
            var jsonStr = JsonConvert.SerializeObject(jsonobj);
            var jsonParams = JsonConvert.DeserializeObject<dynamic>(jsonStr);
            long id = jsonParams.goods_id;
            var tmp = content.Recommend.SingleOrDefault(b => (b.goods_id == id));
            if (tmp == null)
            {
                return new BadReturn("商品id不存在");
            }
            tmp.goods_name = jsonParams.goods_name;
            tmp.short_name = jsonParams.short_name;
            tmp.price = jsonParams.price;
            tmp.counts = jsonParams.counts;
            content.SaveChanges();
            return new GoodReturn<String>("修改成功");
        }

        [HttpGet("/api/admin_allusers")]
        public ReturnType GetAllUsers()
        {
            var content = new myShopContext();
            if (HttpContext.Session.GetString("admin") != "true")
            {
                return new BadReturn("无权限");
            }
            var tmp = content.UserLcx.Where(b => b.Account != "").ToList();
            var re = new List<UserLCXReturn>();
            foreach (var i in tmp)
            {
                re.Add(new UserLCXReturn(i));
            }
            return new GoodReturn<List<UserLCXReturn>>(re);
        }
        [HttpGet("/api/admin_logout")]
        public ReturnType AdminLogout()
        {
            HttpContext.Session.SetString("admin", "false");
            return new GoodReturn<String>("退出登陆成功");
        }

        [HttpPost("/api/delete_recom_goods")]
        public ReturnType DelRecomGoods([FromBody] JObject jsonobj)
        {
            if (HttpContext.Session.GetString("admin") != "true")
                return new BadReturn("无权限");
            var content = new myShopContext();
            var jsonStr = JsonConvert.SerializeObject(jsonobj);
            var jsonParams = JsonConvert.DeserializeObject<dynamic>(jsonStr);
            long id = jsonParams.goods_id;
            var tmp = content.Recommend.SingleOrDefault(b => (b.goods_id == id));
            if (tmp == null)
                return new BadReturn("该商品不存在");
            try
            {
                content.Recommend.Remove(tmp);
                content.SaveChanges();
                return new GoodReturn<String>("删除成功！");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new BadReturn("该商品不存在");
            }
        }
        [HttpPost("/api/add_shop_recom")]
        public ReturnType AddRecomGoods(IFormCollection collection)
        {
            if (HttpContext.Session.GetString("admin") != "true")
            {
                return new BadReturn("无权限");
            }
            var content = new myShopContext();
            long goodsId = long.Parse(collection["goods_id"].ToString());
            if (content.Recommend.SingleOrDefault(b => b.goods_id == goodsId) != null)
            {
                return new GoodReturn<String>("商品已存在", 500);
            }
            var newGoods = new Recommend();
            newGoods.goods_id = int.Parse(collection["goods_id"]);
            newGoods.short_name = collection["short_name"];
            newGoods.goods_name = collection["goods_name"];
            newGoods.counts = int.Parse(collection["counts"]);
            newGoods.price = int.Parse(collection["price"]);
            newGoods.sales_tip = collection["sales_tip"];
            newGoods.category = int.Parse(collection["category"]);
            newGoods.image_url = "图床暂未实现";

            try
            {
                content.Recommend.Add(newGoods);
                content.SaveChanges();
                return new GoodReturn<String>("商品上架成功！");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new BadReturn("商品上架失败");
            }
        }
        [HttpPost("/api/add_shop_cart")]
        public ReturnType AddShopCart([FromBody] JObject jsonobj)
        {
            if (HttpContext.Session.GetString("Account").Equals(""))
            {
                Console.WriteLine(HttpContext.Session.GetString("Account"));
                return new BadReturn("未登录");
            }
            var content = new myShopContext();
            var jsonStr = JsonConvert.SerializeObject(jsonobj);
            var jsonParams = JsonConvert.DeserializeObject<dynamic>(jsonStr);
            String user_id = jsonParams.user_id;
            long goods_id = jsonParams.goods_id;
            var CheckExist = content.OrderLcx.SingleOrDefault(b => b.user_id.Equals(user_id) && b.goods_id == goods_id);
            if (CheckExist != null)
            {
                int tmp = jsonParams.buy_count;
                int buyCount = int.Parse(CheckExist.buy_count.GetValueOrDefault().ToString()) + tmp;
                CheckExist.buy_count = buyCount;
                content.SaveChanges();
                return new GoodReturn<String>("购物车添加成功");
            }
            var order = new OrderLcx();
            order.buy_count = jsonParams.buy_count;
            order.counts = jsonParams.counts;
            order.goods_id = jsonParams.goods_id;
            order.goods_name = jsonParams.goods_name;
            order.price = jsonParams.price;
            order.thumb_url = jsonParams.thumb_url;
            order.user_id = jsonParams.user_id;
            try
            {
                content.OrderLcx.Add(order);
                content.SaveChanges();
                return new GoodReturn<String>("购物车添加成功");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new BadReturn("购物车添加失败");
            }
        }
        [HttpGet("/api/user_info")]
        public ReturnType GetUserInfo()
        {
            try
            {
                if (!HttpContext.Session.GetString("Account").Any() && HttpContext.Session.GetString("Account") != null
                                                                                && !HttpContext.Session.GetString("Account").Equals(""))
                    return new BadReturn("用户未登录");
            }
            catch (Exception e)
            {
                return new BadReturn("用户未登录");
            }
            var content = new myShopContext();
            var userInfo = content.UserLcx.SingleOrDefault(b => b.Account.Equals(HttpContext.Session.GetString("Account")));
            if (userInfo == null)
                return new BadReturn("用户信息缺失");
            return new GoodReturn<UserLCXReturn>(new UserLCXReturn(userInfo));
        }

        [HttpGet("/api/cart_goods")]
        public ReturnType GetCartGoods()
        {
            if (!Request.Query.ContainsKey("user_id"))
                return new BadReturn("用户传递id错误");
            try
            {
                if (HttpContext.Session.GetString("Account")==null || HttpContext.Session.GetString("Account").ToString().Equals(""))
                {
                    return new BadReturn("用户暂未登录");
                }
            }
            catch (Exception e)
            {
                return new BadReturn("用户暂未登录");
            }
            var content = new myShopContext();
            var user_id = Request.Query["user_id"].ToString();
            var userCart = content.OrderLcx.Where(b => b.user_id.Equals(user_id));
            try
            {
                if (userCart.Count() == 0)
                    return new BadReturn("用户购物车为空");
            }
            catch (Exception e)
            {
                return new BadReturn("用户购物车为空");
            }
            var uCart = userCart.ToList();
            return new GoodReturn<List<OrderLcx>>(uCart);
        }

        [HttpPost("/api/delete_all_goods")]
        public ReturnType SellGoods([FromBody] JObject jsonobj)
        {
            if (HttpContext.Session.GetString("Account").Equals(""))
            {
                return new BadReturn("未登录");
            }
            var contentTmp = new myShopContext();
            var jsonStr = JsonConvert.SerializeObject(jsonobj);
            var jsonParams = JsonConvert.DeserializeObject<dynamic>(jsonStr);
            String user_id = jsonParams.user_id;
            var goods_ids = contentTmp.OrderLcx.Where(b => b.user_id.Equals(user_id));
            var task = new List<Task>();
            if (!goods_ids.Any())
            {
                return new BadReturn("用户购物车为空");
            }
            foreach (var i in goods_ids)
            {
                task.Add(Task.Run(() =>
                {
                    var content = new myShopContext();
                    var goodID = i.goods_id;
                    var goodAmount = i.buy_count;
                    var tmp = content.Recommend.SingleOrDefault(b => (b.goods_id == goodID));
                    if (tmp!=null)
                    {
                        content.OrderLcx.Remove(i);
                        tmp.counts = tmp.counts - goodAmount;
                    }
                    content.SaveChanges();
                }));
            }
            Task.WaitAll(task.ToArray());
            return new GoodReturn<String>("结算成功");
        }
        [HttpPost("/api/change_goods_count")]
        public ReturnType ChangeGoodsCount([FromBody] JObject jsonobj)
        {
            if (HttpContext.Session.GetString("Account").Equals(""))
            {
                return new BadReturn("未登录");
            }
            var content = new myShopContext();
            var jsonStr = JsonConvert.SerializeObject(jsonobj);
            var jsonParams = JsonConvert.DeserializeObject<dynamic>(jsonStr);
            String user_id = jsonParams.user_id;
            long goods_id = jsonParams.goods_id;
            var CheckExist = content.OrderLcx.SingleOrDefault(b => b.user_id.Equals(user_id) && b.goods_id == goods_id);
            if (CheckExist != null)
            {
                CheckExist.buy_count = jsonParams.buy_count;
                content.SaveChanges();
                return new GoodReturn<String>("购物车添加成功");
            }
            return new BadReturn("购物车更新失败");
        }
        [HttpGet("/api/logout")]
        public ReturnType Logout()
        {
            HttpContext.Session.SetString("Account", "");
            return new GoodReturn<String>("退出登陆成功");
        }
        [HttpGet("/api/recommendshoplist")]
        public ReturnType GetRecommendShopList()
        {
            if (!Request.Query.ContainsKey("category") || !Request.Query.ContainsKey("pageNo") || !Request.Query.ContainsKey("count"))
                return new BadReturn("参数错误");
            var content = new myShopContext();

            var category = long.Parse(Request.Query["category"].ToString() == "NaN" ? "1" : Request.Query["category"].ToString());
            var pageNo = int.Parse(Request.Query["pageNo"].ToString() == "NaN" ? "1" : Request.Query["pageNo"].ToString());
            var count = int.Parse(Request.Query["count"].ToString());
            try
            {
                var tmp = content.Recommend.Where(b => b.category == category).ToList();
                tmp = tmp.GetRange((pageNo - 1) * count, Math.Min(count, tmp.Count() - (pageNo - 1) * count));
                return new GoodReturn<List<Recommend>>(tmp);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new BadReturn("请求错误");
            }
        }
    }
}
