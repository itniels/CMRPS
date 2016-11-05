using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CMRPS.Web.Enums;
using CMRPS.Web.Models;
using CMRPS.Web.ModelsView;
using Newtonsoft.Json;

namespace CMRPS.Web.Controllers
{
    public class SchedulerController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [Authorize]
        public ActionResult Index()
        {
            List<ScheduledModel> model = db.Schedules.ToList();
            return View(model);
        }

        [Authorize]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult SelectView(Enums.ScheduledType type)
        {
            if (type == ScheduledType.Individual)
            {
                List<ComputerModel> model = db.Computers
                    .Include(c => c.Color)
                    .Include(c => c.Location)
                    .Include(c => c.Type)
                    .ToList();

                return PartialView("_SelectIndividual", model);
            }
            else if (type == ScheduledType.Color)
            {
                List<ColorModel> model = db.Colors
                    .Include(x => x.Computers)
                    .ToList();
                return PartialView("_SelectColor", model);
            }
            else if (type == ScheduledType.Location)
            {
                List<LocationModel> model = db.Locations
                    .Include(x => x.Computers)
                    .ToList();
                return PartialView("_SelectLocation", model);
            }
            else if (type == ScheduledType.Type)
            {
                List<ComputerTypeModel> model = db.ComputerTypes
                    .Include(x => x.Computers)
                    .ToList();
                return PartialView("_SelectType", model);
            }
            return null;
        }

        [HttpGet]
        [Authorize]
        public ActionResult Create()
        {
            ScheduledModel model = new ScheduledModel();
            //model.Computers = db.Computers
            //    .Include(c => c.Color)
            //    .Include(c => c.Location)
            //    .Include(c => c.Type)
            //    .ToList();

            ViewBag.Hours = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23 };
            ViewBag.Minutes = new List<int>()
            {
                0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20,
                21,22,23,24,25,26,27,28,29,30,31,32,33,34,35,36,37,38,39,40,
                41,42,43,44,45,46,47,48,49,50,51,52,53,54,55,56,57,58,59
            };
            return View(model);
        }

        [HttpPost]
        [Authorize]
        public ActionResult Create(ScheduledModel model)
        {
            ModelState.Clear();
            bool valid = model.Name != null;
            model.LastRun = DateTime.Now.AddYears(-100);
            if (!valid)
            {
                ModelState.AddModelError(String.Empty, "Name cannot be empty.");
            }

            if (model.Type == ScheduledType.Individual)
            {
                try
                {
                    string jqlist = model.JsonComputerList.Replace("|,", "");
                    var list = jqlist.Split(',');
                    List<int> IDs = new List<int>();
                    foreach (string s in list)
                    {
                        try
                        {
                            IDs.Add(Convert.ToInt32(s));
                        }
                        catch (Exception) { }

                    }

                    if (IDs.Count == 0)
                    {
                        ModelState.AddModelError(String.Empty, "The computer list cannot be empty.");
                        valid = false;
                    }

                    model.JsonComputerList = JsonConvert.SerializeObject(IDs);

                    
                }
                catch (Exception)
                {
                    ModelState.AddModelError(String.Empty, "The computer list cannot be empty.");
                    valid = false;
                }

            }
            if (model.Type == ScheduledType.Color)
            {
                model.JsonComputerList = "";
                if (model.ColorId == 0)
                {
                    ModelState.AddModelError(String.Empty, "Plese select a color.");
                    valid = false;
                }
            }
            if (model.Type == ScheduledType.Location)
            {
                model.JsonComputerList = "";
                if (model.LocationId == 0)
                {
                    ModelState.AddModelError(String.Empty, "Plese select a location.");
                    valid = false;
                }
            }
            if (model.Type == ScheduledType.Type)
            {
                model.JsonComputerList = "";
                if (model.TypeId == 0)
                {
                    ModelState.AddModelError(String.Empty, "Plese select a Computer Type.");
                    valid = false;
                }
            }

            if (valid)
            {
                db.Schedules.Add(model);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.Hours = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23 };
                ViewBag.Minutes = new List<int>()
                {
                    0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20,
                    21,22,23,24,25,26,27,28,29,30,31,32,33,34,35,36,37,38,39,40,
                    41,42,43,44,45,46,47,48,49,50,51,52,53,54,55,56,57,58,59
                };
                //model.Computers = db.Computers
                //.Include(c => c.Color)
                //.Include(c => c.Location)
                //.Include(c => c.Type)
                //.ToList();
                return View(model);
            }


        }
    }
}