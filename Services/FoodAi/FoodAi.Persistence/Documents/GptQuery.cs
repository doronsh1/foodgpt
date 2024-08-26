using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodAi.Persistence.Documents
{
    public class GptQuery
    {

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = default!;
         
    
        public string UserId { get; set; } = default!;

        [BsonElement("CreatedAt")]
        [BsonRepresentation(BsonType.DateTime)]
        public DateTimeOffset CreatedAt { get; set; }

        public string Query { get; set; } = null!;

        public Guid? PhotoId { get; set; } = null!;

    }
}
