using System.Collections.Generic;
using System.Threading.Tasks;
using Greenergy.Emissions.API.Client.Models;
using Greenergy.Emissions.Optimization;

namespace Greenergy.Emissions.Messaging
{
    public interface IPrognosisMessageSink
    {
        Task SendPrognoses(List<RegionalConsumptionPrognoses> regionalInfos);
    }
}