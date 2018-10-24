using System;

namespace Greenergy.API
{
    public class EmissionDataDTO
    {
        public EmissionDataDTO (int Emission, DateTime TimeStampUTC, string Region)
        {
            this.Emission = Emission;
            this.TimeStampUTC = TimeStampUTC;
            this.Region = Region;
        }

        public int Emission { get; set; }

        public DateTime TimeStampUTC { get; set; }
        
        public string Region { get; set; }      
    }
}