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
        /// <summary>
        /// GET | Gets the list of computers in teh system.
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// GET | ChildActionOnly | View details of a computer in a partial view for a modal display box.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        [ChildActionOnly]
        public ActionResult Details(int id)
        {
            ComputerModel model = db.Computers.SingleOrDefault(x => x.Id == id);
            return PartialView("_PartialDetails", model);
        }

        /// <summary>
        /// GET | Delete a Computer in the system.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        public ActionResult Delete(int id)
        {
            ComputerModel model = db.Computers.Single(x => x.Id == id);
            db.Computers.Remove(model);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        /// <summary>
        /// GET | Create a new computer.
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// POST | Create computer from form data.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
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

        /// <summary>
        /// GET | Returns the view for edit with the computer to edit.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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

        /// <summary>
        /// POST | Saves the edited computer to the database.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CreateComputerViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Save primitive type to 'local' first without committing to DB.
                db.Computers.AddOrUpdate(model.Computer);

                // Get model back into context including dependencie objects (color,type,location) and primitive types saved above.
                ComputerModel computer = db.Computers.Single(x => x.Id == model.Computer.Id);
                computer.Type = db.ComputerTypes.SingleOrDefault(x => x.Name == model.SelectedType);
                computer.Color = db.Colors.SingleOrDefault(x => x.Name == model.SelectedColor);
                computer.Location = db.Locations.SingleOrDefault(x => x.Location == model.SelectedLocation);
                db.Computers.AddOrUpdate(computer);
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