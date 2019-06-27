using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SweepstakesAppEngineMySQL.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pomelo.EntityFrameworkCore;
using Microsoft.AspNetCore.DataProtection;
using Google.Cloud.AspNetCore.DataProtection.Kms;
using Google.Cloud.AspNetCore.DataProtection.Storage;

namespace SweepstakesAppEngineMySQL
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.Configure<Models.SecretsModel>(
                Configuration.GetSection("Secrets"));

            // Temp fix for antiforgery token issue
            //implement https://medium.com/google-cloud/antiforgery-tokens-asp-net-core-and-google-cloud-7ac6a5c7842b
            // when out of alpha
            //services.AddDataProtection()
            //    .PersistKeysToFileSystem(new DirectoryInfo(@"\\server\share\directory\"))
            //    .ProtectKeysWithCertificate("thumbprint");

            // Antiforgery tokens require data protection.
            //https://cloud.google.com/appengine/docs/flexible/dotnet/application-security
            services.AddDataProtection()
            // Store keys in Cloud Storage so that multiple instances
            // of the web application see the same keys.
            .PersistKeysToGoogleCloudStorage(
                Configuration["DataProtection:Bucket"],
                Configuration["DataProtection:Object"])
            // Protect the keys with Google KMS for encryption and fine-
            // grained access control.
            .ProtectKeysWithGoogleKms(
                Configuration["DataProtection:KmsKeyName"]);


            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseMySql(
                    Configuration.GetConnectionString("LiveConnectionPrivate")));
            services.AddDefaultIdentity<IdentityUser>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                //app.UseDeveloperExceptionPage();
                //app.UseDatabaseErrorPage();
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
