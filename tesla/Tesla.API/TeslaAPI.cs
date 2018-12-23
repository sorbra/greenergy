using System.Net.Http;

namespace Greenergy.Tesla
{
    public class TeslaAPI
    {
        static public string TESLA_CLIENT_ID = "81527cff06843c8634fdc09e8ac0abefb46ac849f38fe1e431c2ef2106796384";
        static public string TESLA_CLIENT_SECRET = "c7257eb71a564034f9419ee651c7d0e5f7aa6bfbd18bafb5c5c033b093bb2fa3";
        static public string OWNER_API_BASE_URI = "https://owner-api.teslamotors.com";
        static public string OAUTH_TOKEN_PATH = OWNER_API_BASE_URI + "/oauth/token";
        static public string VEHICLES_API_BASE_URI = OWNER_API_BASE_URI + "/api/1/vehicles";
        static public string CHARGESTATE_PATH = VEHICLES_API_BASE_URI + "/{id}/data_request/charge_state";
        static public string COMMAND_PATH = VEHICLES_API_BASE_URI + "/{id}/command/{command}";
        static public HttpClient GetClient()
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "Greenergy");
            return client;
        }
    }
}