using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Greenergy.TeslaCharger
{
    public class ApplicationSettings
    {
        public string Name { get; set; }

        public int ChargingCheckRateInMinutes { get; internal set; } = 5;
    }
}