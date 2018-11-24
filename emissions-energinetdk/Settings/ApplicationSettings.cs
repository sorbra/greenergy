using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Greenergy.Settings
{
    public class ApplicationSettings
    {
        public string Name { get; set; }

        public string UserAgent { get; set; }

        // The earliest DateTime to get energinet data from
        // when bootstrapping the database
        public DateTimeOffset BootstrapDate { get; set; }
        public int EmissionsSyncRateInMinutes { get; internal set; } = 5;
        public int PrognosisSyncRateInMinutes { get; internal set; } = 5;
        public string EmissionsServiceBaseURL { get; set; }
        public int RunEveryMinutes { get; internal set; } = 5;
        public string TimeZone { get; internal set; }
    }
}