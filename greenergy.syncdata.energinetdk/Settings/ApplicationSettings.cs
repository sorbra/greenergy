using Newtonsoft.Json;
using System.Collections.Generic;

namespace Greenergy.Settings
{
    public class ApplicationSettings
    {
        public string Name { get; set; }

        public string UserAgent { get; set; }

        public URLSettings EnergyDataAPI { get; set; }

        public int UpdateDelayInMinutes { get; set; } = 5;
    }

    public class URLSettings
    {
        public string Protocol { get; set; }
        public string Host { get; set; }
        public string Port { get; set; }
    }
}