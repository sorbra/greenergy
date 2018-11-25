using System.Collections.Generic;
using Greenergy.Emissions.API.Client.Models;

namespace Greenergy.Emissions.MessageProviders
{
    public interface IPrognosisMessageProvider
    {
        void OnNewPrognosisData(List<EmissionDataDTO> prognoses);
    }

}