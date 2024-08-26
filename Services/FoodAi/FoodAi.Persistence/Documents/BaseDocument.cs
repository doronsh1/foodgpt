using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
 

namespace FoodAi.Persistence.Documents
{
    internal abstract class BaseDocument
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = default!;

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string UserId { get; set; } = default!;


    }
}
