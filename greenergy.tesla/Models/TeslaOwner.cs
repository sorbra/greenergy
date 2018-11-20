using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Greenergy.Tesla
{
    public class TeslaOwner
    {
        string Email;
        string AccessToken;
        DateTime TokenExpiry;
        public HttpClient Client { get; set; }
        public TeslaOwner(string email)
        {
            Email = email;
            Client = TeslaAPI.GetClient();
            TokenExpiry = DateTime.MinValue;
        }
        public async Task AuthenticateAsync(string password)
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
            TokenExpiry = token.ExpiresAt;

            Client.DefaultRequestHeaders.Add("Authorization","Bearer " + AccessToken);

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
                .Select( v => new TeslaVehicle {
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