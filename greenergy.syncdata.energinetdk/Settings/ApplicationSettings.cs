using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Greenergy.Settings
{
    public class ApplicationSettings
    {
        public string Name { get; set; }

        public string UserAgent { get; set; }

        public URLSettings EnergyDataAPI { get; set; }

        public int UpdateDelayInMinutes { get; set; } = 5;

        // The earliest DateTime to get energinet data from
        // when bootstrapping the database
        public DateTime BootstrapDate { get; set; }
    }

    public class URLSettings
    {
        public string Protocol { get; set; }
        public string Host { get; set; }
        public string Port { get; set; }
    }
}