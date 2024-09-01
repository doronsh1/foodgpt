using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodAi.Persistence.Configuration
{
    public class AzureStorageSettings
    {
        public string ConnectionString { get; set; } = default!;
        public string ContainerName { get; set; } = default!;
    }
}
