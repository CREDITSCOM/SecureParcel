using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SecureParcel.Classes.Database;
using Microsoft.Owin.Security;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using SecureParcel.Models;
using SecureParcel.Classes;
using System.Threading.Tasks;

namespace SecureParcel.Controllers
{
    [Authorize]
    public class SendParcelController : Controller
    {
        private DatabaseContext dbContext = new DatabaseContext();

        //[AllowAnonymous]
        //public async Task<ActionResult> Index()
        //{
        //    var token = await DhlApi.GetToken();
        //    var item = await DhlApi.AddItem();


        //    var identity = HttpContext.GetOwinContext().Authentication.GetExternalIdentity(DefaultAuthenticationTypes.ApplicationCookie);

        //    if (identity == null || identity.Name == null)
        //    {
        //        return RedirectToAction("Login", "Account");                
        //    }
        //    else
        //    {
        //        var user = dbContext.Users.FirstOrDefault(x => x.Email == identity.Name);

        //        if (user == null)
        //        {
        //            return RedirectToAction("Login", "Account");
        //        }
        //        else if (!user.IsActivated)
        //        {
        //            return RedirectToAction("WaitActivation", "Account");
        //        }

        //        var model = new ParcelModel();
        //        var senderList = new List<Parcel>();
        //        var recipientList = new List<Parcel>();
        //        var parcels = new List<Parcel>();

        //        using (var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new DatabaseContext())))
        //        {
        //            var userId = userManager.FindByName(identity.Name).Id;
        //            var publicKey = user.PublicKey;

        //            if (userManager.IsInRole(userId, "Admin"))
        //            {
        //                parcels = dbContext.Parcels.Include("Sender").ToList();
        //            }
        //            else
        //            {
        //                parcels = dbContext.Parcels.Include("Sender").Where(x => x.Sender.Id == userId || x.RecipientPublicKey == publicKey).ToList();
        //            }

        //            foreach (var parcel in parcels)
        //            {
        //                if (userManager.IsInRole(userId, "Admin") || parcel.Sender.Id == userId)
        //                {
        //                    model.ParcelList.Add(new ParcelModel.TableItem()
        //                    {
        //                        GUID = parcel.GUID,
        //                        Amount = parcel.PaymentAmount?.ToString() + " CS",
        //                        ParcelName = parcel.Name,
        //                        RecipientName = string.IsNullOrEmpty(parcel.RecipientName) ? parcel.RecipientPublicKey : parcel.RecipientName,
        //                        SenderName = string.IsNullOrEmpty(parcel.Sender?.FullName) ? parcel.Sender?.PublicKey : parcel.Sender?.FullName,
        //                        Status = parcel.DeliveryStatus,
        //                        CreatedAt = parcel.CreatedAt
        //                    });
        //                }
        //            }
        //        }

        //        return View(model);
        //    }
        //}

        [AllowAnonymous]
        public ActionResult AllShipments()
        {
            var identity = HttpContext.GetOwinContext().Authentication.GetExternalIdentity(DefaultAuthenticationTypes.ApplicationCookie);
            if (identity == null || identity.Name == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                ViewBag.ThisShipments_FixLayout = true;
                return View();
            }
        }

        [AllowAnonymous]
        public ActionResult SentShipment()
        {
            var identity = HttpContext.GetOwinContext().Authentication.GetExternalIdentity(DefaultAuthenticationTypes.ApplicationCookie);
            if (identity == null || identity.Name == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                ViewBag.ThisShipments_FixLayout = true;
                return View();
            }
        }

        [AllowAnonymous]
        public ActionResult SentShipment_send()
        {
            var identity = HttpContext.GetOwinContext().Authentication.GetExternalIdentity(DefaultAuthenticationTypes.ApplicationCookie);
            if (identity == null || identity.Name == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                ViewBag.ThisShipments_FixLayout = true;
                return View();
            }
        }

        [AllowAnonymous]
        public ActionResult ProfileAccount()
        {
            var identity = HttpContext.GetOwinContext().Authentication.GetExternalIdentity(DefaultAuthenticationTypes.ApplicationCookie);
            if (identity == null || identity.Name == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                ViewBag.ThisShipments_FixLayout = true;
                return View();
            }
        }

        [AllowAnonymous]
        public ActionResult SentShipment_Demo()
        {
            var identity = HttpContext.GetOwinContext().Authentication.GetExternalIdentity(DefaultAuthenticationTypes.ApplicationCookie);
            if (identity == null || identity.Name == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                ViewBag.ThisShipments_FixLayout = true;
                return View();
            }
        }

        public ActionResult SentShipment_Demo_2()
        {
            var identity = HttpContext.GetOwinContext().Authentication.GetExternalIdentity(DefaultAuthenticationTypes.ApplicationCookie);
            if (identity == null || identity.Name == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                ViewBag.ThisShipments_FixLayout = true;
                return View();
            }
        }

        public ActionResult SentShipment_Demo_3()
        {
            var identity = HttpContext.GetOwinContext().Authentication.GetExternalIdentity(DefaultAuthenticationTypes.ApplicationCookie);
            if (identity == null || identity.Name == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                ViewBag.ThisShipments_FixLayout = true;
                return View();
            }
        }

        public ActionResult SentShipment_Demo_4()
        {
            var identity = HttpContext.GetOwinContext().Authentication.GetExternalIdentity(DefaultAuthenticationTypes.ApplicationCookie);
            if (identity == null || identity.Name == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                ViewBag.ThisShipments_FixLayout = true;
                return View();
            }
        }

    }
}