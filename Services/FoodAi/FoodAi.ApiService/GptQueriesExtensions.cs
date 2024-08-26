namespace FoodAi.ApiService
{
    using FoodAi.ApiService.Models;
    using FoodAi.Persistence.Documents;
    using FoodAi.Persistence.Service;

    public static class GptQueriesExtensions
    {
        private const string Tags = "Queries";

        public static IEndpointRouteBuilder MapQueriesEndpoints(
        this IEndpointRouteBuilder app
    )
        {
            var queries = app.MapGroup("/queries");

            queries
                  .MapPost(
                      "",
                      async (CreateQueryRequest request,
                             [AsParameters] GptQueryServices queryServices, 
                             CancellationToken cancellationToken
                      ) =>
                      {
                          //var (queryService, publisher, timeProvider) = queryServices;
                          var (queryService, timeProvider) = queryServices;
                          var query = new GptQuery
                          {
                              CreatedAt = timeProvider.GetUtcNow(),
                              Query = request.Query,
                              UserId = request.UserId,
                          };
                          await queryService.CreateQureyAsync(query, cancellationToken);
                          
                          return TypedResults.Created();
                      }        
                  ).WithName("CreatePost")
                   .WithTags(Tags)
                   .WithOpenApi();

            return app;
        }
    }

    public record class GptQueryServices(
        GptQueryService queryService,
       // IPublishEndpoint Publisher,
        TimeProvider TimeProvider
    );
}
