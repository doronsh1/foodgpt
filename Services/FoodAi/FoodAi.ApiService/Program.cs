using FoodAi.ApiService;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://0.0.0.0:5305;https://0.0.0.0:7475");

//builder.WebHost.UseKestrel(options =>
//{
//    options.ListenAnyIP(7475, listenOptions =>
//    {
//        listenOptions.UseHttps("C:\\tmp\\ssl\\devcert.pfx", "openaidevssl");
//    });
//});

builder.Configuration.AddUserSecrets<Program>(); 

builder.AddApplicationServices();

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();

//builder.AddAzureBlobClient("BlobConnection");

builder.AddRabbitMQClient("messaging");
builder.AddMongoDBClient("foodai");//mongodb://admin1:j9Go8TSJjDL3@localhost:49720/?authSource=admin

var services = builder.Services;
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

services.ConfigureHttpJsonOptions(json =>
{
    json.SerializerOptions.PropertyNamingPolicy =
        JsonNamingPolicy.KebabCaseLower;
    json.SerializerOptions.WriteIndented = true;

});
builder.AddRedisOutputCache("cache");

var app = builder.Build();

app.UseCors("AllowAllOrigins");
app.UseOutputCache();
// Configure the HTTP request pipeline.
app.UseExceptionHandler();

app.UseSwagger();
app.UseSwaggerUI();

app.MapQueriesEndpoints();

if (builder.Environment.IsDevelopment())
{
    app.MapGet("/", () => Results.Redirect("/swagger"))
        .ExcludeFromDescription();
}

app.MapDefaultEndpoints();

app.Run();

 