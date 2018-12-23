using System.Collections.Generic;
using System.Threading.Tasks;
using Greenergy.Emissions.API.Client.Models;

namespace Greenergy.Emissions.Optimization
{
    public interface IConsumptionOptimizer
    {
        List<RegionalConsumptionPrognoses> SuggestConsumption(List<EmissionDataDTO> prognoses);
        OptimalConsumptionPrognosis SuggestConsumption(string region, int hours, List<EmissionDataDTO> prognoses);
    }

    public class RegionalConsumptionPrognoses
    {
        public string Region { get; set; }
        public List<OptimalConsumptionPrognosis> Prognoses { get; set; }
    }
}