using FoodAi.Persistence.Configuration;
using FoodAi.Persistence.Service;
using FoodAi.Persistence.Services;
using MassTransit;

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
            services.Configure<AzureStorageSettings>(builder.Configuration.GetSection(nameof(AzureStorageSettings)));

            services.AddSingleton<AzureBlobStorageService>();
            services.AddSingleton<StorageService>();
            services.AddSingleton<MongoDbService>();

            services.AddMassTransit(x =>
            {
                x.UsingRabbitMq(
                    (context, cfg) =>
                    {
                        var connection = builder.Configuration.GetConnectionString("messaging");
                        cfg.Host(connection);
                        // builder.Configuration.GetConnectionString("messaging")

                        cfg.ConfigureEndpoints(context);
                    }
                );

            });
            return services;
        }
    }
}
