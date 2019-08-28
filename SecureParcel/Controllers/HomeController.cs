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
                return RedirectToAction("Index", "SendParcel");
            else
                return RedirectToAction("Login", "Account");
        }
    }
}