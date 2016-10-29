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

        [Authorize]
        public ActionResult Events()
        {
            List<SysEvent> model = db.Events.Include(c => c.User).OrderByDescending(x => x.Timestamp).ToList();
            return View(model);
        }

        [Authorize]
        public ActionResult Logins()
        {
            List<SysLogin> model = db.Logins.Include(c => c.User).OrderByDescending(x => x.Timestamp).ToList();
            return View(model);
        }
    }
}