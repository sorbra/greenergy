using System;
using MongoDB.Bson;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Greenergy.TeslaCharger.DTOModels
{
    public class TeslaVehicleDTO
    {
        [Required]
        public string OwnerEmail { get; set; }
        public string AccessToken { get; set; }
        [Required]
        public string VIN { get; set; }
        public string DisplayName { get; set; }
        [Required]
        public List<ChargingConstraintDTO> ChargingConstraints { get; set; }
    }
}