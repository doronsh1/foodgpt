using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodAi.Persistence.Documents
{
    public class OpenAiTransaction
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; } = default!;    
        public string UserId { get; set; } = default!;
        [BsonElement("CreatedAt")]
        [BsonRepresentation(BsonType.DateTime)]
        public DateTimeOffset CreatedAt { get; set;}
        public string Prompt { get; set; } = null!;
        public string ImageName { get; set; } = null!;
        public Guid OperationId { get; set; } = default!;
        public OpenAIResponse OpenAIResponse { get; set; } = null!;
    }

    public class OpenAIResponse
    {
        public string Content { get; set; } = null!;
        public Usage Usage { get; set; } = null!;
        public string Model { get; set; } = null!;
        public string Id { get; set; } = null!;
        public TimeSpan ResponseTime { get; set; } = default;
        
    }

    public class Usage
    {
        public int TotalTokens { get; set; }
        public int InputTokens { get; set; }
        public int OutputTokens { get; set; }
    }
}
