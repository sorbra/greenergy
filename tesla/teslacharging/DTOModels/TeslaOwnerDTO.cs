using System;
using MongoDB.Bson;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Greenergy.TeslaCharger.DTOModels
{
    public class TeslaOwnerDTO
    {
        public TeslaOwnerDTO(string Email)
        {
            this.Email = Email;
        }
        public TeslaOwnerDTO()
        {
        }
        [Required]
        public string Email { get; set; }
        [Required]
        public string AccessToken { get; set; }
        [Required]
        public List<TeslaVehicleDTO> Vehicles;
    }

    public class TeslaVehicleDTO
    {
        public string VIN { get; set; }
        public string DisplayName { get; set; }
        public List<ChargingConstraintDTO> ChargingConstraints { get; set; }
    }

    public class ChargingConstraintDTO
    {

    }
}