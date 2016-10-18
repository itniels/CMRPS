using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CMPRS.Web.Models;
using CMRPS.Web.Models;

namespace CMRPS.Web.Controllers
{
    public class AdminController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Admin
        [HttpGet]
        [Authorize]
        public ActionResult Index()
        {
            SettingsModel model = db.Settings.SingleOrDefault(x => x.Id == 1);
            return View(model);
        }

        [HttpPost]
        [Authorize]
        public ActionResult Index(SettingsModel model)
        {
            if (ModelState.IsValid)
            {
                db.Settings.AddOrUpdate(model);
                db.SaveChanges();
                return RedirectToAction("Index", "Home");
            }
            return View(model);
        }
    }
}