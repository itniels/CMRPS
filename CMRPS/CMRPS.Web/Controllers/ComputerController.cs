using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
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

        [HttpGet]
        [Authorize]
        public ActionResult Create()
        {
            // Send lists of items for dropdowns.
            ViewBag.Colors = db.Colors.ToList();
            ViewBag.Locations = db.Locations.ToList();
            ViewBag.Types = db.ComputerTypes.ToList();
            CreateComputerViewModel model = new CreateComputerViewModel();
            return View(model);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreateComputerViewModel model)
        {
            // Get models from dropdowns
            model.Computer.Type = db.ComputerTypes.SingleOrDefault(x => x.Name == model.SelectedType);
            model.Computer.Color = db.Colors.SingleOrDefault(x => x.Name == model.SelectedColor);
            model.Computer.Location = db.Locations.SingleOrDefault(x => x.Location == model.SelectedLocation);

            if (ModelState.IsValid)
            {
                // Save to db if valid
                db.Computers.Add(model.Computer);
                db.SaveChanges();
                return RedirectToAction("Index", "Computer");
            }
            
            // If NOT valid
            // Send lists of items for dropdowns.
            ViewBag.Colors = db.Colors.ToList();
            ViewBag.Locations = db.Locations.ToList();
            ViewBag.Types = db.ComputerTypes.ToList();
            // Return model with errors.
            return View(model);
        }

        [HttpGet]
        [Authorize]
        public ActionResult Edit(int id)
        {
            // Send lists of items for dropdowns.
            ViewBag.Colors = db.Colors.ToList();
            ViewBag.Locations = db.Locations.ToList();
            ViewBag.Types = db.ComputerTypes.ToList();
            // Create view model and add model
            CreateComputerViewModel model = new CreateComputerViewModel();
            //Replace line breaks with HTML code.
            //model.Computer.Description?.Replace(Environment.NewLine, "<br/>");

            model.Computer = db.Computers.SingleOrDefault(x => x.Id == id);
            model.SelectedType = model.Computer.Type.ToString();
            model.SelectedColor = model.Computer.Color.ToString();
            model.SelectedLocation = model.Computer.Location.ToString();
            return View(model);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CreateComputerViewModel model)
        {
            // Get models from dropdowns and add to computer.
            model.Computer.Type = db.ComputerTypes.SingleOrDefault(x => x.Name == model.SelectedType);
            model.Computer.Color = db.Colors.SingleOrDefault(x => x.Name == model.SelectedColor);
            model.Computer.Location = db.Locations.SingleOrDefault(x => x.Location == model.SelectedLocation);

            if (ModelState.IsValid)
            {
                // Save to db if valid
                db.Computers.AddOrUpdate(model.Computer);
                db.SaveChanges();
                return RedirectToAction("Index", "Computer");
            }

            // If NOT valid
            // Send lists of items for dropdowns.
            ViewBag.Colors = db.Colors.ToList();
            ViewBag.Locations = db.Locations.ToList();
            ViewBag.Types = db.ComputerTypes.ToList();
            // Return model with errors.
            return View(model);
        }
    }
}