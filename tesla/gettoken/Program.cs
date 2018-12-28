using System;
using System.Linq;
using System.Threading.Tasks;
using Greenergy.TeslaTools;

namespace Greenergy.TeslaToken
{
    class Program
    {
        static async Task<string> GetToken(string email, string password)
        {
            TeslaOwner owner = new TeslaOwner(email);
            var AccessToken = await owner.AuthenticateAsync(password);
            return AccessToken;
        }

        static void Main(string[] args)
        {
            string AccessToken = GetToken(args[0],args[1]).Result;

            Console.WriteLine(AccessToken);
        }
    }
}
