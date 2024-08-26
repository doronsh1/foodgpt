 

namespace FoodAi.ApiService.Models
{
    public class QueryViewModel
    {
        public string Id { get; set; } = default!;
 
        public string UserId { get; set; } = default!;
 
        public DateTimeOffset CreatedAt { get; set; }

        public string Query { get; set; } = null!;
    }
}
