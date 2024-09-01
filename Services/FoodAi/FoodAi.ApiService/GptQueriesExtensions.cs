namespace FoodAi.ApiService
{
    using Azure;
    using FoodAi.ApiService.Models;
    using FoodAi.ApiService.Models.Mappers;
    using FoodAi.MessageQueue.Events;
    using FoodAi.Persistence.Documents;
    using FoodAi.Persistence.Service;
    using MassTransit;
    using Microsoft.AspNetCore.Mvc;
    using System.Buffers.Text;
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
                        var result = await  mongoDbService.GetTransactionByOperationIdAsync(operationId);
                        if (result == null)
                        {
                            response = TypedResults.NotFound(result);
                        }
                        QueryResponse queryResponse = new()
                        {
                            Response = result!.OpenAIResponse.Content
                        };
                        response = TypedResults.Ok(result);
                        return response;
                    })
                .WithName("GetUserPosts")
                .WithTags(Tags)
                .WithOpenApi();

            queries
                  .MapPost(
                      "",
                      async ([FromBody] CreateQueryRequest request,
                             [AsParameters] GptQueryServices queryServices,                             
                             CancellationToken cancellationToken
                      ) =>
                      {
                          var (publisher, storageService, timeProvider) = queryServices;
                          
                          var url = await storageService.UploadImageAsync(request.UserId, timeProvider.GetUtcNow(), request.Base64Image);
                          QueryCreatedEvent queryCreatedEvent = new()
                          {
                              UserId = request.UserId,
                              ImageUrl = url,
                              CreatedAt = timeProvider.GetUtcNow(),
                              OperationId = Guid.NewGuid()
                          };
                          await publisher.Publish(queryCreatedEvent, cancellationToken);

                          return TypedResults.Accepted($"/results/{queryCreatedEvent.OperationId}");
                      }        
                  ).WithName("CreateQuery")
                   .WithTags(Tags)
                   .WithOpenApi();

            return app;
        }
    }

  
    public record class GptQueryServices(
         
        IPublishEndpoint Publisher,
        StorageService storageService,
        TimeProvider TimeProvider
    );


}
