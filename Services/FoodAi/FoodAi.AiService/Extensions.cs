using FoodAi.AiService.Configuration;
using FoodAi.AiService.Consumers;
using FoodAi.Persistence.Configuration;
using FoodAi.Persistence.Service;
using FoodAi.Persistence.Services;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
 

namespace FoodAi.AiService
{
    public static class Extensions
    {
        public static IServiceCollection AddApplicationServices(this IHostApplicationBuilder builder)
        {
            var services = builder.Services;
            services.Configure<MongoSettings>(builder.Configuration.GetSection(nameof(MongoSettings)));
            services.Configure<OpenAISettings>(builder.Configuration.GetSection(nameof(OpenAISettings)));
            services.Configure<AzureStorageSettings>(builder.Configuration.GetSection(nameof(AzureStorageSettings)));

            var openAISettings = builder.Configuration.GetSection(nameof(OpenAISettings)).Get<OpenAISettings>();
            var mongoSettings = builder.Configuration.GetSection(nameof(MongoSettings)).Get<MongoSettings>();

            services.AddSingleton<OpenAIService>();

            services.AddSingleton<MongoDbService>();

            services.AddSingleton<AzureBlobStorageService>();

            services.AddMassTransit(x =>
            {
                x.AddConsumer<QueryCreatedConsumer>();
                 
                x.UsingRabbitMq(
                    (context, cfg) =>
                    {
                        cfg.Host(
                            builder.Configuration.GetConnectionString("messaging")
                        );
                        //cfg.ReceiveEndpoint("QueryCreated", e =>
                        //{
                        //    e.ConfigureConsumer<QueryCreatedConsumer>(context);
                        //});
                        cfg.ConfigureEndpoints(context);
                    }
                );

            });
            return services;
        }
    }
}
