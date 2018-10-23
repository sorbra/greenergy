using Newtonsoft.Json;

namespace Greenergy.Database
{
    [JsonObject("MongoSettings")]
    public class MongoSettings
    {
        [JsonProperty("ConnectionString")]
        public string ConnectionString;
        
        [JsonProperty("Database")]
        public string Database;
    }
}