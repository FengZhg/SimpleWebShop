using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.Web.Mvc;
using Newtonsoft.Json;
using System.IO;

namespace backEnd
{
    public class submitFrom
    {
        public int goods_id { get; set; }
        public String short_name { get; set; }
        public String goods_name { get; set; }
        public int counts { get; set; }
        public int price { get; set; }
        public String sales_tip { get; set; }
        public String category { get; set; }
        public byte[] goods_img { get; set; }
    }
    public class ReturnType
    {

        public int success_code { get; set; }

    }
    public class GoodReturn<T>:ReturnType
    {
        public T message { get; set; }
        public GoodReturn(T tmp)
        {
            message = tmp;
            success_code = 200;
        }
        public GoodReturn(T tmp,int success_code)
        {
            this.message = tmp;
            this.success_code = success_code;
        }
    }
    public class BadReturn : ReturnType
    {
        public String message { get; set; }
        public int error_code { get; set; }

        public BadReturn(String tmp)
        {
            message = tmp;
            error_code = 0;
        }

    }
    public class UserLCXReturn
    {
        public String id { get; set; }
        public String user_name { get; set; }
        public String user_nickname { get; set; }
        public String user_phone { get; set; }
        public String user_sex { get; set; }
        public String user_address { get; set; }
        public String user_sign { get; set; }
        public String user_birthday { get; set; }
        public String user_avatar { get; set; }
        public String user_pwd { get; set; }

        public UserLCXReturn(UserLcx tmp)
        {
            this.id = tmp.Account;
            this.user_name = tmp.Account;
            this.user_nickname = tmp.Account;
            this.user_phone = tmp.PhoneNumber;
            this.user_sex = "Male";
            this.user_address = "南宁";
            this.user_sign = "None";
            this.user_birthday = "1970-1-1";
            this.user_avatar = "None";
        }
    }
}
