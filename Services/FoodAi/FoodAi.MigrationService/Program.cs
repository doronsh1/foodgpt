using FoodAi.MigrationService;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Initializer>();

builder.AddApplicationServices();
builder.AddMongoDBClient("foodai");
builder.AddServiceDefaults();

builder
    .Services.AddOpenTelemetry()
    .WithTracing(tracing =>
        tracing.AddSource(Initializer.ActivitySourceName)
    );

var app = builder.Build();

app.Run();