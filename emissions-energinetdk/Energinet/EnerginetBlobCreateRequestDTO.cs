/* {"format":"json","filters":[{"name":"Minutes5DK","operator":">: ","value":"2018-10-18 21:29"}],"resource_id":"d856694b-5e0e-463b-acc4-d9d7d895128a"}
 */
using System;
using System.Collections.Generic;

namespace Greenergy.Energinet
{
    public class EnerginetResource
    {
        public const string Co2PrognosisBlobCreateURL = "https://www.energidataservice.dk/api/3/action/azure_blob_create";
        public const string Co2PrognosisResourceID = "d856694b-5e0e-463b-acc4-d9d7d895128a";
        public const string Co2EmissionsResourceID = "b5a8e0bc-44af-49d7-bb57-8f968f96932d";
    }

    public class EnerginetBlobCreateRequestDTO
    {
        public static EnerginetBlobCreateRequestDTO NewEmissionsRequest(DateTime noEarlierThan)
        {
            var request = new EnerginetBlobCreateRequestDTO();

            request.resource_id = EnerginetResource.Co2EmissionsResourceID;

            var filter = new Filter()
            {
                name = "Minutes5UTC",
                @operator = ">=",
                value = noEarlierThan.ToUniversalTime().ToString("yyyy-MM-dd HH:mm")
            };
            request.filters.Add(filter);

            return request;
        }

        public static EnerginetBlobCreateRequestDTO NewPrognosisRequest()
        {
            var request = new EnerginetBlobCreateRequestDTO();

            request.resource_id = EnerginetResource.Co2PrognosisResourceID;

            var filter = new Filter()
            {
                name = "Minutes5UTC",
                @operator = ">=",
                value = DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd HH:mm")
            };
            request.filters.Add(filter);

            return request;
        }

        public EnerginetBlobCreateRequestDTO()
        {
            format = "json";
            filters = new List<Filter>();
        }
        public List<Filter> filters { get; set; }
        public string format { get; set; }
        public string resource_id { get; set; }
        //        public string sort { get; set; }
    }

    public class Filter
    {
        public string name { get; set; }
        public string @operator { get; set; }
        public string value { get; set; }
        //        public int limit { get; set; }
    }


}
