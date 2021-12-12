using CrossBIM.HelperClasses;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace CrossBIM
{
    public class Startup
    {
        public IWebHostEnvironment Hosting { get; set; }
        public Startup(IConfiguration configuration, IWebHostEnvironment _Hosting)
        {
            Configuration = configuration;
            Hosting = _Hosting;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            bool connectionStatus = CheckForInternetConnection();

            DeletewwwRootFilesAfterOneHour();

            if (connectionStatus)
                HelperFunctions.DeleteCloudibaryFilesAfterOneHour();

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
        public void DeletewwwRootFilesAfterOneHour()
        {
            string dirName = Path.Combine(Hosting.WebRootPath, "Uploads");
              
            try
            {
                string[] files = Directory.GetFiles(dirName);

                foreach (string file in files)
                {
                    FileInfo fi = new FileInfo(file);
                    //if (fi.LastAccessTime < DateTime.Now.AddMonths(-3))
                    if (fi.CreationTime < DateTime.Now.AddHours(-1))
                        fi.Delete();
                }
            }
            catch (Exception)
            {


            }
        }

        /*https://stackoverflow.com/questions/2031824/what-is-the-best-way-to-check-for-internet-connectivity-using-ne*/
        public static bool CheckForInternetConnection(int timeoutMs = 10000, string url = null)
        {
            try
            {
                url ??= CultureInfo.InstalledUICulture switch
                {
                    { Name: var n } when n.StartsWith("fa") => // Iran
                        "http://www.aparat.com",
                    { Name: var n } when n.StartsWith("zh") => // China
                        "http://www.baidu.com",
                    _ =>
                        "http://www.gstatic.com/generate_204",
                };

                var request = (HttpWebRequest)WebRequest.Create(url);
                request.KeepAlive = false;
                request.Timeout = timeoutMs;
                using var response = (HttpWebResponse)request.GetResponse();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
