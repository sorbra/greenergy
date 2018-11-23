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
        public DateTime BootstrapDate { get; set; }
        public int EmissionsSyncRateInMinutes { get; internal set; } = 5;
        public int PrognosisSyncRateInMinutes { get; internal set; } = 5;
    }
}