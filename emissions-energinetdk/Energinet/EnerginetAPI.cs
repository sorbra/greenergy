using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using Greenergy.Settings;
using Microsoft.Extensions.Options;
using Greenergy.Emissions.API;

namespace Greenergy.Energinet
{
    public class EnerginetAPI : IEnerginetAPI
    {
        private IOptions<ApplicationSettings> _config;

        public EnerginetAPI(IOptions<ApplicationSettings> config)
        {
            _config = config;
        }
        public async Task<List<EmissionDataDTO>> GetRecentEmissions(DateTimeOffset noEarlierThan)
        {

            var request = EnerginetBlobCreateRequestDTO.NewEmissionsRequest(noEarlierThan);
            return await EnerginetGet(request);
        }
        
        public async Task<List<EmissionDataDTO>> GetCurrentEmissionsPrognosis()
        {
            var request = EnerginetBlobCreateRequestDTO.NewPrognosisRequest();
            return await EnerginetGet(request);
        }

        public async Task<List<EmissionDataDTO>> EnerginetGet(EnerginetBlobCreateRequestDTO request)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("User-Agent", _config.Value.UserAgent);

                var jsonRequest = Newtonsoft.Json.JsonConvert.SerializeObject(request);

                var blobCreateResponse = await client.PostAsync(EnerginetResource.Co2PrognosisBlobCreateURL, new StringContent(jsonRequest));

                if (blobCreateResponse.IsSuccessStatusCode)
                {
                    var blobCreateJsonResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<EnerginetBlobCreateResponseDTO>(await blobCreateResponse.Content.ReadAsStringAsync());

                    var emissionsResponse = await client.GetAsync(blobCreateJsonResponse.result);
                    if (emissionsResponse.IsSuccessStatusCode)
                    {
                        var emissionsJsonResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<EnerginetEmissionsResponseDTO>(await emissionsResponse.Content.ReadAsStringAsync());

                        var results = new List<EmissionDataDTO>();
                        foreach (var record in emissionsJsonResponse.records)
                        {
                            results.Add(
                                new EmissionDataDTO() {
                                    Emission = Int32.Parse(record[3]),
                                    EmissionTimeUTC = DateTimeOffset.Parse(record[0]),
                                    Region = record[2]
                            });
                        }
                        return results;
                    }
                }
                return null;
            }
        }
    }
}