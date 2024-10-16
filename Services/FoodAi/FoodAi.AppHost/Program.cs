var builder = DistributedApplication.CreateBuilder(args);


var pAdmin = builder.AddParameter("postgres-admin");
var admin = builder.AddParameter("admin");
var password = builder.AddParameter("admin-password", secret: true);
//var azureStorage = builder.AddParameter("AzureStorageSettings");
//var azureStorageContainerName = builder.AddParameter("AzureStorageSettings:ContainerName");

var cache = builder.AddRedis("cache");

//var storage = builder.AddAzureStorage("foodaidev").;
//var blobs = storage.AddBlobs("BlobConnection").;
    
var messageBus = builder
    .AddRabbitMQ("messaging", admin, password, port: 5672)
    .WithDataVolume()
    .WithManagementPlugin();

var dbServer = builder.AddMongoDB("mongodb", 49320)
    //.WithEnvironment("MONGO_INITDB_ROOT_USERNAME", "admin1")
    //.WithEnvironment("MONGO_INITDB_ROOT_PASSWORD", "j9Go8TSJjDL3")
    .WithDataVolume()
    .WithMongoExpress(c => c.WithHostPort(8081))
    .AddDatabase("foodai");




var apiService = builder.AddProject<Projects.FoodAi_ApiService>("apiservice")
    .WithReference(messageBus)
    .WithReference(dbServer)
    .WithReference(cache);
    //.WithReference(blobs);

var aiService = builder.AddProject<Projects.FoodAi_AiService>("aiservice")
    .WithReference(messageBus)
    .WithReference(dbServer);
//    .WithReference(blobs);


var migrator = builder
    .AddProject < Projects.FoodAi_MigrationService>("migrator")
    .WithReference(dbServer);



//builder.AddProject<Projects.FoodAi_Web>("webfrontend")
//    .WithExternalHttpEndpoints()
//    .WithReference(cache)
//    .WithReference(apiService);



builder.Build().Run();
