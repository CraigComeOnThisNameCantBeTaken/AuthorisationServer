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
            services.AddControllers();
            services.AddRazorPages();

            // there are entity framework implementations you can use out of the box,
            // You can also use IdentityUser (or extend that) and IdentityRole
            // AddIdentity instead of AddIdentityCore will register all the same services
            // plus setup a default cookie auth scheme, an external scheme for google facebook etc,
            // a two factor remember me scheme so that we dont constantly require MFA (uses security stamps somehow),
            // and a two factor scheme (unclear how this works)
            // SignInManager is also registered which sits ontop of the user manager and uses these schemes.
            //services.AddIdentityCore<ApplicationUser>()
            //    .AddUserStore<ApplicationUserStore>();

            // Note: you can also create your own UserClaimsPrincipleFactory to add more claims
            // without a claims transformer and register as scoped. This is probably only better because its less db hits.
            services.AddIdentity<ApplicationUser, ApplicationRole>(o => { })
                .AddUserStore<ApplicationUserStore>()
                .AddRoleStore<ApplicationRoleStore>();

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
            services.ConfigureApplicationCookie(options => options.LoginPath = "/Login");

            services.AddAuthorization(options =>
                options.AddPolicy("MFA", x => x.RequireClaim("amr", "mfa"))); // authentication method reference     
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
                endpoints.MapControllers();
                endpoints.MapRazorPages();
                endpoints.MapFallback(context =>
                {
                    context.Response.Redirect("/");
                    return Task.CompletedTask;
                });
            });
        }
    }
}
