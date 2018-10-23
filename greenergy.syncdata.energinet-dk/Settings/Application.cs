using Newtonsoft.Json;
using System.Collections.Generic;

namespace Greenergy.Settings
{
    [JsonObject("application")]    
    public class Application
    {
        [JsonProperty("name")]
        public string Name { get; set; }

    }
}