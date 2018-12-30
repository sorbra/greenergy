using System;
using System.ComponentModel.DataAnnotations;

namespace Greenergy.TeslaCharger.DTOModels
{    
    public class ChargingConstraintDTO
    {
        // Either weekDays or date should be set, but not both
        // If date is set, this constraint only applies on the set date, where i overrides other constraints.
        // If weekDays is set, this constraint applies to any week
        // If neither is set, this constraint applies to any day of any week, as if WeekDays was set to all days.
        public string[] WeekDays { get => weekDays; set => weekDays = value; }
        public DateTime? Date { get; set; }
        // The time of day when charging should be between minCharge and maxCharge
        [Required]
        public int ByHour  { get; set; }
        // The earliest time of day when charging should begin. Set to null if charging can start anytime
        [Required]
        public int? NoEarlierThanHour  { get; set; }
        // Minimum charging percentage to reach
        [Required]
        public int MinCharge  { get; set; }
        // Maximum charging percentage to reach
        [Required]
        public int MaxCharge  { get; set; }
        private string[] weekDays = { "Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun" };
    }
}