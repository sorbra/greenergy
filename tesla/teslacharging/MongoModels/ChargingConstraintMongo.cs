using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Greenergy.TeslaCharger.MongoModels
{
    public class ChargingConstraintMongo
    {
        // The time of day when charging should at least be at minCharge
        int byHour;
        // The earliest time of day when charging should begin. Set to null if charging can start anytime
        int? noEarlierThanHour;
        // Minimum charging percentage to reach
        int minCharge;
        // Maximum charging percentage to reach
        int maxCharge;
    }
    public class NormalCaseConstraintMongo : ChargingConstraintMongo
    {
        // Days of week where this Normal Case Constraint applies
        DayOfWeek[] weekDays;
    }
    public class SpecificCaseConstraintMongo : ChargingConstraintMongo
    {
        // The date where this specific constraint applies
        DateTime date;
    }
}