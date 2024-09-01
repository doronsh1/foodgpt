namespace FoodAi.ApiService.Models.Mappers
{ 
    using FoodAi.MessageQueue.Events;
    using FoodAi.Persistence.Documents;
    using Riok.Mapperly.Abstractions;
    [Mapper]
    public static partial class QueriesMapper
    {        
        public static partial QueryCreatedEvent ToQueryCreatedEvent(this CreateQueryRequest query);
        //{ 
        //    return new QueryCreatedEvent
        //    {
        //        UserId = query.UserId,
        //        ImageData = query.ImageData,
        //        CreatedAt = DateTimeOffset.UtcNow
        //    };
        //}
                 
        // public static partial IEnumerable<QueryViewModel> ToQueryViewModel(this IEnumerable<GptQuery> query);
    }
}
