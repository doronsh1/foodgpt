using Bogus;
using FoodAi.Persistence.Documents;
using FoodAi.Persistence.Service;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry.Trace;
using System.Diagnostics;

namespace FoodAi.MigrationService
{

    public class Initializer(
    IServiceProvider serviceProvider,
    IHostApplicationLifetime hostApplicationLifetime
) : BackgroundService
    {
        public const string ActivitySourceName = "Migrations";
        private static readonly ActivitySource ActivitySource = new(ActivitySourceName);
        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var activity = ActivitySource.StartActivity(
             "Migrating database",
             ActivityKind.Client
            );
            try
            {
                using var scope = serviceProvider.CreateScope();                

                var queries = GenerateQueries();

                var seeded = await SeedGptQueryDatabaseAsync(
                scope,
                queries,
                stoppingToken
            );
            }
            catch (Exception ex)
            {
                activity?.RecordException(ex);
                throw;
            }
            hostApplicationLifetime.StopApplication();
        }

        private static async Task<bool> SeedGptQueryDatabaseAsync(
                                         IServiceScope scope,
                                         IEnumerable<GptQuery> queries,
                                         CancellationToken stoppingToken
 )
        {
            using var activity = ActivitySource.StartActivity(
                "Seeding Mongo database",
                ActivityKind.Client
            );
            var queryService =
            scope.ServiceProvider.GetRequiredService<GptQueryService>();

            var empty = await queryService.IsEmptyCollectionAsync();

            if (!empty)
            {
                return false;
            }
            await queryService.EnsureIndexesCreatedAsync();

            await queryService.CreateQueryBulkAsync( queries );

            return true;
        }
        private static List<GptQuery> GenerateQueries()
        {
            var faker = new Faker<GptQuery>()
                .RuleFor(p => p.Id, f => f.Random.AlphaNumeric(10))
                .RuleFor(p => p.CreatedAt, f => f.Date.Past())
                .RuleFor(p => p.UserId, f => f.Random.AlphaNumeric(10))
                .RuleFor(p => p.Query, f => (""));
            
            var queries = faker.Generate(1).ToList();
            
            return queries;


        }
    }
}
