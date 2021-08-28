using CryptoAverager.Service;
using Microsoft.Extensions.DependencyInjection;

namespace CryptoAverager.FrontEnd.Middleware
{
    public static class DependencyInjection 
    {
        public static IServiceCollection ConfigureDependencyInjection(this IServiceCollection services)
        {
            services.AddScoped<IBinanceService, BinanceService>();

            return services;
        }
    }
}
