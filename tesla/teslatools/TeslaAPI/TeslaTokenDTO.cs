using System;
using Newtonsoft.Json;

namespace Greenergy.Tesla.API
{
    [JsonObject]
    class TeslaTokenDTO
    {
        [JsonProperty("access_token")]
        public string AccessToken;
        [JsonProperty("refresh_token")]
        public string ResfreshToken;
        [JsonProperty("expires_in")]
        private int _ExpiresIn;
        [JsonProperty("created_at")]
        private int _CreatedAt;

        public DateTime CreatedAt
        {
            get
            {
                return DateTimeOffset.FromUnixTimeSeconds(_CreatedAt)
                                        .DateTime
                                        .ToLocalTime();
            }

        }
        public DateTime ExpiresAt
        {
            get
            {
                return DateTimeOffset.FromUnixTimeSeconds(_CreatedAt + _ExpiresIn)
                                .DateTime
                                .ToLocalTime();
            }
        }
    }
}