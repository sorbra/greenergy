using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Runtime.Serialization.Json;

namespace Greenergy.API
{
    public class GreenergyAPIClient : IGreenergyAPIClient
    {
        private IOptions<GreenergyAPISettings> _config;
        private ILogger<GreenergyAPIClient> _logger;

        public GreenergyAPIClient(
            IOptions<GreenergyAPISettings> config,
            ILogger<GreenergyAPIClient> logger)
        {
            _config = config;
            _logger = logger;

            string apiURL = $"{_config.Value.Protocol}://{_config.Value.Host}:{_config.Value.Port}/api/emissions/latest";
            _logger.LogInformation("GreenergyAPIClient constructor: " + apiURL);

        }

        public async Task<DateTime> GetLatestTimeStamp()
        {
            var latestEmissions = await GetLatest();
            if (latestEmissions != null && latestEmissions.Count > 0)
            {
                return latestEmissions[0].TimeStampUTC;
            }
            else
            {
                return DateTime.MinValue;
            }
        }
        public async Task<List<EmissionDataDTO>> GetLatest()
        {
            string apiURL = $"{_config.Value.Protocol}://{_config.Value.Host}:{_config.Value.Port}/api/emissions/latest";

            using (HttpClient client = NewClient())
            {
                var stringTask = client.GetStringAsync(apiURL);
                var json = await stringTask;

                var emissions = Newtonsoft.Json.JsonConvert.DeserializeObject<List<EmissionDataDTO>>(json);

                //var serializer = new DataContractJsonSerializer(typeof(List<EmissionData>));

                // var emissions = serializer.ReadObject(await streamTask) as List<EmissionData>;
                return emissions;
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

        public async Task UpdateEmissionData(List<EmissionDataDTO> emissions)
        {
            string apiURL = $"{_config.Value.Protocol}://{_config.Value.Host}:{_config.Value.Port}/api/emissions";

            _logger.LogInformation($"Sending {emissions.Count} EmissionData elements to EnergyData API.");

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
    }
}