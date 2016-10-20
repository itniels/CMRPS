using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CMRPS.Web.Models;
using CMRPS.Web.ModelsView;

namespace CMRPS.Web.Controllers
{
    public class ComputerController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Computer
        [HttpGet]
        [Authorize]
        public ActionResult Index()
        {
            IndexComputerViewModels model = new IndexComputerViewModels();
            model.ComputerTypes = db.ComputerTypes.ToList();
            model.Computers = db.Computers.ToList();
            model.ComputerColors = db.Colors.ToList();
            return View(model);
        }

        //[HttpGet]
        //[Authorize]
        //public ActionResult Create()
        //{
        //    return View();
        //}
    }
}