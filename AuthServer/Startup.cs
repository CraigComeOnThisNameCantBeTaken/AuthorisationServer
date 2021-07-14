using AuthServer.AuthModels;
using AuthServer.IdentityServices;
using AuthServer.NetCoreIdentityServices;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;

namespace AuthServer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();

            // there are entity framework implementations you can use out of the box,
            // You can also use IdentityUser (or extend that) and IdentityRole
            //services.AddIdentity<ApplicationUser, ApplicationRole>()
            //    .AddUserStore<ApplicationUserStore>()
            //    .AddRoleStore<ApplicationRoleStore>()
            //    .AddDefaultTokenProviders();

            services.AddIdentityCore<ApplicationUser>()
                .AddUserStore<ApplicationUserStore>();

            services.AddAuthorization(options =>
                options.AddPolicy("MFA",
                    x => x.RequireClaim("amr", "mfa")));

            services.Configure<IdentityOptions>(options =>
            {
                // Password settings.
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;

                // Lockout settings.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // User settings.
                options.User.AllowedUserNameCharacters =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = false;
            });

            services.AddAuthentication("cookies")
                .AddCookie("cookies", options => options.LoginPath = "/Login");
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapFallback(context => {
                    context.Response.Redirect("/");
                    return Task.CompletedTask;
                });
            });
        }
    }
}
