using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Newtonsoft.Json;


// {
//     "internalId": "5bcf24af4d667cd9c8dad036",
//     "emission": 53,
//     "timeStampUTC": "2018-10-23T13:35:00Z",
//     "region": "DK2",
//     "createdOn": "2018-10-23T13:39:59.023Z"
// }

namespace Greenergy.Models
{
    [DataContract]
    public class EmissionData
    {
        public EmissionData (int Emission, DateTime TimeStampUTC, string Region)
        {
            this.Emission = Emission;
            this.TimeStampUTC = TimeStampUTC;
            this.Region = Region;
        }

        [DataMember(Name = "emission")]
        public int Emission { get; set; }

        [DataMember(Name = "timeStampUTC")]
        public DateTime TimeStampUTC { get; set; }
        
        [DataMember(Name = "region")]
        public string Region { get; set; }      
    }
}