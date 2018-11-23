using System;
using System.Linq;
using System.Threading.Tasks;

namespace Greenergy.Tesla
{
    class Program
    {
        static async Task DoStuff(string email, string password)
        {
            TeslaOwner owner = new TeslaOwner(email);
            await owner.AuthenticateAsync(password);
            var vehicles = await owner.GetVehiclesAsync();
            
            var myTesla = vehicles.FirstOrDefault();
            
            var chargeState = await myTesla.GetChargeStateAsync();

            bool limitset = await myTesla.SetChargeLimit(60);

            bool charging = await myTesla.StartCharge();
            

            // var response = await hclient.GetAsync(client.BaseUri+$"/api/1/vehicles/{vehicle.Id}/data_request/charge_state");
            // var cstate = JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync());


            // .Data.Response.FirstOrDefault();
            // IChargeState chargeState = (await client.GetChargeStateAsync(vehicle.Id,token.AccessToken)).Data.Response;
        }


        static void Main(string[] args)
        {
            DoStuff(args[0],args[1]).Wait();
        }
    }
}
