using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CMPRS.Web.Models;
using CMRPS.Web.Enums;
using CMRPS.Web.Models;
using Hangfire;
using Hangfire.Common;
using Hangfire.Server;
using Microsoft.AspNet.Identity;
using Action = CMRPS.Web.Enums.Action;

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
            // Event
            SysEvent ev = new SysEvent();
            ev.Action = Action.Settings;
            ev.Description = "Updated system settings";

            if (ModelState.IsValid)
            {
                // Check for changed settings we need to reflect here and now.
                SettingsModel settings = db.Settings.Single(x => x.Id == 1);
                if (settings.PingInterval != model.PingInterval)
                {
                    // Reset Hangfires recurring ping job.
                    var manager = new RecurringJobManager();
                    manager.AddOrUpdate("Enqueue", Job.FromExpression(() => JobsController.Enqueue()), Cron.MinuteInterval(model.PingInterval));
                }

                // Save to database
                db.Settings.AddOrUpdate(model);
                db.SaveChanges();
                
                
                ev.ActionStatus = ActionStatus.OK;
                LogsController.AddEvent(ev, User.Identity.GetUserId());
                
                return RedirectToAction("Index", "Home");
            }
            ev.ActionStatus = ActionStatus.Error;
            LogsController.AddEvent(ev, User.Identity.GetUserId());
            return View(model);
        }
    }
}