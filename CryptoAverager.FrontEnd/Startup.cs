using CryptoAverager.FrontEnd.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Globalization;
using System.Linq;

namespace CryptoAverager.FrontEnd
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddSingleton<WeatherForecastService>();

            services.AddLocalization(options => options.ResourcesPath = "Resources");
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

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRequestLocalization(GetLocalisationOptions());

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }

        private RequestLocalizationOptions GetLocalisationOptions()
        {
            var cultures = Configuration.GetSection("Cultures")
                .GetChildren().ToDictionary(x => x.Key, x => x.Value);

            var supportedCultures = cultures.Keys.ToArray();

            // Supported cultures handles the different data formats for cultures such as dates etc.
            // Supported UI cultures is to do with the translation files inside the resources folder.

            return new RequestLocalizationOptions()
                .AddSupportedCultures(supportedCultures)
                .AddSupportedUICultures(supportedCultures);
        }
    }
}
