using System;
using MongoDB.Bson;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Greenergy.Emissions.API.Server.Models.Mongo
{
    public class PrognosisDataMongo: EmissionDataMongo
    {
        public DateTime UpdatedTimeUTC { get; set; } = DateTime.UtcNow;

        public PrognosisDataMongo (int Emission, DateTime EmissionTimeUTC, string Region) 
                    : base (Emission, EmissionTimeUTC, Region)
        {
        }
    }
}