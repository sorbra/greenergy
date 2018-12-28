using System;
using System.Net.Http;
using Greenergy.Emissions.API;

namespace testclient
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var httpClient = new HttpClient();
            var emissionsClient = new EmissionsClient(httpClient);
            emissionsClient.BaseUrl = "http://localhost:5000/";

            var emissions = emissionsClient.GetMostRecentEmissionsAsync().Result;

            foreach (var emission in emissions) 
            {
                System.Console.WriteLine( $"{emission.Region}: {emission.EmissionTimeUTC}, {emission.Emission}" ); 
            }

            var prognosisClient = new PrognosisClient(httpClient);
            prognosisClient.BaseUrl = "http://localhost:5000/";

            var consumptionRecommendation = prognosisClient.OptimalConsumptionTimeAsync("DK1",2,DateTimeOffset.UtcNow.ToString("o"),DateTimeOffset.UtcNow.AddDays(1).ToString("o")).Result;

            System.Console.WriteLine($"{consumptionRecommendation.Best.Emissions}g at {consumptionRecommendation.Best.StartUTC.ToLocalTime().ToString("o")}");
            System.Console.WriteLine($"{consumptionRecommendation.Earliest.Emissions}g at {consumptionRecommendation.Earliest.StartUTC.ToLocalTime().ToString("o")}");
            System.Console.WriteLine($"{consumptionRecommendation.Latest.Emissions}g at {consumptionRecommendation.Latest.StartUTC.ToLocalTime().ToString("o")}");
        }
    }
}
