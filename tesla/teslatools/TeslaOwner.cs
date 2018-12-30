using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Greenergy.Tesla.API;

namespace Greenergy.TeslaTools
{
    public class TeslaOwner
    {
        public string Email { get; set; }
        string AccessToken { get; set; }
        public HttpClient Client { get; set; }
        public TeslaOwner(string email)
        {
            Email = email;
            Client = TeslaAPI.GetClient();
        }
        public TeslaOwner(string email, string accessToken) : this(email)
        {
            AccessToken = accessToken;
            Client.DefaultRequestHeaders.Add("Authorization", "Bearer " + AccessToken);
        }
        public async Task<string> AuthenticateAsync(string password)
        {
            var request = JsonConvert.SerializeObject(new
            {
                grant_type = "password",
                client_id = TeslaAPI.TESLA_CLIENT_ID,
                client_secret = TeslaAPI.TESLA_CLIENT_SECRET,
                email = Email,
                password
            });

            var response = await Client.PostAsync(
                TeslaAPI.OAUTH_TOKEN_PATH,
                new StringContent(
                        request,
                        Encoding.UTF8,
                        "application/json"
            ));

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception($"Tesla login failed. StatusCode={response.StatusCode}, Reason={response.ReasonPhrase}");
            }

            var token = JsonConvert.DeserializeObject<TeslaTokenDTO>(
                await response.Content.ReadAsStringAsync()
            );

            AccessToken = token.AccessToken;

            Client.DefaultRequestHeaders.Add("Authorization", "Bearer " + AccessToken);

            return AccessToken;

            // System.Console.WriteLine($"Got token {AccessToken}. Expires on {TokenExpiry.ToString("o")}");
        }

        public async Task<List<TeslaVehicle>> GetVehiclesAsync()
        {
            var response = await Client.GetAsync(TeslaAPI.VEHICLES_API_BASE_URI);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception($"Failed to get Tesla Vehicles. StatusCode={response.StatusCode}, Reason={response.ReasonPhrase}");
            }

            var vehiclesDTO = JsonConvert.DeserializeObject<TeslaVehiclesResponseDTO>(
                await response.Content.ReadAsStringAsync()
            );

            var vehicles = vehiclesDTO.Vehicles
                .Select(v => new TeslaVehicle
                {
                    Owner = this,
                    Id = v.Id,
                    VIN = v.VIN,
                    DisplayName = v.DisplayName,
                    InService = v.InService,
                    State = v.State
                })
                .ToList();

            return vehicles;
        }
    }
}