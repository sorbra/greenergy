using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Greenergy.TeslaCharger.MongoModels
{
    public class ChargingConstraintMongo
    {
        // Either weekDays or date should be set, but not both
        // If date is set, this constraint only applies on the set date, where it overrides other constraints.
        // If weekDays is set, this constraint applies to any week
        // If neither is set, this constraint applies to any day of any week, as if WeekDays was set to all days.
        public DayOfWeek[] WeekDays { get; set; }
        public string Date { get; set; }
        // The time of day when charging should be between minCharge and maxCharge
        [Required]
        public int ByHour  { get; set; }
        // The earliest time of day when charging should begin. Set to null if charging can start anytime
        public int? NoEarlierThanHour  { get; set; }
        // The timezone to apply to ByHour and NoEarlierThanHour
        public string TimeZone { get; set; }
        // Minimum charging percentage to reach
        public int MinCharge  { get; set; }
        // Maximum charging percentage to reach
        public int MaxCharge  { get; set; }
        public TimeZoneInfo GetTimeZone ()
        {
            return TimeZoneInfo.FindSystemTimeZoneById(TimeZone);
        }
        internal void Validate()
        {
            if (Date == null)
            {
                if (WeekDays == null) throw new Exception("ChargingConstraint.WeekDays or ChargingConstraint.Date must be set");
                if (WeekDays.Length == 0) throw new Exception("ChargingConstraint.WeekDays must contain at least one day of week");
            }
            if (Date != null && WeekDays != null)
            {
                throw new Exception("Either ChargingConstraint.WeekDays or ChargingConstraint.Date should be set, but not both");
            }
        }
    }
}