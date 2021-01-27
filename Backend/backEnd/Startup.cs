using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using backEnd;

namespace backEnd
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            Console.WriteLine("Êä³öÅäÖÃÇë¿ö" + Configuration.GetConnectionString("MysqlConnection"));

        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {



            services.AddDistributedMemoryCache();

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(60);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
            services.AddDataProtection().PersistKeysToFileSystem(
                new DirectoryInfo(
                    Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "DataProtection"));

            services.AddControllers().AddNewtonsoftJson();
            /*services.AddDbContextPool<myShopContext>(option =>
                option.UseMySql(Configuration.GetConnectionString("MysqlConnection")),
                 poolSize: 70
            );*/


            services.AddControllers();

            services.AddDbContextPool<myShopContext>(options => 
                options.UseMySql("server=172.17.0.1;userid=root;password=root;database=myShop;Port=3306;sslmode=None;charset=utf8", 
                    x => {
                        x.ServerVersion("8.0.22-mysql");
                        x.EnableRetryOnFailure(); 
                    }));

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSession();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

        }
    }
}
