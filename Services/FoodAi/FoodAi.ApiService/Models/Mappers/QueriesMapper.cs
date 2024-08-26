namespace FoodAi.ApiService.Models.Mappers
{
    using FoodAi.ApiService.Dto;
    using FoodAi.Persistence.Documents;
    using Riok.Mapperly.Abstractions;
    [Mapper]
    public static partial class QueriesMapper
    {
        public static partial QueryCreated ToQueryCreatedEvent(this GptQuery query);

        public static partial QueryCreated ToQueryViewModel(this GptQuery query);

        public static partial IEnumerable<QueryViewModel> ToQueryViewModel(this IEnumerable<GptQuery> query);
    }
}
