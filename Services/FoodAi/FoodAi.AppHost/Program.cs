var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache");

var dbServer = builder.AddMongoDB("mongodb")
    //.WithEnvironment("MONGO_INITDB_ROOT_USERNAME", "admin1")
    //.WithEnvironment("MONGO_INITDB_ROOT_PASSWORD", "j9Go8TSJjDL3")
    .WithDataVolume()
    .WithMongoExpress(c => c.WithHostPort(8081))
    .AddDatabase("foodai"); 


var apiService = builder.AddProject<Projects.FoodAi_ApiService>("apiservice") 
    .WithReference(dbServer);

var migrator = builder
    .AddProject < Projects.FoodAi_MigrationService>("migrator")
    .WithReference(dbServer);

//builder.AddProject<Projects.FoodAi_Web>("webfrontend")
//    .WithExternalHttpEndpoints()
//    .WithReference(cache)
//    .WithReference(apiService);



builder.Build().Run();
