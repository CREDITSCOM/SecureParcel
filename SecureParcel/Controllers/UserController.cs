﻿using System;
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

namespace SecureParcel.Controllers
{
    public class UserController : Controller
    {
        private DatabaseContext dbContext = new DatabaseContext();

        // GET: User
        public async Task<ActionResult> Index()
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
                else if (!User.IsInRole("Admin"))
                {
                    return RedirectToAction("Index", "Home");
                }

                return View(await dbContext.Users.ToListAsync());
            }
        }

        [HttpGet]
        public async Task<ActionResult> Edit(string email)
        {
            var identity = HttpContext.GetOwinContext().Authentication.GetExternalIdentity(DefaultAuthenticationTypes.ApplicationCookie);

            if (identity.Name != email && !User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Home");
            }

            var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Email == email);

            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(user);
        }

        // POST: User/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(ApplicationUser model)
        {
            var identity = HttpContext.GetOwinContext().Authentication.GetExternalIdentity(DefaultAuthenticationTypes.ApplicationCookie);

            if (identity.Name != model.Email && !User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Home");
            }

            if (ModelState.IsValid)
            {
                var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == model.Id);
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;

                if (User.IsInRole("Admin"))
                {
                    user.IsActivated = model.IsActivated;
                }

                await dbContext.SaveChangesAsync();
                return RedirectToAction("Index", "User");
            }

            return View(model);
        }

        // GET: User/Delete/5
        public async Task<ActionResult> Delete(string email)
        {
            if (email == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Email == email);

            if (user == null)
            {
                return HttpNotFound();
            }

            return View(user);
        }

        // POST: User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string email)
        {
            if (!User.IsInRole("Admin") || email == "admin@credits.com")
            {
                return RedirectToAction("Index", "Home");
            }

            var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Email == email);
            var publicKey = user.PublicKey;
            var parcels = await dbContext.Parcels.Include("Sender").Include("Recipient").Where(x => x.Sender.Id == user.Id || x.RecipientPublicKey == publicKey).ToListAsync();
            var messages = await dbContext.Messages.Include("User").Where(x => x.User.Id == user.Id).ToListAsync();
            var events = await dbContext.TrackEvents
                .Include("Parcel")
                .Include("Parcel.Recipient")
                .Include("Parcel.Sender")
                .Where(x => x.Parcel.RecipientPublicKey == publicKey || x.Parcel.Sender.Id == user.Id).ToListAsync();

            dbContext.TrackEvents.RemoveRange(events);
            dbContext.Messages.RemoveRange(messages);
            dbContext.Parcels.RemoveRange(parcels);
            dbContext.Users.Remove(user);

            await dbContext.SaveChangesAsync();

            return RedirectToAction("Index");
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
