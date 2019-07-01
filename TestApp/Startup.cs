using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mrh.Messaging;
using Mrh.Messaging.Client;
using Mrh.Messaging.Common;
using Mrh.Messaging.Json;
using Mrh.Messaging.NetMq;
using Mrh.Messaging.NetMq.Client;
using NetMqTestCommon;
using ServiceApplicationTester;
using TestApp.Hubs;

namespace TestApp
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

            services.AddSignalR();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddSingleton<
                    IConnectionIdGenerator>(
                    new ConnectionIdGenerator())
                .AddSingleton<IBodyReconstructorFactory<string>,
                    JsonBodyReconstructorFactory>()
                .AddSingleton<
                    IEnvelopFactory<PayloadType, string>,
                    JsonEnvelopeFactory<PayloadType>>()
                .AddSingleton<
                    IEncoder<PayloadType, string>,
                    StringBodyEncoder<PayloadType>>()
                .AddSingleton<
                    IJsonSetting,
                    JsonSettings>()
                .AddSingleton<
                    IForwardingClient<PayloadType, string>,
                    NetMqForwardingClient<PayloadType, string>>()
                .AddSingleton<
                    IEncoder<PayloadType, string>,
                    StringBodyEncoder<PayloadType>>()
                .AddSingleton<
                    INetMqConfig,
                    NetMqConfig>()
                .AddSingleton<
                    IBodyEncoder<string>,
                    JsonBodyEncoder>()
                .AddSingleton<
                    IMessageSetting,
                    MessagingSetting>()
                .AddSingleton<
                    IPayloadTypeEncoder<PayloadType, string>,
                    PayloadTypeEncoder>()
                ;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseWebSockets();
            app.UseCors();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}