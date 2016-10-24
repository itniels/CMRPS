using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CMPRS.Web.Models;
using CMRPS.Web.Models;
using Hangfire;
using Hangfire.Common;
using Hangfire.Server;

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
                // Check for changed settings we need to reflect here and now.
                SettingsModel settings = db.Settings.Single(x => x.Id == 1);
                if (settings.PingInterval != model.PingInterval)
                {
                    // Reset Hangfires recurring ping job.
                    var manager = new RecurringJobManager();
                    manager.AddOrUpdate("Ping", Job.FromExpression(() => JobsController.Ping()), Cron.MinuteInterval(model.PingInterval));
                }

                // Save to database
                db.Settings.AddOrUpdate(model);
                db.SaveChanges();
                return RedirectToAction("Index", "Home");
            }
            return View(model);
        }
    }
}