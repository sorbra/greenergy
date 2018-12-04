using System.Collections.Generic;
using System.Threading.Tasks;
using Greenergy.Emissions.API.Client.Models;

namespace Greenergy.Emissions.Messaging
{
    public interface IPrognosisMessageSink
    {
        Task SendPrognoses(List<List<OptimalConsumptionPrognosis>> regionalInfos);
    }
}