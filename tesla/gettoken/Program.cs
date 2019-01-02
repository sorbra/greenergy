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

        static string ReadPassword()
        {            
            string password = "";
            do
            {
                ConsoleKeyInfo key = Console.ReadKey(true);
                // Backspace Should Not Work
                if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
                {
                    password += key.KeyChar;
                    Console.Write("*");
                }
                else
                {
                    if (key.Key == ConsoleKey.Backspace && password.Length > 0)
                    {
                        password = password.Substring(0, (password.Length - 1));
                        Console.Write("\b \b");
                    }
                    else if (key.Key == ConsoleKey.Enter)
                    {
                        Console.WriteLine();
                        return password;
                    }
                }
            } while (true);
        }

        static void Main(string[] args)
        {
            Console.Write("Email: ");
            var email = Console.ReadLine();

            Console.Write("Password: ");
            var password = ReadPassword();

            string AccessToken = GetToken(email, password).Result;

            Console.WriteLine($"AccessToken: {AccessToken}");
        }
    }
}
