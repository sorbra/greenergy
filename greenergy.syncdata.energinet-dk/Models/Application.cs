using Newtonsoft.Json;
using System.Collections.Generic;

namespace Greenergy.Models
{
    [JsonObject("application")]    
    public class Application
    {
        [JsonProperty("name")]
        public string Name { get; set; }

    }
}