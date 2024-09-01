using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodAi.AiService.Models
{
    public class GptQuery1
    {
        public string UserId { get; set; } = default!;
        public DateTimeOffset CreatedAt { get; set; }
        public string Query { get; set; } = null!;
        public string PhotoUrl { get; set; } = null!;
    }
}
