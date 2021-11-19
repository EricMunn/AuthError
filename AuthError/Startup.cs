using AuthError.Auth;
using AuthError.ServiceInterface;
using Funq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ServiceStack;
using ServiceStack.Auth;
using ServiceStack.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthError
{
    public class Startup : ModularStartup
    {
        public new IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration) => Configuration = configuration;
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public new void ConfigureServices(IServiceCollection services)
        {
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseServiceStack(new AppHost
            {
                AppSettings = new NetCoreAppSettings(Configuration)
            });
        }

        public class AppHost : AppHostBase
        {
            public AppHost() : base("ReferenceQuery", typeof(MyServices).Assembly)
            {
                // Don't include auth entpoint in metadata
                typeof(Authenticate)
                    .AddAttributes(new RestrictAttribute { VisibilityTo = RequestAttributes.None });
            }

            // Configure your AppHost with the necessary configuration and dependencies your App needs
            public override void Configure(Container container)
            {
                SetConfig(new HostConfig
                {
                    DebugMode = AppSettings.Get(nameof(HostConfig.DebugMode), false)
                });

                Plugins.Add(
                     new AuthFeature(
                         sessionFactory: () => new AuthUserSession(),
                         authProviders: new IAuthProvider[] {
                        new CustomBasicAuthProvider()
                         }
                     )
                     { HtmlRedirect = null, IncludeAssignRoleServices = false, OnAuthenticateValidate = (IRequest req) => null }
                 );

            }
        }
    }
}
