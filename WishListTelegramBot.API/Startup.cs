
using WishListTelegramBot.BL;
using WishListTelegramBot.BL.Models;
using WishListTelegramBot.Core;
using WishListTelegramBot.DL;
using WishListTelegramBot.DL.Services;
using System;

namespace WishListTelegramBot.API
{
    public class Startup
    {
        public Startup(IWebHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddNewtonsoftJson();
            services.Configure<AppSettings>(Configuration);
            services.AddSingleton<Bot>();
            services.AddTransient<AppDbContext>();
            services.AddSingleton<WishListService>();
            services.AddSingleton<WishService>();
            services.AddSingleton<UserService>();
            services.AddSingleton<DataBaseConnector>();
            services.AddSingleton<UpdateDistributor>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
