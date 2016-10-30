using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CMRPS.Web.Models;

namespace CMRPS.Web.Controllers
{
    public class LogsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public static void AddEvent(SysEvent model, string uid)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            model.User = db.Users.SingleOrDefault(x => x.Id == uid);
            model.Timestamp = DateTime.Now;

            if (model.Exception == null)
                model.Exception = "";

            db.Events.Add(model);
            db.SaveChanges();
        }


        [Authorize]
        public ActionResult Events()
        {
            List<SysEvent> model = db.Events.Include(c => c.User).OrderByDescending(x => x.Timestamp).ToList();
            return View(model);
        }

        [Authorize]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult EventDetails(int id)
        {
            SysEvent model = db.Events.Include(c => c.User).SingleOrDefault(x => x.ID == id);
            return PartialView("EventDetails", model);
        }

        [Authorize]
        public ActionResult Logins()
        {
            List<SysLogin> model = db.Logins.Include(c => c.User).OrderByDescending(x => x.Timestamp).ToList();
            return View(model);
        }
    }
}