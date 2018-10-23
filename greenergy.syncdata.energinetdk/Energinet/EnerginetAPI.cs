using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using Greenergy.Models;

namespace Greenergy.Energinet
{
    public class EnerginetAPI
    {
        public static async Task<List<EmissionData>> GetRecentEmissions(DateTime noEarlierThan)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("User-Agent", "Greenergy Data Synchronizer");

                var jsonRequest = Newtonsoft.Json.JsonConvert.SerializeObject(EnerginetBlobCreateRequestDTO.NewEmissionsRequest(noEarlierThan));


                var blobCreateResponse = await client.PostAsync(EnerginetResource.Co2PrognosisBlobCreateURL, new StringContent(jsonRequest));

                if (blobCreateResponse.IsSuccessStatusCode)
                {
                    var blobCreateJsonResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<EnerginetBlobCreateResponseDTO>(await blobCreateResponse.Content.ReadAsStringAsync());

                    var emissionsResponse = await client.GetAsync(blobCreateJsonResponse.result);
                    if (emissionsResponse.IsSuccessStatusCode)
                    {
                        var emissionsJsonResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<EnerginetEmissionsResponseDTO>(await emissionsResponse.Content.ReadAsStringAsync());

                        var results = new List<EmissionData>();
                        foreach (var record in emissionsJsonResponse.records)
                        {
                            results.Add(
                                new EmissionData(
                                    Emission: Int32.Parse(record[3]),
                                    TimeStampUTC: DateTime.Parse(record[0]),
                                    Region: record[2]
                            ));
                        }
                        return results;
                    }
                }
                return null;
            }
        }
    }
}