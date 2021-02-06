using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using A19.Database.Repository.A19Test;
using A19.Database.Repository.A19Test.User;
using A19.Messaging;
using A19.Messaging.Client;
using A19.Messaging.Common;
using A19.Messaging.NetMq;
using A19.Messaging.NetMq.Client;
using A19.Security.User;
using A19.User.Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mrh.Messaging.Json;
using NetMqTestCommon;
using ServiceApplicationTester;
using TestApp.Hubs;
using TestApp.User;

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

            services.AddSignalR(o => { o.EnableDetailedErrors = true; });
            services.AddMvc(o => { o.EnableEndpointRouting = false; }).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
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
                .AddSingleton<
                    A19DatabaseConnection>()
                .AddSingleton<
                    IUserRepository,
                    UserRepository>()
                .AddSingleton<
                    IUserService,
                    UserService>()
                .AddSingleton<
                    IPasswordHashing,
                    BCryptPasswordHashing>()
                .AddSingleton<ConnectionManager>()
                .AddSingleton<SetupForwarding>()
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
            app.UseCors(builder =>
            {
                builder
                    .WithOrigins("http://localhost:4200")
                    .AllowCredentials()
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
            //app.UseSignalR(routes => { routes.MapHub<BrowserHub>("/browser"); });
        }
    }
}