using System;
using MongoDB.Bson;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Greenergy.Models
{
    public class PrognosisDataMongo: EmissionDataMongo
    {
        public PrognosisDataMongo (int Emission, DateTime TimeStampUTC, string Region) 
                    : base (Emission, TimeStampUTC, Region)
        {
        }
    }
}