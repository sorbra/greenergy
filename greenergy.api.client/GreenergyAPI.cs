using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Runtime.Serialization.Json;
using Greenergy.API.Models;

namespace Greenergy.API
{
    public class GreenergyAPI : IGreenergyAPI
    {
        private IOptions<GreenergyAPISettings> _config;
        private ILogger<GreenergyAPI> _logger;

        public GreenergyAPI(
            IOptions<GreenergyAPISettings> config,
            ILogger<GreenergyAPI> logger)
        {
            _config = config;
            _logger = logger;
        }

        public async Task<DateTime> GetMostRecentEmissionsTimeStamp()
        {
            var latestEmissions = await GetMostRecentEmissions();
            if (latestEmissions != null && latestEmissions.Count > 0)
            {
                return latestEmissions[0].TimeStampUTC;
            }
            else
            {
                return DateTime.MinValue;
            }
        }
        public async Task<List<EmissionDataDTO>> GetMostRecentEmissions()
        {
            string apiURL = $"{_config.Value.Protocol}://{_config.Value.Host}:{_config.Value.Port}/api/emissions/latest";

            using (HttpClient client = NewClient())
            {
                var stringTask = client.GetStringAsync(apiURL);
                return Newtonsoft.Json.JsonConvert.DeserializeObject<List<EmissionDataDTO>>(await stringTask);
            }
        }

        private HttpClient NewClient()
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            return client;
        }

        public async Task UpdateEmissions(List<EmissionDataDTO> emissions)
        {
            string apiURL = $"{_config.Value.Protocol}://{_config.Value.Host}:{_config.Value.Port}/api/emissions";

            _logger.LogDebug($"Sending {emissions.Count} EmissionData elements to EnergyData API.");

            using (HttpClient client = NewClient())
            {
                var jsonRequest = Newtonsoft.Json.JsonConvert.SerializeObject(emissions);

                var content = new StringContent(jsonRequest,Encoding.UTF8,"application/json");

                var apiResponse = await client.PostAsync( apiURL, content );

                if (!apiResponse.IsSuccessStatusCode)
                {
                    throw new Exception($"Failed to save data: {apiResponse.StatusCode} - {apiResponse.ReasonPhrase}");
                }
            }
        }

        public async Task UpdateEmissionsPrognosis(List<EmissionDataDTO> prognosis)
        {
            string apiURL = $"{_config.Value.Protocol}://{_config.Value.Host}:{_config.Value.Port}/api/prognosis";

            _logger.LogDebug($"Sending {prognosis.Count} Emission Prognosis data elements to EnergyData API.");

            using (HttpClient client = NewClient())
            {
                var jsonRequest = Newtonsoft.Json.JsonConvert.SerializeObject(prognosis);

                var content = new StringContent(jsonRequest,Encoding.UTF8,"application/json");

                var apiResponse = await client.PostAsync( apiURL, content );

                if (!apiResponse.IsSuccessStatusCode)
                {
                    throw new Exception($"Failed to save data: {apiResponse.StatusCode} - {apiResponse.ReasonPhrase}");
                }
            }
        }
    }
}