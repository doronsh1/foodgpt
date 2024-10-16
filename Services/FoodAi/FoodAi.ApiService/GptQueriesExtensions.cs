namespace FoodAi.ApiService
{
    using Azure;
    using Azure.Storage.Blobs;
    using FoodAi.ApiService.Models;
    using FoodAi.ApiService.Models.Mappers;
    using FoodAi.MessageQueue.Events;
    using FoodAi.Persistence.Documents;
    using FoodAi.Persistence.Service;
    using FoodAi.Persistence.Services;
    using MassTransit;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using System.Buffers.Text;
    using System.Net.Http;
    using System.Security.Cryptography;
    using System.Text;
    using static MassTransit.ValidationResultExtensions;

    public static class GptQueriesExtensions
    {
        private const string Tags = "Queries";

        public static IEndpointRouteBuilder MapQueriesEndpoints(
        this IEndpointRouteBuilder app
    )
        {
            var queries = app.MapGroup("/queries");
            app.MapGet(
                    "/results/{operationId}",
                    async (
                        Guid operationId,
                        [FromServices] MongoDbService mongoDbService,
                        CancellationToken cancellationToken
                    ) =>
                    {
                        IResult response;
                        var result = await mongoDbService.GetTransactionByOperationIdAsync(operationId);
                        if (result == null)
                        {
                            response = TypedResults.NotFound(result);
                            return response;
                        }
                        QueryResponse queryResponse = new()
                        {
                            Response = result!.OpenAIResponse.Content
                        };
                        response = TypedResults.Ok(queryResponse);
                        return response;
                    })
                .WithName("GetUserPosts")
                .WithTags(Tags)
                .WithOpenApi();

            _ = queries
                  .MapPost(
                      "",
                      async ([FromBody] CreateQueryRequest request,
                             [AsParameters] GptQueryServices queryServices,
                             HttpContext httpContext,
                             [FromServices] RedisCacheService cacheService,
                             CancellationToken cancellationToken

                      ) =>
                      {
                          httpContext.Response.Headers.Add("Access-Control-Expose-Headers", "Location, Date, Server");
                          var (publisher, storageService, timeProvider) = queryServices;

                          var key = GenerateCacheKey(request.UserId, request.Base64Image);
                          var cachedOperationId = await cacheService.GetAsync<CreateQueryRequest>(key);
                          if (cachedOperationId != null)
                          {
                              return TypedResults.Accepted($"/results/{cachedOperationId}");
                          }
                          var url = await storageService.UploadImageAsync(request.UserId, timeProvider.GetUtcNow(), request.Base64Image);
                          QueryCreatedEvent queryCreatedEvent = new()
                          {
                              UserId = request.UserId,
                              ImageUrl = url,
                              CreatedAt = timeProvider.GetUtcNow(),
                              OperationId = Guid.NewGuid()
                          };
                          await publisher.Publish(queryCreatedEvent, cancellationToken);
                          await cacheService.SetAsync(key, queryCreatedEvent.OperationId);

                          return TypedResults.Accepted($"/results/{queryCreatedEvent.OperationId}");
                      }
                  ).WithName("CreateQuery")
                   .WithTags(Tags)
                   .WithOpenApi()
                   .CacheOutput()
                   .RequireCors("AllowAllOrigins");

            return app;
        }

        private static string GenerateCacheKey(string userId, string base64Image)
        {
            // Concatenate userid and base64 image
            string dataToHash = $"{userId}:{base64Image}";

            // Compute SHA256 hash
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] sourceBytes = Encoding.UTF8.GetBytes(dataToHash);
                byte[] hashBytes = sha256Hash.ComputeHash(sourceBytes);
                string hash = BitConverter.ToString(hashBytes).Replace("-", string.Empty);
                return hash;
            }
        }
    }

    public record class GptQueryServices(

        IPublishEndpoint Publisher,
        StorageService storageService,
        TimeProvider TimeProvider
    );


}
