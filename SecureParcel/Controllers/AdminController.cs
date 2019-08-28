using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SecureParcel.Classes.Database;
using Microsoft.Owin.Security;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using SecureParcel.Models;
using System.Data.Entity.Validation;
using System.Diagnostics;

namespace SecureParcel.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private DatabaseContext dbContext = new DatabaseContext();

        [AllowAnonymous]
        public ActionResult Index()
        {
            var identity = HttpContext.GetOwinContext().Authentication.GetExternalIdentity(DefaultAuthenticationTypes.ApplicationCookie);

            if (identity == null || identity.Name == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                var model = new ParcelModel();

                using (var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new DatabaseContext())))
                {
                    var parcels = dbContext.Parcels.Include("Sender").ToList();

                    foreach (var parcel in parcels)
                    {
                        model.ParcelList.Add(new ParcelModel.TableItem()
                        {
                            GUID = parcel.GUID,
                            Amount = parcel.PaymentAmount?.ToString() + " CS",
                            ParcelName = parcel.Name,
                            RecipientName = string.IsNullOrEmpty(parcel.RecipientName) ? parcel.RecipientPublicKey : parcel.RecipientName,
                            SenderName = string.IsNullOrEmpty(parcel.Sender?.FullName) ? parcel.Sender?.PublicKey : parcel.Sender?.FullName,
                            Status = parcel.DeliveryStatus
                        });
                    }
                }

                return View(model);
            }
        }

        [HttpGet]
        public ActionResult Edit(string GUID = "-1")
        {
            var model = new ParcelModel();

            var identity = HttpContext.GetOwinContext().Authentication.GetExternalIdentity(DefaultAuthenticationTypes.ApplicationCookie);

            var user = dbContext.Users.FirstOrDefault(x => x.Email == identity.Name);
            var publicKey = user.PublicKey;

            if (GUID == "-1")
            {
                model.GUID = "-1";
                model.Sender.Address = user.Address;
                model.Sender.Name = user.FullName;
                model.Sender.PublicKey = user.PublicKey;
                model.Payment.Date = "Not paid";
                model.Delivery.Status = DeliveryStatusEnum.AwaitingPayment.ToString();
                model.Delivery.ShipmentDate = "---";
            }
            else
            {
                var parcel = dbContext.Parcels.Include("Sender").Include("TrackEvents").Include("Messages").FirstOrDefault(x => x.GUID == GUID);

                if (parcel == null)
                {
                    return RedirectToAction("Index", "SendParcel");
                }
                else
                {
                    if (!User.IsInRole("Admin") && !(parcel.Sender.Id == user.Id || parcel.RecipientPublicKey == publicKey))
                    {
                        return RedirectToAction("Index", "SendParcel");
                    }

                    model.GUID = parcel.GUID;

                    model.Parcel.Name = parcel.Name;
                    model.Parcel.Description = parcel.Description;

                    model.Sender.Address = parcel.Sender.Address;
                    model.Sender.Name = parcel.Sender.FullName;
                    model.Sender.PublicKey = parcel.Sender.PublicKey;

                    model.Recipient.Address = parcel.RecipientAddress;
                    model.Recipient.Name = parcel.RecipientName;
                    model.Recipient.PublicKey = parcel.RecipientPublicKey;

                    model.Delivery.Status = parcel.DeliveryStatus.ToString();
                    model.Delivery.ShipmentDate = parcel.ShipmentDate;
                    model.Delivery.TrackNumber = parcel.TrackNumber;

                    model.Payment.Amount = parcel.PaymentAmount;
                    model.Payment.Date = parcel.PaymentDate;
                    model.Payment.SafeAccount = parcel.SafeAccount;

                    model.MessageList = parcel.Messages.OrderBy(x => x.Date).ToList();
                    model.TrackEventList = parcel.TrackEvents.OrderBy(x => x.Date).ToList();
                }
            }

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(ParcelModel model)
        {
            var identity = HttpContext.GetOwinContext().Authentication.GetExternalIdentity(DefaultAuthenticationTypes.ApplicationCookie);

            if (identity == null || identity.Name == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                var user = dbContext.Users.FirstOrDefault(x => x.Email == identity.Name);

                if (user == null)
                {
                    return RedirectToAction("Login", "Account");
                }
                else if (!user.IsActivated)
                {
                    return RedirectToAction("WaitActivation", "Account");
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        var parcel = await dbContext.Parcels.Include("Sender").Include("TrackEvents").Include("Messages").FirstOrDefaultAsync(x => x.GUID == model.GUID);

                        if (parcel == null)
                        {
                            parcel = new Parcel();
                            parcel.CreatedAt = DateTime.Now;
                            parcel.DeliveryStatus = DeliveryStatusEnum.AwaitingPayment;
                            parcel.GUID = Guid.NewGuid().ToString();

                            //Parcel
                            parcel.Name = model.Parcel.Name;
                            parcel.Description = model.Parcel.Description;

                            //Sender
                            parcel.Sender = user;
                            parcel.SenderAddress = model.Sender.Address;
                            parcel.SenderName = model.Sender.Name;
                            user.Address = model.Sender.Address;
                            user.FullName = model.Sender.Name;

                            //Payment
                            parcel.PaymentAmount = model.Payment.Amount;
                            parcel.PaymentDate = "Not paid";
                            parcel.SafeAccount = model.Payment.SafeAccount;

                            //Comments
                            parcel.Messages = new List<Message>();
                            if (!string.IsNullOrEmpty(model.Comment))
                                parcel.Messages.Add(new Message() { Date = DateTime.Now, Parcel = parcel, Text = model.Comment, User = user });

                            //Recipient
                            parcel.RecipientPublicKey = model.Recipient.PublicKey;
                            parcel.RecipientAddress = model.Recipient.Address;
                            parcel.RecipientName = model.Recipient.Name;

                            //Delivery
                            parcel.TrackNumber = model.Delivery.TrackNumber;
                            parcel.ShipmentDate = "---";

                            //TrackEvents
                            parcel.TrackEvents = new List<TrackEvent>();
                            parcel.TrackEvents.Add(new TrackEvent() { Date = DateTime.Now, Parcel = parcel, Text = "New parcel added" });

                            dbContext.Parcels.Add(parcel);
                        }
                        else
                        {
                            //Parcel
                            parcel.Name = model.Parcel.Name;
                            parcel.Description = model.Parcel.Description;

                            //Sender
                            parcel.SenderAddress = model.Sender.Address;
                            parcel.SenderName = model.Sender.Name;
                            user.Address = model.Sender.Address;
                            user.FullName = model.Sender.Name;

                            //Payment
                            parcel.PaymentAmount = model.Payment.Amount;
                            parcel.SafeAccount = model.Payment.SafeAccount;

                            //Comments
                            if (!string.IsNullOrEmpty(model.Comment))
                                parcel.Messages.Add(new Message() { Date = DateTime.Now, Parcel = parcel, Text = model.Comment, User = user });

                            //Recipient
                            parcel.RecipientPublicKey = model.Recipient.PublicKey;
                            parcel.RecipientAddress = model.Recipient.Address;
                            parcel.RecipientName = model.Recipient.Name;

                            //Delivery
                            parcel.TrackNumber = model.Delivery.TrackNumber;

                            //TrackEvents
                            parcel.TrackEvents.Add(new TrackEvent() { Date = DateTime.Now, Parcel = parcel, Text = "Parcel changed" });
                        }

                        await dbContext.SaveChangesAsync();
                    }
                    catch (DbEntityValidationException e)
                    {
                        foreach (var eve in e.EntityValidationErrors)
                        {
                            Debug.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:", eve.Entry.Entity.GetType().Name, eve.Entry.State);
                            foreach (var ve in eve.ValidationErrors)
                            {
                                Debug.WriteLine("- Property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage);
                            }
                        }
                        throw;
                    }
                    catch (Exception err)
                    {
                        var error = err.ToString();
                    }

                    return RedirectToAction("Index", "SendParcel");
                }

                return View(model);
            }
        }

        [HttpGet]
        public async Task<ActionResult> Delete(string GUID)
        {
            if (string.IsNullOrEmpty(GUID))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Parcel parcel = await dbContext.Parcels.FirstOrDefaultAsync(x => x.GUID == GUID);
            if (parcel == null)
            {
                return RedirectToAction("Index");
            }

            return View(parcel);
        }

        // POST: SmartJobs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string GUID)
        {
            if (string.IsNullOrEmpty(GUID))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var parcel = await dbContext.Parcels.FirstOrDefaultAsync(x => x.GUID == GUID);
            var messages = await dbContext.Messages.Include("Parcel").Where(x => x.Parcel.GUID == GUID).ToListAsync();
            var events = await dbContext.TrackEvents.Include("Parcel").Where(x => x.Parcel.GUID == GUID).ToListAsync();

            dbContext.Messages.RemoveRange(messages);
            dbContext.TrackEvents.RemoveRange(events);
            dbContext.Parcels.Remove(parcel);

            await dbContext.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<ActionResult> Pay(string GUID)
        {
            Parcel parcel = await dbContext.Parcels.FirstOrDefaultAsync(x => x.GUID == GUID);
            if (parcel == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return View(parcel);
        }

        [HttpPost, ActionName("Pay")]
        public async Task<ActionResult> PayConfirmed(string GUID)
        {
            Parcel parcel = await dbContext.Parcels.Include("TrackEvents").FirstOrDefaultAsync(x => x.GUID == GUID);
            if (parcel == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else
            {
                parcel.DeliveryStatus = DeliveryStatusEnum.PreparationForShipment;
                parcel.TrackEvents.Add(new TrackEvent() { Date = DateTime.Now, Parcel = parcel, Text = "Parcel paid" });
                parcel.PaymentDate = DateTime.Now.ToString();

                await dbContext.SaveChangesAsync();
            }

            return RedirectToAction("Edit", new { GUID = GUID });
        }

        [HttpGet]
        public async Task<ActionResult> SendParcel(string GUID)
        {
            Parcel parcel = await dbContext.Parcels.FirstOrDefaultAsync(x => x.GUID == GUID);
            if (parcel == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return View(parcel);
        }

        [HttpPost, ActionName("SendParcel")]
        public async Task<ActionResult> SendParcelConfirmed(string GUID)
        {
            Parcel parcel = await dbContext.Parcels.Include("TrackEvents").FirstOrDefaultAsync(x => x.GUID == GUID);
            if (parcel == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else
            {
                parcel.DeliveryStatus = DeliveryStatusEnum.ParcelSent;
                parcel.TrackEvents.Add(new TrackEvent() { Date = DateTime.Now, Parcel = parcel, Text = "Parcel sent" });
                parcel.ShipmentDate = DateTime.Now.ToString();

                await dbContext.SaveChangesAsync();
            }

            return RedirectToAction("Edit", new { GUID = GUID });
        }

        [HttpGet]
        public async Task<ActionResult> AwaitingReceipt(string GUID)
        {
            Parcel parcel = await dbContext.Parcels.FirstOrDefaultAsync(x => x.GUID == GUID);
            if (parcel == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return View(parcel);
        }

        [HttpPost, ActionName("AwaitingReceipt")]
        public async Task<ActionResult> AwaitingReceiptConfirmed(string GUID)
        {
            Parcel parcel = await dbContext.Parcels.Include("TrackEvents").FirstOrDefaultAsync(x => x.GUID == GUID);
            if (parcel == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else
            {
                parcel.DeliveryStatus = DeliveryStatusEnum.AwaitingReceipt;
                parcel.TrackEvents.Add(new TrackEvent() { Date = DateTime.Now, Parcel = parcel, Text = "Awaiting receipt" });

                await dbContext.SaveChangesAsync();
            }

            return RedirectToAction("Edit", new { GUID = GUID });
        }

        [HttpGet]
        public async Task<ActionResult> ParcelReceived(string GUID)
        {
            Parcel parcel = await dbContext.Parcels.FirstOrDefaultAsync(x => x.GUID == GUID);
            if (parcel == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return View(parcel);
        }

        [HttpPost, ActionName("ParcelReceived")]
        public async Task<ActionResult> ParcelReceivedConfirmed(string GUID)
        {
            Parcel parcel = await dbContext.Parcels.Include("TrackEvents").FirstOrDefaultAsync(x => x.GUID == GUID);
            if (parcel == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else
            {
                parcel.DeliveryStatus = DeliveryStatusEnum.Received;
                parcel.TrackEvents.Add(new TrackEvent() { Date = DateTime.Now, Parcel = parcel, Text = "Parcel received" });

                await dbContext.SaveChangesAsync();
            }

            return RedirectToAction("Edit", new { GUID = GUID });
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                dbContext.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}