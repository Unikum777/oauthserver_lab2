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
    [Table(Name = "Folders")]
    public class Folder
    {
        [DataMember]
        [Column(IsPrimaryKey = true)]
        public int Id { get; set; }
        [DataMember]
        [Column]
        public string FolderName { get; set; }
        [DataMember]
        [Column]
        public string ExternalId { get; set; }
        [DataMember]
        [Column]
        public int UserId { get; set; }
    }
}