using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Greenergy.Tesla.API
{
    [JsonObject]
    class TeslaVehiclesResponseDTO
    {
        [JsonProperty("response")]
        public List<TeslaVehicleDTO> Vehicles;
    }

    [JsonObject]
    class TeslaVehicleDTO
    {
        [JsonProperty("id")]
        public string Id;
        [JsonProperty("vin")]
        public string VIN;
        [JsonProperty("display_name")]
        public string DisplayName;
        [JsonProperty("in_service")]
        public bool InService;
        [JsonProperty("state")]
        public string State;
    }

}