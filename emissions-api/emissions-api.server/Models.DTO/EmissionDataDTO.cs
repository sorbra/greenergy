using System;

namespace Greenergy.Emissions.API.Client.Models
{
    public class EmissionDataDTO
    {
        public EmissionDataDTO (int Emission, DateTimeOffset EmissionTimeUTC, string Region)
        {
            this.Emission = Emission;
            this.EmissionTimeUTC = EmissionTimeUTC;
            this.Region = Region;
        }
        public EmissionDataDTO()
        {
        }

        public int Emission { get; set; }

        public DateTimeOffset EmissionTimeUTC { get; set; }
        
        public string Region { get; set; }      
        public DateTimeOffset RecordedTimeUTC { get; set; } = DateTimeOffset.Now;
    }
}