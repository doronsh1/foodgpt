namespace FoodAi.ApiService.Models
{
    public class CreateQueryRequest
    {
        public string Base64Image { get; set; } = string.Empty;
        public string Query { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
    }
}
