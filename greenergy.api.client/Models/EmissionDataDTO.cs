using System;

namespace Greenergy.API.Models
{
    public class EmissionDataDTO
    {
        public EmissionDataDTO (int Emission, DateTime TimeStampUTC, string Region)
        {
            this.Emission = Emission;
            this.TimeStampUTC = TimeStampUTC;
            this.Region = Region;
        }
        public EmissionDataDTO()
        {
        }

        public int Emission { get; set; }

        public DateTime TimeStampUTC { get; set; }
        
        public string Region { get; set; }      
        public DateTime CreatedOn { get; set; } = DateTime.Now;
    }
}