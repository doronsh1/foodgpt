using FoodAi.Persistence.Configuration;
using FoodAi.Persistence.Service;

namespace FoodAi.ApiService
{
    public static class Extensions
    {
        public static IServiceCollection AddApplicationServices(
        this IHostApplicationBuilder builder
    )
        {
            var services = builder.Services;
            services.Configure<MongoSettings>(builder.Configuration.GetSection(nameof(MongoSettings)));

            services.AddSingleton<GptQueryService>();

            var mongoSettings = builder.Configuration.GetSection(nameof(MongoSettings)).Get<MongoSettings>();

            return services;
        }
    }
}
