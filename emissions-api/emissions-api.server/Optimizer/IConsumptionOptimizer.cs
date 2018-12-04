using System.Collections.Generic;
using System.Threading.Tasks;
using Greenergy.Emissions.API.Client.Models;

namespace Greenergy.Emissions.Optimization
{
    public interface IConsumptionOptimizer
    {
        List<List<OptimalConsumptionPrognosis>> SuggestConsumption(List<EmissionDataDTO> prognoses);
        OptimalConsumptionPrognosis SuggestConsumption(string region, int hours, List<EmissionDataDTO> prognoses);
    }
}