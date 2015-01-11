using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;
using System.Data.Linq;
using System.Data.Linq.Mapping;


namespace Live_com.Models
{
    [DataContract]
    [Table(Name = "Files")]
    public class File
    {
        public File() { }
        public File(string ExternalId, string FileName, int size) 
        {
            this.ExternalId = ExternalId;
            this.FileName = FileName;
            this.Size = size;
            Access = 0;
        }
        [Required]
        [StringLength(100, ErrorMessage = "File name is soo long (> 100 symbols)")]
        [DataMember]
        [Column]
        public string FileName { get; set; }
        [DataMember]
        [Column(IsPrimaryKey = true)]
        public int Id { get; set; }
        [Required]
        [DataMember]
        [Column]
        public string ExternalId { get; set; }
        
        [Required]
        [DataMember]
        [Column]
        public int Size { get; set; }
        [Required]
        [DataMember]
        [Column]
        public int Access { get; set; } //true = publuc, false = private
        [Required]
        [DataMember]
        [Column]
        public int UserId { get; set; }
        [Required]
        [DataMember]
        [Column]
        public int FolderId { get; set; }
    }
}