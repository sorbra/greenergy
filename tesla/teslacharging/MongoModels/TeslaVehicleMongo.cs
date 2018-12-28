using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Greenergy.TeslaCharger.MongoModels
{
    public class TeslaVehicleMongo
    {
        public string Id { get; set; }
        public string VIN { get; set; }
        public string DisplayName { get; set; }

        public List<ChargingConstraintMongo> chargingConstraints;
    }
}