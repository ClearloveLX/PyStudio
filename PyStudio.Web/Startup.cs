using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PyStudio.Web.Models;
using PyStudio.Model.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PyStudio.Model.ClientModel;

namespace PyStudio.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            //自定义配置
            services.Configure<PySelfSetting>(Configuration.GetSection("PySelfSetting"));

            //添加数据库上下文
            services.AddDbContext<PyStudioDBContext>(b =>
            {
                var dbLink = Configuration.GetSection("PySelfSetting:DbLink").Value;
                if (string.IsNullOrWhiteSpace(dbLink))
                {
                    throw new Exception("未找到数据库链接！");
                }
                b.UseSqlServer(dbLink);
            });

            //MemoryCaChe支持
            services.AddDistributedMemoryCache();
            services.AddMemoryCache();

            //Session支持
            services.AddSession(b =>
            {
                b.IdleTimeout = TimeSpan.FromMinutes(60);//Session过期时间
                b.Cookie.HttpOnly = true;
                b.Cookie.Name = "ClearloveLX";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseSession();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
                routes.MapRoute(
                    name: "areas",
                    template: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
