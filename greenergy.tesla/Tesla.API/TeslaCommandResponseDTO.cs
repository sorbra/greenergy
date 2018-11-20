using System;
using Newtonsoft.Json;

namespace Greenergy.Tesla
{
    [JsonObject]
    class TeslaCommandResponseDTO
    {
        [JsonProperty("response")]
        public bool response;
    }
}