using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodAi.AiService.Models
{
    internal class OpenAIResult
    {
        public string Content { get; set; } = null!;
        public OpenAiUsage Usage { get; set; } = null!;
        public string Model { get; set; } = null!;
        public string Id { get; set; } = null!;
    }
}
