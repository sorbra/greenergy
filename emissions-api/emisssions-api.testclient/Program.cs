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
                System.Console.WriteLine( $"{emission.Region}: {emission.TimeStampUTC}, {emission.Emission}" ); 
            }

            var prognosisClient = new PrognosisClient(httpClient);
            prognosisClient.BaseUrl = "http://localhost:5000/";

            var consumptionRecommendation = prognosisClient.OptimalConsumptionTimeAsync(120,"DK1",null,null).Result;

            System.Console.WriteLine($"{consumptionRecommendation.OptimalEmissions}g at {consumptionRecommendation.OptimalConsumptionStartUTC.ToLocalTime().ToString("o")}");
            System.Console.WriteLine($"{consumptionRecommendation.FirstEmissions}g at {consumptionRecommendation.FirstConsumptionStartUTC.ToLocalTime().ToString("o")}");
            System.Console.WriteLine($"{consumptionRecommendation.LastEmissions}g at {consumptionRecommendation.LastConsumptionStartUTC.ToLocalTime().ToString("o")}");
        }
    }
}
