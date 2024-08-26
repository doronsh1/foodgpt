using FoodAi.Persistence.Configuration;
using FoodAi.Persistence.Documents;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace FoodAi.Persistence.Service
{
    public class GptQueryService
    {
        private readonly IMongoCollection<GptQuery> collection;
        public GptQueryService(IMongoClient mongoClient, IOptions<MongoSettings> settings)
        {
            var database = mongoClient.GetDatabase(settings.Value.Database);
            this.collection = database.GetCollection<GptQuery>(settings.Value.Collection);
        }

        public async Task EnsureIndexesCreatedAsync()
        {
            var indexOptions = new CreateIndexOptions { Unique = false };
            var indexKeysDefinition = Builders<GptQuery>.IndexKeys.Text(post =>
                post.UserId
            );
            var indexModel = new CreateIndexModel<GptQuery>(
                indexKeysDefinition,
                indexOptions
            );

            await this.collection.Indexes.CreateOneAsync(indexModel);
        }


        public async Task<bool> IsEmptyCollectionAsync()
        {
            var count = await this.collection.CountDocumentsAsync(
                new BsonDocument()
            );
            return count == 0;
        }

        public async Task CreateQureyAsync(
        GptQuery  query,
        CancellationToken cancellationToken = default
    ) =>
        await this.collection.ReplaceOneAsync(
            x => x.Id == query.Id,
            query,
            new ReplaceOptions { IsUpsert = true },
            cancellationToken

        );
        public async Task CreateQueryBulkAsync(IEnumerable<GptQuery> posts, CancellationToken cancellationToken = default)
        {
            await this.collection.InsertManyAsync(
                posts,
                new InsertManyOptions(),
                cancellationToken
            );
        }

        public async Task<GptQuery?> GetQueryByIdAsync(
        string id,
        CancellationToken cancellationToken = default
    ) =>
        await this
            .collection.Find(x => x.Id == id)
            .FirstOrDefaultAsync(cancellationToken);


        public async Task<IEnumerable<GptQuery>> GetAuthorPostsAsync(
        string userId,
        CancellationToken cancellationToken = default
    )
        {
            var filter = Builders<GptQuery>.Filter.Eq(x => x.UserId, userId);

            return await this
                .collection.Find(filter)
                .ToListAsync(cancellationToken);
        }
    }



}
