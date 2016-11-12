using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CMRPS.Web.Enums;
using CMRPS.Web.Models;
using CMRPS.Web.ModelsView;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;

namespace CMRPS.Web.Controllers
{
    public class SchedulerController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // =================================================================================
        // INDEX
        // =================================================================================

        /// <summary>
        /// GET | Gets all schedules in the system
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public ActionResult Index()
        {
            List<ScheduledModel> model = db.Schedules.ToList();
            return View(model);
        }

        // =================================================================================
        // SELECT VIEW AJAX
        // =================================================================================

        /// <summary>
        /// GET | Changes the view using ajax.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
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

        // =================================================================================
        // DETAILS
        // =================================================================================

        /// <summary>
        /// GET | Details for a schedule.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DetailsScheduleViewModel model = new DetailsScheduleViewModel();
            model.Individual = new List<ComputerModel>();
            model.Schedule = db.Schedules.Single(x => x.Id == id);

            if (model.Schedule.Type == ScheduledType.Individual)
            {
                List<int> IDs = JsonConvert.DeserializeObject<List<int>>(model.Schedule.JsonComputerList);
                foreach (int i in IDs)
                {
                    ComputerModel c = db.Computers.SingleOrDefault(x => x.Id == i);
                    model.Individual.Add(c);
                }
            }
            else if (model.Schedule.Type == ScheduledType.Color)
            {
                model.Color = db.Colors
                    .Include(x => x.Computers)
                    .SingleOrDefault(x => x.Id == model.Schedule.ColorId);
            }
            else if (model.Schedule.Type == ScheduledType.Location)
            {
                model.Location = db.Locations
                    .Include(x => x.Computers)
                    .SingleOrDefault(x => x.Id == model.Schedule.LocationId);
            }
            else if (model.Schedule.Type == ScheduledType.Type)
            {
                model.ComputerType = db.ComputerTypes
                    .Include(x => x.Computers)
                    .SingleOrDefault(x => x.Id == model.Schedule.TypeId);
            }

            if (model.Schedule == null)
            {
                return HttpNotFound();
            }
            return PartialView("_PartialDetails", model);
        }

        // =================================================================================
        // CREATE
        // =================================================================================

        /// <summary>
        /// GET | Create new schedule.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public ActionResult Create()
        {
            ScheduledModel model = new ScheduledModel();

            ViewBag.Hours = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23 };
            ViewBag.Minutes = new List<int>()
            {
                0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20,
                21,22,23,24,25,26,27,28,29,30,31,32,33,34,35,36,37,38,39,40,
                41,42,43,44,45,46,47,48,49,50,51,52,53,54,55,56,57,58,59
            };
            return View(model);
        }

        /// <summary>
        /// POST | Create a new schedule.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
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
                    List<int> clist = JsonConvert.DeserializeObject<List<int>>(model.JsonComputerList);
                    if (clist.Count == 0)
                    {
                        ModelState.AddModelError(String.Empty, "The computer list cannot be empty.");
                        valid = false;
                    }

                    try
                    {
                        string names = "|";
                        foreach (var id in clist)
                        {
                            names += ", " + db.Computers.SingleOrDefault(x => x.Id == id).Name;
                        }
                        model.ComputerListNames = names.Replace("|, ", "");
                    }
                    catch (Exception)
                    {

                    }
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

                try
                {
                    string names = "|";
                    ColorModel item = db.Colors.Include(x => x.Computers).SingleOrDefault(x => x.Id == model.ColorId);
                    foreach (var computer in item.Computers)
                    {
                        names += ", " + computer.Name;
                    }
                    model.ComputerListNames = names.Replace("|, ", "");
                }
                catch (Exception)
                {

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

                try
                {
                    string names = "|";
                    LocationModel item = db.Locations.Include(x => x.Computers).SingleOrDefault(x => x.Id == model.LocationId);
                    foreach (var computer in item.Computers)
                    {
                        names += ", " + computer.Name;
                    }
                    model.ComputerListNames = names.Replace("|, ", "");
                }
                catch (Exception)
                {

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

                try
                {
                    string names = "|";
                    ComputerTypeModel item = db.ComputerTypes.Include(x => x.Computers).SingleOrDefault(x => x.Id == model.TypeId);
                    foreach (var computer in item.Computers)
                    {
                        names += ", " + computer.Name;
                    }
                    model.ComputerListNames = names.Replace("|, ", "");
                }
                catch (Exception)
                {

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
                return View(model);
            }
        }

        // =================================================================================
        // EDIT
        // =================================================================================

        /// <summary>
        /// GET | Edit a schedule.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ScheduledModel model = db.Schedules.Find(id);

            ViewBag.Hours = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23 };
            ViewBag.Minutes = new List<int>()
            {
                0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20,
                21,22,23,24,25,26,27,28,29,30,31,32,33,34,35,36,37,38,39,40,
                41,42,43,44,45,46,47,48,49,50,51,52,53,54,55,56,57,58,59
            };
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        /// <summary>
        /// POST | Edit a schedule.
        /// </summary>
        /// <param name="colorModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ScheduledModel model)
        {
            if (ModelState.IsValid)
            {
                bool valid = true;
                // Get info
                if (model.Type == ScheduledType.Individual)
                {
                    try
                    {
                        List<int> clist = JsonConvert.DeserializeObject<List<int>>(model.JsonComputerList);
                        if (clist.Count == 0)
                        {
                            ModelState.AddModelError(String.Empty, "The computer list cannot be empty.");
                            valid = false;
                        }

                        try
                        {
                            string names = "|";
                            foreach (var id in clist)
                            {
                                names += ", " + db.Computers.SingleOrDefault(x => x.Id == id).Name;
                            }
                            model.ComputerListNames = names.Replace("|, ", "");
                        }
                        catch (Exception)
                        {

                        }
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

                    try
                    {
                        string names = "|";
                        ColorModel item = db.Colors.Include(x => x.Computers).SingleOrDefault(x => x.Id == model.ColorId);
                        foreach (var computer in item.Computers)
                        {
                            names += ", " + computer.Name;
                        }
                        model.ComputerListNames = names.Replace("|, ", "");
                    }
                    catch (Exception)
                    {

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

                    try
                    {
                        string names = "|";
                        LocationModel item = db.Locations.Include(x => x.Computers).SingleOrDefault(x => x.Id == model.LocationId);
                        foreach (var computer in item.Computers)
                        {
                            names += ", " + computer.Name;
                        }
                        model.ComputerListNames = names.Replace("|, ", "");
                    }
                    catch (Exception)
                    {

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

                    try
                    {
                        string names = "|";
                        ComputerTypeModel item = db.ComputerTypes.Include(x => x.Computers).SingleOrDefault(x => x.Id == model.TypeId);
                        foreach (var computer in item.Computers)
                        {
                            names += ", " + computer.Name;
                        }
                        model.ComputerListNames = names.Replace("|, ", "");
                    }
                    catch (Exception)
                    {

                    }
                }

                if (valid)
                {
                    // Event
                    SysEvent ev = new SysEvent();
                    ev.Action = Enums.Action.Info;
                    ev.Description = "Edited schedule: " + model.Name;
                    ev.ActionStatus = ActionStatus.OK;
                    LogsController.AddEvent(ev, User.Identity.GetUserId());

                    model.LastRun = DateTime.Now.AddYears(-100);
                    db.Entry(model).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(model);
                }
            }
            return View(model);
        }

        // =================================================================================
        // DELETE
        // =================================================================================

        /// <summary>
        /// GET | Delete a Schedule.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ScheduledModel model = db.Schedules.Find(id);
            if (model == null)
            {
                return HttpNotFound();
            }
            return PartialView("Delete", model);
        }

        /// <summary>
        /// POST | Delete a Schedule.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult Delete(int id)
        {
            ScheduledModel model = db.Schedules.Find(id);
            // Event
            SysEvent ev = new SysEvent();
            ev.Action = Enums.Action.Info;
            ev.Description = "Deleted schedule: " + model.Name;
            ev.ActionStatus = ActionStatus.OK;
            LogsController.AddEvent(ev, User.Identity.GetUserId());


            db.Schedules.Remove(model);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}