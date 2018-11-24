using System;

namespace Greenergy.Emissions.API.Client.Models
{
    public class PrognosisDataDTO: EmissionDataDTO
    {
        public DateTimeOffset UpdatedOn { get; set; } = DateTimeOffset.Now;

        public PrognosisDataDTO (int Emission, DateTimeOffset EmissionTimeUTC, string Region) 
                    : base (Emission, EmissionTimeUTC, Region)
        {
        }
    }
}