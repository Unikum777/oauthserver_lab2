using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace Live_com.Models
{
    [DataContract]
    [Table(Name = "Applications")]
    public class Apps
    {
        [DataMember]
        [Column(IsPrimaryKey = true)]
        public int ClientId { get; set; }
        [DataMember]
        [Column]
        public string ClientSecret { get; set; }
        [DataMember]
        [Column]
        public string RedirectUri { get; set; }
        [DataMember]
        [Column]
        public string State { get; set; }
        public string GenerateRandomString(int length)
        {
            var str = new Char[length];
            Random rnd = new Random();
            for (int i = 0; i < length; i++)
            {
                str[i] = Convert.ToChar(rnd.Next(52) + 46);
            }
            return str.ToString();

        }
    }
}