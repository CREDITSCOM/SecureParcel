using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SecureParcel.Classes.Database
{
    [Table(name: "Messages")]
    public class Message
    {
        [Key]
        public int ID { get; set; }
        public DateTime Date { get; set; }
        public string Text { get; set; }
        public Parcel Parcel { get; set; }
        public ApplicationUser User { get; set; }
    }
}