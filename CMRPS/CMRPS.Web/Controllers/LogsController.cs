﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CMRPS.Web.Hubs;
using CMRPS.Web.Models;
using Microsoft.AspNet.SignalR;

namespace CMRPS.Web.Controllers
{
    public class LogsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public static void AddEvent(SysEvent model, string uid = null)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            if (uid != null)
            {
                model.User = db.Users.SingleOrDefault(x => x.Id == uid);
            }
            model.Timestamp = DateTime.Now;

            if (model.Exception == null)
                model.Exception = "";

            db.Events.Add(model);
            db.SaveChanges();

            // Call SignalR
            var context = GlobalHost.ConnectionManager.GetHubContext<LiveUpdatesHub>();
            context.Clients.All.UpdateHomePage();
        }


        [System.Web.Mvc.Authorize]
        public ActionResult Events()
        {
            List<SysEvent> model = db.Events.Include(c => c.User).OrderByDescending(x => x.Timestamp).ToList();
            return View(model);
        }

        [System.Web.Mvc.Authorize]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult EventDetails(int id)
        {
            SysEvent model = db.Events.Include(c => c.User).SingleOrDefault(x => x.ID == id);
            return PartialView("EventDetails", model);
        }

        [System.Web.Mvc.Authorize]
        public ActionResult Logins()
        {
            List<SysLogin> model = db.Logins.Include(c => c.User).OrderByDescending(x => x.Timestamp).ToList();
            return View(model);
        }
    }
}