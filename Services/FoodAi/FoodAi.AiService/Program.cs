// See https://aka.ms/new-console-template for more information
//Console.WriteLine("Hello, World!");

using FoodAi.AiService;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);
builder.AddApplicationServices();
// Add service defaults & Aspire components.
builder.AddServiceDefaults();
builder.Configuration.AddUserSecrets<Program>();

//builder.AddAzureBlobClient("BlobConnection");

// Add services to the container.
builder.Services.AddProblemDetails();

builder.AddMongoDBClient("foodai");//mongodb://admin1:j9Go8TSJjDL3@localhost:49720/?authSource=admin
builder.AddRabbitMQClient("messaging");

var openAIApiKey = builder.Configuration["OpenAISettings:ApiKey"];

//OpenAIService openAIService = new OpenAIService(openAIApiKey!, "gpt-4o");

//var result = openAIService.RunChatCompletionAsync("How many calories are in this meal?", "https://cdn77-s3.lazycatkitchen.com/wp-content/uploads/2020/09/basil-tofu-steak-dinner-close-up-1024x1536.jpg").Result;

//Console.WriteLine(result);

var app = builder.Build();
app.UseExceptionHandler();

app.MapDefaultEndpoints();

app.Run();
