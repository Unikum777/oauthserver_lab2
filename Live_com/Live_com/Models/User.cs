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
    [Table(Name = "Users")]
    public class User
    {
        [DataMember]
        [Column(IsPrimaryKey = true)]
        public int Id { get; set; }
        [DataMember]
        [Column]
        public string Login { get; set; }
        [DataMember]
        [Column]
        public string Password { get; set; }
        [DataMember]
        [Column]
        public string Email { get; set; }
        [DataMember]
        [Column]
        public string Phone { get; set; }
        [DataMember]
        [Column]
        public int Permissions { get; set; }

        /**************************************/
        [DataMember]
        [Column]
        public string AuthorizationCode { get; set; }
        [DataMember]
        [Column]
        public string AccessToken { get; set; }
        [DataMember]
        [Column]
        public string RefreshToken { get; set; }
        [DataMember]
        [Column]
        public DateTime? ExpiredDateTime { get; set; }
        [DataMember]
        [Column]
        public string RedirectUri { get; set; }
        public string GenerateRandomString(int length)
        {
            var str = new Char[length];
            Random rnd = new Random();
            for (int i = 0; i < length; i++)
            {
                if (rnd.Next(2) == 0)
                {
                    str[i] = Convert.ToChar(rnd.Next(26) + 'A');
                }
                else
                {
                    str[i] = Convert.ToChar(rnd.Next(26) + 'a');
                }

            }
            return new string(str);
        }
    }
}