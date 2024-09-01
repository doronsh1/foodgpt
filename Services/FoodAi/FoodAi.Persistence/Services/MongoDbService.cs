using FoodAi.Persistence.Configuration;
using FoodAi.Persistence.Documents;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace FoodAi.Persistence.Service
{
    public class MongoDbService
    {
        private readonly IMongoCollection<OpenAiTransaction> collection;
        public MongoDbService(IMongoClient mongoClient, IOptions<MongoSettings> settings)
        {
            var database = mongoClient.GetDatabase(settings.Value.Database);
            this.collection = database.GetCollection<OpenAiTransaction>(settings.Value.Collection);
        }

        public async Task EnsureIndexesCreatedAsync()
        {
            var indexOptions = new CreateIndexOptions { Unique = false };
            var indexKeysDefinition = Builders<OpenAiTransaction>.IndexKeys.Text(post =>
                post.UserId
            );
            var indexModel = new CreateIndexModel<OpenAiTransaction>(
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
        OpenAiTransaction query,
        CancellationToken cancellationToken = default
    ) =>
            await this.collection.InsertOneAsync(
                query,
                new InsertOneOptions(),
                cancellationToken
            );
        //await this.collection.ReplaceOneAsync(
        //    x => x.Id == query.Id,
        //    query,
        //    new ReplaceOptions { IsUpsert = true },
        //    cancellationToken

        //);
        public async Task CreateQueryBulkAsync(IEnumerable<OpenAiTransaction> openAiTransactions, CancellationToken cancellationToken = default)
        {
            await this.collection.InsertManyAsync(
                openAiTransactions,
                new InsertManyOptions(),
                cancellationToken
            );
        }

    //    public async Task<OpenAiTransaction?> GetQueryByIdAsync(
    //    string id,
    //    CancellationToken cancellationToken = default
    //) =>
    //    await this
    //        .collection.Find(x => x.Id == id)
    //        .FirstOrDefaultAsync(cancellationToken);

        public async Task<OpenAiTransaction?> GetTransactionByOperationIdAsync(
        Guid operationId,
        CancellationToken cancellationToken = default
     ) =>
            await this
            .collection.Find(x => x.OperationId == operationId)
            .FirstOrDefaultAsync(cancellationToken);


        public async Task<IEnumerable<OpenAiTransaction>> GetTransactionsByUserIdAsync(
        string userId,
        CancellationToken cancellationToken = default
    )
        {
            var filter = Builders<OpenAiTransaction>.Filter.Eq(x => x.UserId, userId);

            return await this
                .collection.Find(filter)
                .ToListAsync(cancellationToken);
        }
    }



}
