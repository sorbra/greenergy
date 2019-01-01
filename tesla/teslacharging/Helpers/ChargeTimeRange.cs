using System;
using System.Collections.Generic;
using System.Linq;
using Greenergy.TeslaCharger.MongoModels;

namespace Greenergy.TeslaCharger.Constraints
{
    public class ChargeTimeRange
    {
        public ChargingConstraintMongo Constraint;
        public DateTime ChargeNoEarlierThan;
        public DateTime ChargeBy;

        public static int DaysToNextDayOfWeek(DayOfWeek day, DayOfWeek[] cdays)
        {
            return cdays.ToList()
                        .ConvertAll(cd => (cd < day ? (int)cd + 7 - (int)day : (int)cd - (int)day))
                        .OrderBy(ci => ci)
                        .First();
        }
        public static ChargeTimeRange NextChargeBy(List<ChargingConstraintMongo> constraints)
        {
            var utcNow = DateTime.UtcNow;
            ChargingConstraintMongo nextConstraint = null;
            double hoursToNextChargeBy = Double.MaxValue;

            DateTime chargeNoEarlierThan = utcNow;
            DateTime chargeBy = DateTime.MaxValue;

            foreach (var c in constraints)
            {
                TimeZoneInfo ctz = c.GetTimeZone();
                TimeSpan offset = ctz.GetUtcOffset(utcNow);
                var ctzNow = utcNow.AddHours(offset.Hours).AddMinutes(offset.Minutes);

                var ctzNowDate = ctzNow.Date;
                if (ctzNow.Hour >= c.ByHour)
                {
                    ctzNowDate = ctzNowDate.AddDays(1);
                }
                var ctzNextDate = ctzNowDate.AddDays(DaysToNextDayOfWeek(ctzNowDate.DayOfWeek, c.WeekDays));
                var cChargeBy = new DateTime(ctzNextDate.Year, ctzNextDate.Month, ctzNextDate.Day, c.ByHour, 0, 0);

                var span = cChargeBy - ctzNow;
                if (span.TotalHours < hoursToNextChargeBy)
                {
                    hoursToNextChargeBy = span.TotalHours;
                    nextConstraint = c;
                    chargeBy = cChargeBy;
                }
            }

            if (nextConstraint.NoEarlierThanHour.HasValue)
            {
                chargeNoEarlierThan = chargeBy.AddHours(nextConstraint.NoEarlierThanHour.Value - nextConstraint.ByHour);
            }

            var targetChargeTime = new ChargeTimeRange()
            {
                Constraint = nextConstraint,
                ChargeBy = chargeBy.ToUniversalTime(),
                ChargeNoEarlierThan = chargeNoEarlierThan.ToUniversalTime()
            };

            return targetChargeTime;
        }
    }
}