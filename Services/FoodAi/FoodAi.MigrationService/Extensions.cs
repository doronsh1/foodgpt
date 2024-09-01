
namespace FoodAi.MigrationService
{
    using FoodAi.Persistence.Configuration;
    using FoodAi.Persistence.Service;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    public static class Extensions
    {
        public static IServiceCollection AddApplicationServices(
        this IHostApplicationBuilder builder
    )
        {
            var services = builder.Services;

            services.Configure<MongoSettings>(
                builder.Configuration.GetSection(nameof(MongoSettings))
            );

            var mongoSettings = builder.Configuration.GetSection(nameof(MongoSettings)).Get<MongoSettings>();

            services.AddSingleton<MongoDbService>();

            return services;
        }
    }
}
