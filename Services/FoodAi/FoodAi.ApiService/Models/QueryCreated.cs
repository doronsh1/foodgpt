 
namespace FoodAi.ApiService.Dto
{
    public class QueryCreated
    {
        public string Base64Image { get; set; } = string.Empty;
        public string Query { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public DateTimeOffset CreatedAt { get; set; }

    }
}
