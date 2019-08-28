using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SecureParcel.Classes.Database
{
    [Table(name: "Parcels")]
    public class Parcel
    {
        [Key]
        public int ID { get; set; }
        public string GUID { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ApplicationUser Sender { get; set; }
        public string SenderAddress { get; set; }
        public string SenderName { get; set; }
        public string RecipientPublicKey { get; set; }
        public string RecipientAddress { get; set; }
        public string RecipientName { get; set; }
        public string SafeAccount { get; set; }
        public string PaymentAmount { get; set; }
        public string PaymentDate { get; set; }
        public string ShipmentDate { get; set; }
        public string TrackNumber { get; set; }
        public string Comment { get; set; }
        public DeliveryStatusEnum DeliveryStatus { get; set; }
        public List<Message> Messages { get; set; }
        public List<TrackEvent> TrackEvents { get; set; }
    }

    public enum DeliveryStatusEnum
    {
        [Description("Awaiting payment")]
        AwaitingPayment = 1,

        [Description("Preparation for shipment")]
        PreparationForShipment = 2,

        [Description("Parcel sent")]
        ParcelSent = 3,

        [Description("Awaiting receipt")]
        AwaitingReceipt = 4,

        [Description("Received")]
        Received = 5
    }
}