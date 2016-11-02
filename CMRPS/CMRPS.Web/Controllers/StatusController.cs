using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CMRPS.Web.Models;

namespace CMRPS.Web.Controllers
{
    public class StatusController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [Authorize]
        public ActionResult Overview()
        {
            List<ComputerModel> model = db.Computers
                .Include(x => x.Color)
                .Include(x => x.Location)
                .Include(x => x.Type)
                .ToList();

            return View(model);
        }

        [Authorize]
        public ActionResult ListView()
        {
            ViewBag.Colors = db.Colors.ToList();
            ViewBag.Locations = db.Locations.ToList();
            ViewBag.Types = db.ComputerTypes.ToList();
            ViewBag.Status = new List<string>() {"Online", "Offline"};

            List<ComputerModel> model = db.Computers
                .Include(x => x.Color)
                .Include(x => x.Location)
                .Include(x => x.Type)
                .ToList();

            return View(model);
        }
    }
}