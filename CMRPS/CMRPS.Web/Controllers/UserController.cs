using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CMRPS.Web.Models;
using CMRPS.Web.ModelsView;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace CMRPS.Web.Controllers
{
    public class UserController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private ApplicationUserManager manager;

        /// <summary>
        /// GET | List of users.
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public ActionResult Index()
        {
            List<ApplicationUser> model = db.Users.ToList();
            return View(model);
        }

        /// <summary>
        /// GET | Delete a User.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult Delete(string id)
        {
            ApplicationUser model = db.Users.Find(id);
            if (model == null)
            {
                return HttpNotFound();
            }
            return PartialView("Delete", model);
        }

        /// <summary>
        /// GET | Delete a user.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public ActionResult Delete(string id, bool removeLogins, bool removeEvents)
        {
            if (id != null)
            {
                ApplicationUser user = db.Users.SingleOrDefault(x => x.Id == id);

                // Delete or NULL dependencies
                List<SysLogin> Logins = db.Logins.Include(c => c.User).Where(c => c.User.Id == id).ToList();
                List<SysEvent> Events = db.Events.Include(c => c.User).Where(c => c.User.Id == id).ToList();

                if (removeLogins)
                    db.Logins.RemoveRange(Logins);
                else
                {
                    foreach (SysLogin item in Logins)
                    {
                        item.Name = item.User.Firstname + " " + item.User.Lastname;
                        item.Username = item.User.UserName + " (DELETED)";
                        item.User = null;
                        db.Logins.AddOrUpdate(item);
                    }
                }
                if (removeEvents)
                    db.Events.RemoveRange(Events);
                else
                {
                    foreach (SysEvent item in Events)
                    {
                        item.Name = item.User.Firstname + " " + item.User.Lastname;
                        item.Username = item.User.UserName + " (DELETED)";
                        item.User = null;
                        db.Events.AddOrUpdate(item);
                    }
                }
                db.SaveChanges();

                // Delete user
                db.Users.Remove(user);
                db.SaveChanges();
                return RedirectToAction("Index", "User");
            }
            return RedirectToAction("Index", "User");
        }

        /// <summary>
        /// GET | Edit a user.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        public ActionResult Edit(string id)
        {
            if (id != null)
            {
                ApplicationUser model = db.Users.SingleOrDefault(x => x.Id == id);
                return View(model);
            }
            return RedirectToAction("Index", "User");
        }


        [Authorize]
        [HttpPost]
        public ActionResult Edit(ApplicationUser model)
        {
            ApplicationUser user = db.Users.SingleOrDefault(x => x.Id == model.Id);
            user.UserName = model.UserName;
            user.Firstname = model.Firstname;
            user.Lastname = model.Lastname;
            user.Email = model.Email;

            if (ModelState.IsValid)
            {
                db.Users.AddOrUpdate(user);
                db.SaveChanges();
                return RedirectToAction("Index", "User");
            }
            return View(model);
        }

    }


}