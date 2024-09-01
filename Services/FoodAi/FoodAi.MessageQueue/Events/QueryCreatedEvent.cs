using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodAi.MessageQueue.Events
{
    public class QueryCreatedEvent
    {
        public string UserId { get; set; } = default!;                
        public string ImageUrl { get; set; } = default!;
        public DateTimeOffset CreatedAt { get; set; }
        public Guid OperationId { get; set; } = default!;
    }
}
