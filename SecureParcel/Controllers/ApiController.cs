using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using SecureParcel.Classes.Database;
using SecureParcel.Models;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SecureParcel.Controllers
{
    public class ApiController : Controller
    {
        public JsonResult Get(string GUID)
        {
            var response = new JsonResponse();
            
            try
            {
                var identity = HttpContext.GetOwinContext().Authentication.GetExternalIdentity(DefaultAuthenticationTypes.ApplicationCookie);

                if (identity == null || identity.Name == null)
                {
                    response.IsSuccess = false;
                    response.Message = "Authorization error!";
                }
                else
                {
                    using (var dbContext = new DatabaseContext())
                    {
                        dbContext.Configuration.LazyLoadingEnabled = false;

                        var user = dbContext.Users.FirstOrDefault(x => x.Email == identity.Name);

                        if (user == null)
                        {
                            response.IsSuccess = false;
                            response.Message = "User not found!";
                        }
                        else if (!user.IsActivated)
                        {
                            response.IsSuccess = false;
                            response.Message = "User not activated!";
                        }
                        else
                        {
                            var parcel = dbContext.Parcels.Include("Sender").FirstOrDefault(x => x.GUID == GUID);

                            if (parcel == null)
                            {
                                response.IsSuccess = false;
                                response.Message = "Parcel not found!";
                            }
                            else
                            {
                                if (!User.IsInRole("Admin") && !(parcel.Sender.Id == user.Id || parcel.RecipientPublicKey == user.PublicKey))
                                {
                                    response.IsSuccess = false;
                                    response.Message = "Access is denied!";
                                }

                                response.IsSuccess = true;
                                response.Message = "OK!";
                                response.Parcel = parcel;
                            }
                        }
                    }
                }
            }
            catch (Exception err)
            {
                response.IsSuccess = false;
                response.Message = err.Message;
            }

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Post(ParcelModel model)
        {
            var response = new JsonResponse();

            try
            {
                var identity = HttpContext.GetOwinContext().Authentication.GetExternalIdentity(DefaultAuthenticationTypes.ApplicationCookie);

                if (identity == null || identity.Name == null)
                {
                    response.IsSuccess = false;
                    response.Message = "Authorization error!";
                }
                else
                {
                    using (var dbContext = new DatabaseContext())
                    {
                        dbContext.Configuration.LazyLoadingEnabled = false;

                        var user = dbContext.Users.FirstOrDefault(x => x.Email == identity.Name);

                        if (user == null)
                        {
                            response.IsSuccess = false;
                            response.Message = "User not found!";
                        }
                        else if (!user.IsActivated)
                        {
                            response.IsSuccess = false;
                            response.Message = "User not activated!";
                        }
                        else
                        {
                            var parcel = dbContext.Parcels.Include("Sender").FirstOrDefault(x => x.GUID == model.GUID);

                            if (parcel != null)
                            {
                                if (!User.IsInRole("Admin") && !(parcel.Sender.Id == user.Id || parcel.RecipientPublicKey == user.PublicKey))
                                {
                                    response.IsSuccess = false;
                                    response.Message = "Access is denied!";

                                    return Json(response, JsonRequestBehavior.AllowGet);
                                }
                            }
                            else
                            {
                                parcel = new Parcel();
                                parcel.Sender = user;                                
                                parcel.CreatedAt = DateTime.Now;
                                parcel.DeliveryStatus = DeliveryStatusEnum.AwaitingPayment;
                                parcel.GUID = Guid.NewGuid().ToString();
                                parcel.PaymentDate = "Not paid";
                                parcel.ShipmentDate = "---";

                                dbContext.Parcels.Add(parcel);

                                //Деплоим новый смарт

                            }

                            parcel.Sender.FullName = model.Sender.Name;
                            parcel.Sender.PublicKey = model.Sender.PublicKey;
                            parcel.Sender.Address = model.Sender.Address;
                            parcel.PaymentAmount = model.Payment.Amount;
                            parcel.SafeAccount = model.Payment.SafeAccount;
                            parcel.Comment = model.Comment;
                            parcel.RecipientPublicKey = model.Recipient.PublicKey;
                            parcel.RecipientAddress = model.Recipient.Address;
                            parcel.RecipientName = model.Recipient.Name;
                            parcel.TrackNumber = model.Delivery.TrackNumber;

                            dbContext.SaveChanges();

                            response.IsSuccess = true;
                            response.Message = "OK!";
                            response.Parcel = parcel;
                        }
                    }
                }
            }
            catch (Exception err)
            {
                response.IsSuccess = false;
                response.Message = err.Message;
            }

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Delete(ChangeModel model)
        {
            var response = new JsonResponse();

            try
            {
                var identity = HttpContext.GetOwinContext().Authentication.GetExternalIdentity(DefaultAuthenticationTypes.ApplicationCookie);

                if (identity == null || identity.Name == null)
                {
                    response.IsSuccess = false;
                    response.Message = "Authorization error!";
                }
                else
                {
                    using (var dbContext = new DatabaseContext())
                    {
                        dbContext.Configuration.LazyLoadingEnabled = false;

                        var user = dbContext.Users.FirstOrDefault(x => x.Email == identity.Name);

                        if (user == null)
                        {
                            response.IsSuccess = false;
                            response.Message = "User not found!";
                        }
                        else if (!user.IsActivated)
                        {
                            response.IsSuccess = false;
                            response.Message = "User not activated!";
                        }
                        else
                        {
                            if (!User.IsInRole("Admin"))
                            {
                                response.IsSuccess = false;
                                response.Message = "Access is denied!";
                            }
                            else
                            {
                                var parcel = dbContext.Parcels.Include("Sender").FirstOrDefault(x => x.GUID == model.GUID);

                                if (parcel == null)
                                {
                                    response.IsSuccess = false;
                                    response.Message = "Parcel not found!";
                                }
                                else
                                {
                                    var messages = dbContext.Messages.Include("Parcel").Where(x => x.Parcel.GUID == model.GUID).ToList();
                                    var events = dbContext.TrackEvents.Include("Parcel").Where(x => x.Parcel.GUID == model.GUID).ToList();

                                    dbContext.Messages.RemoveRange(messages);
                                    dbContext.TrackEvents.RemoveRange(events);
                                    dbContext.Parcels.Remove(parcel);
                                    dbContext.SaveChanges();

                                    response.IsSuccess = true;
                                    response.Message = "OK!";
                                }
                            }

                            return Json(response, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
            }
            catch (Exception err)
            {
                response.IsSuccess = false;
                response.Message = err.Message;
            }

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ChangeStatus(ChangeModel model)
        {
            var response = new JsonResponse();

            try
            {
                var identity = HttpContext.GetOwinContext().Authentication.GetExternalIdentity(DefaultAuthenticationTypes.ApplicationCookie);

                if (identity == null || identity.Name == null)
                {
                    response.IsSuccess = false;
                    response.Message = "Authorization error!";
                }
                else
                {
                    using (var dbContext = new DatabaseContext())
                    {
                        dbContext.Configuration.LazyLoadingEnabled = false;

                        var user = dbContext.Users.FirstOrDefault(x => x.Email == identity.Name);

                        if (user == null)
                        {
                            response.IsSuccess = false;
                            response.Message = "User not found!";
                        }
                        else if (!user.IsActivated)
                        {
                            response.IsSuccess = false;
                            response.Message = "User not activated!";
                        }
                        else
                        {
                            if (!User.IsInRole("Admin"))
                            {
                                response.IsSuccess = false;
                                response.Message = "Access is denied!";
                            }
                            else
                            {
                                var parcel = dbContext.Parcels.Include("Sender").FirstOrDefault(x => x.GUID == model.GUID);

                                if (parcel == null)
                                {
                                    response.IsSuccess = false;
                                    response.Message = "Parcel not found!";                                    
                                }
                                else
                                {
                                    if (model.Status == (int)DeliveryStatusEnum.AwaitingPayment)
                                    {
                                        parcel.DeliveryStatus = DeliveryStatusEnum.AwaitingPayment;
                                        parcel.PaymentDate = "Not paid";
                                        parcel.ShipmentDate = "---";
                                    }
                                    else if (model.Status == (int)DeliveryStatusEnum.PreparationForShipment)
                                    {
                                        parcel.DeliveryStatus = DeliveryStatusEnum.PreparationForShipment;
                                        parcel.PaymentDate = DateTime.Now.ToString();
                                        parcel.ShipmentDate = "---";
                                    }
                                    else if (model.Status == (int)DeliveryStatusEnum.ParcelSent)
                                    {
                                        parcel.DeliveryStatus = DeliveryStatusEnum.ParcelSent;
                                        parcel.ShipmentDate = DateTime.Now.ToString();
                                    }
                                    else if (model.Status == (int)DeliveryStatusEnum.AwaitingReceipt)
                                    {
                                        parcel.DeliveryStatus = DeliveryStatusEnum.AwaitingReceipt;
                                    }
                                    else if (model.Status == (int)DeliveryStatusEnum.Received)
                                    {
                                        parcel.DeliveryStatus = DeliveryStatusEnum.Received;
                                    }

                                    dbContext.SaveChanges();

                                    response.IsSuccess = true;
                                    response.Message = "OK!";
                                    response.Parcel = parcel;
                                }
                            }

                            return Json(response, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
            }
            catch (Exception err)
            {
                response.IsSuccess = false;
                response.Message = err.Message;
            }

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        public class ChangeModel
        {
            public string GUID { get; set; }
            public int Status { get; set; }
        }

        private class JsonResponse
        {
            public bool IsSuccess { get; set; }
            public Parcel Parcel { get; set; }
            public string Message { get; set; }
        }
    }
}