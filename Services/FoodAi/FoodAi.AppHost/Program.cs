var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache");

var dbServer = builder.AddMongoDB("mongodb")
    .WithEnvironment("MONGO_INITDB_ROOT_USERNAME", "admin1")
    .WithEnvironment("MONGO_INITDB_ROOT_PASSWORD", "j9Go8TSJjDL3"); 

var db = dbServer.AddDatabase("foodai");

var apiService = builder.AddProject<Projects.FoodAi_ApiService>("apiservice");
var presistance = builder.AddProject<Projects.FoodAi_Persistence>("presistance")
    .WithReference(db);


builder.AddProject<Projects.FoodAi_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(cache)
    .WithReference(apiService);

builder.Build().Run();
