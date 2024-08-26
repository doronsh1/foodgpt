using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodAi.Persistence.Configuration
{
    public class MongoSettings
    {
        public string? Database { get; set; } = default!;
        public string? Collection { get; set; } = default!;
 
    }
}
