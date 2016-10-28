using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CMRPS.Web.Models;
using CMRPS.Web.ModelsView;

namespace CMRPS.Web.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [Authorize]
        public ActionResult Index()
        {
            // Get data
            HomeIndexViewModels model = new HomeIndexViewModels();
            model.DevicesTotal = db.Computers.ToList().Count;
            model.DevicesOnline = db.Computers.Where(x => x.Status == true).ToList().Count;
            model.DevicesOffline = db.Computers.Where(x => x.Status == false).ToList().Count;
            // Calculate Percentage
            model.DevicesOnlinePercentage = ((double)model.DevicesOnline / (double)model.DevicesTotal) * 100;
            model.DevicesOfflinePercentage = ((double)model.DevicesOffline / (double)model.DevicesTotal) * 100;
            // Lists
            model.Events = db.Events.OrderByDescending(x => x.Timestamp).Take(10).ToList();
            model.Logins = db.Logins.Include(c => c.User).OrderByDescending(x => x.Timestamp).Take(10).ToList();

            // Make View Model
            return View(model);
            
        }
    }
}