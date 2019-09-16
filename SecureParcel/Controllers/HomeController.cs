using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using SecureParcel.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SecureParcel.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            //SmartContract.Deploy();

            if (User.Identity.IsAuthenticated)
                return RedirectToAction("SentShipment_Demo", "SendParcel");
            else
                return RedirectToAction("Login", "Account");
        }

        public ActionResult AllShipments()
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("AllShipments", "SendParcel");
            else
                return RedirectToAction("Login", "Account");

            //ViewBag.ThisShipments_FixLayout = true;
            //return View();
        }

        public ActionResult ProfileAccount()
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("ProfileAccount", "SendParcel");
            else
                return RedirectToAction("Login", "Account");

            //ViewBag.ThisShipments_FixLayout = true;
            //return View();
        }

        public ActionResult SentShipment()
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("SentShipment", "SendParcel");
            else
                return RedirectToAction("Login", "Account");
        }

        public ActionResult SentShipment_send()
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("SentShipment_send", "SendParcel");
            else
                return RedirectToAction("Login", "Account");
        }

        public ActionResult SentShipment_Demo()
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("SentShipment_Demo", "SendParcel");
            else
                return RedirectToAction("Login", "Account");
        }
    }
}