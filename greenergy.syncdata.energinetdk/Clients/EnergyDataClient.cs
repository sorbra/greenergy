using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Greenergy.Models;
using Greenergy.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Greenergy.Clients
{
    public class EnergyDataClient : IEnergyDataClient
    {
        private IOptions<ApplicationSettings> _config;
        private ILogger<EnergyDataClient> _logger;

        public EnergyDataClient(
            IOptions<ApplicationSettings> config,
            ILogger<EnergyDataClient> logger)
        {
            _config = config;
            _logger = logger;
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
        public async Task<List<EmissionData>> GetLatest()
        {
            string apiURL = $"{_config.Value.EnergyDataAPI.Protocol}://{_config.Value.EnergyDataAPI.Host}:{_config.Value.EnergyDataAPI.Port}/api/emissions/latest";

            using (HttpClient client = NewClient())
            {
                var stringTask = client.GetStringAsync(apiURL);
                var json = await stringTask;

                var emissions = Newtonsoft.Json.JsonConvert.DeserializeObject<List<EmissionData>>(json);

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
            client.DefaultRequestHeaders.Add("User-Agent", _config.Value.UserAgent);
            return client;
        }

        public async Task UpdateEmissionData(List<EmissionData> emissions)
        {
            string apiURL = $"{_config.Value.EnergyDataAPI.Protocol}://{_config.Value.EnergyDataAPI.Host}:{_config.Value.EnergyDataAPI.Port}/api/emissions";

            _logger.LogInformation($"Sending {emissions.Count} EmissionData elements to EnergyData API.");

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("User-Agent", _config.Value.UserAgent);
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("*/*")
                );

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