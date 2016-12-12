using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using CMRPS.Web.Models;

namespace CMRPS.Web.Controllers
{
    public class ViewController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // =================================================================================
        // Overview
        // =================================================================================

        /// <summary>
        /// GET | Overview page.
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public ActionResult Overview()
        {
            List<ComputerModel> model = db.Computers
                .Include(x => x.Color)
                .Include(x => x.Location)
                .Include(x => x.Type)
                .OrderBy(x => x.Name)
                .ToList();

            return View(model);
        }

        // =================================================================================
        // ListView
        // =================================================================================

        /// <summary>
        /// GET | ListView page.
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public ActionResult ListView()
        {
            ViewBag.Colors = db.Colors.ToList();
            ViewBag.Locations = db.Locations.ToList();
            ViewBag.Types = db.ComputerTypes.ToList();
            ViewBag.Status = new List<string>() { "Online", "Offline" };

            List<ComputerModel> model = db.Computers
                .Include(x => x.Color)
                .Include(x => x.Location)
                .Include(x => x.Type)
                .OrderBy(x => x.Name)
                .ToList();

            return View(model);
        }

        /// <summary>
        /// GET | Get a filtered list using ajax.
        /// [DEPRECATED: This is moved to JavaScript code instead to make it easier for signalR.]
        /// </summary>
        /// <param name="name"></param>
        /// <param name="hostname"></param>
        /// <param name="status"></param>
        /// <param name="type"></param>
        /// <param name="color"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult GetFilterList(string name, string hostname, string status, string type, string color, string location)
        {
            List<ComputerModel> model = new List<ComputerModel>();
            List<ComputerModel> list = db.Computers
                .Include(x => x.Type)
                .Include(x => x.Color)
                .Include(x => x.Location)
                .OrderBy(x => x.Name)
                .ToList();
            foreach (ComputerModel item in list)
            {
                var isFound = true;
                // Name
                if (name != "")
                {
                    if (!item.Name.ToUpper().Contains(name.ToUpper()))
                        isFound = false;
                }
                // Hostname
                if (hostname != "")
                {
                    if (!item.Hostname.ToUpper().Contains(hostname.ToUpper()))
                        isFound = false;
                }
                // IsOnline
                if (status != "")
                {
                    if (status.ToUpper() == "ONLINE")
                    {
                        if (item.IsOnline == false)
                            isFound = false;
                    }
                    if (status.ToUpper() == "OFFLINE")
                    {
                        if (item.IsOnline == true)
                            isFound = false;
                    }
                }
                // Type
                if (type != "")
                {
                    if (item.Type.Name.ToUpper() != type.ToUpper())
                        isFound = false;
                }
                // Color
                if (color != "")
                {
                    if (item.Color.Name.ToUpper() != color.ToUpper())
                        isFound = false;
                }
                // Location
                if (location != "")
                {
                    if (item.Location.Location.ToUpper() != location.ToUpper())
                        isFound = false;
                }

                if (isFound)
                    model.Add(item);
            }


            return PartialView("_ListViewComputers", model);
        }
    }
}