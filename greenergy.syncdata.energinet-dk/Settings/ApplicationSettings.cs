using Newtonsoft.Json;
using System.Collections.Generic;

namespace Greenergy.Settings
{
    [JsonObject("application-settings")]
    public class ApplicationSettings
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("user-agent")]
        public string UserAgent { get; set; }

        [JsonProperty("energy-data-api")]
        public URLSettings EnergyDataAPI { get; set; }
    }

    [JsonObject("url-settings")]
    public class URLSettings
    {
        [JsonProperty("protocol")]
        public string Protocol { get; set; }
        [JsonProperty("host")]
        public string Host { get; set; }
        [JsonProperty("port")]
        public string Port { get; set; }
    }
}