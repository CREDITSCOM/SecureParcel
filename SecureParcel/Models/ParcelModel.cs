using SecureParcel.Classes.Database;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SecureParcel.Models
{
    public class ParcelModel
    {
        public string GUID { get; set; }
        public string CreatedAt { get; set; }
        public List<TableItem> ParcelList { get; set; }
        public ParcelCard Parcel { get; set; }
        public SenderCard Sender { get; set; }
        public RecipientCard Recipient { get; set; }
        public PaymentCard Payment { get; set; }
        public DeliveryCard Delivery { get; set; }
        public List<TrackEvent> TrackEventList { get; set; }
        public List<Message> MessageList { get; set; }
        public string Comment { get; set; }


        public class TableItem
        {
            public string GUID { get; set; }
            public string ParcelName { get; set; }
            public string Amount { get; set; }
            public DeliveryStatusEnum Status { get; set; }
            public string SenderName { get; set; }
            public string RecipientName { get; set; }
            public DateTime CreatedAt { get; set; }
        }

        public class ParcelCard
        {
            public string Name { get; set; }
            public string Description { get; set; }
        }

        public class SenderCard
        {
            [Required]
            [Display(Name = "Public key")]
            public string PublicKey { get; set; }

            [Required]
            [Display(Name = "Address")]
            public string Address { get; set; }

            [Required]
            [Display(Name = "Full name")]
            public string Name { get; set; }
        }

        public class RecipientCard
        {
            [Required]
            [Display(Name = "Public key")]
            public string PublicKey { get; set; }

            [Required]
            [Display(Name = "Address")]
            public string Address { get; set; }

            [Required]
            [Display(Name = "Full name")]
            public string Name { get; set; }
        }

        public class PaymentCard
        {
            [Required]
            [Display(Name = "Amount CS")]
            public string Amount { get; set; }

            [Display(Name = "Date")]
            public string Date { get; set; }

            [Required]
            [Display(Name = "Safe account number")]
            public string SafeAccount { get; set; }
        }

        public class DeliveryCard
        {
            [Required]
            [Display(Name = "Track number")]
            public string TrackNumber { get; set; }

            [Display(Name = "Shipment date")]
            public string ShipmentDate { get; set; }

            [Display(Name = "Status")]
            public string Status { get; set; }
        }

        public ParcelModel()
        {
            ParcelList = new List<TableItem>();
            MessageList = new List<Message>();
            TrackEventList = new List<TrackEvent>();
            Parcel = new ParcelCard();
            Sender = new SenderCard();
            Recipient = new RecipientCard();
            Payment = new PaymentCard();
            Delivery = new DeliveryCard();
        }
    }
}