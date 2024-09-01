using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodAi.AiService.Models
{
    internal class OpenAiUsage
    {
        public int TotalTokens { get; set; }
        public int InputTokens { get; set; }
        public int OutputTokens { get; set; }
    }
}
