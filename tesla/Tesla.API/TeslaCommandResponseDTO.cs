using System;
using Newtonsoft.Json;

namespace Greenergy.Tesla
{
    [JsonObject]
    class TeslaCommandResponseType
    {
        [JsonProperty("reason")]
        public string reason;
        [JsonProperty("result")]
        public bool result;
    }
}