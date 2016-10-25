using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CMRPS.Web.Models;

namespace CMRPS.Web.Controllers
{
    public class LocationController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        /// <summary>
        /// GET | List locations.
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public ActionResult Index()
        {
            return View(db.Locations.ToList());
        }

        /// <summary>
        /// GET | Details for a location.
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
            LocationModel locationModel = db.Locations.Single(x => x.Id == id);
            if (locationModel == null)
            {
                return HttpNotFound();
            }
            return View(locationModel);
        }

        /// <summary>
        /// GET | Create a new location.
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public ActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// POST | Create new location.
        /// </summary>
        /// <param name="locationModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Location")] LocationModel locationModel)
        {
            if (ModelState.IsValid)
            {
                db.Locations.Add(locationModel);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(locationModel);
        }

        /// <summary>
        /// GET | Edit a location.
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
            LocationModel locationModel = db.Locations.Find(id);
            if (locationModel == null)
            {
                return HttpNotFound();
            }
            return View(locationModel);
        }

        /// <summary>
        /// POST | Edit a location.
        /// </summary>
        /// <param name="locationModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Location")] LocationModel locationModel)
        {
            if (ModelState.IsValid)
            {
                db.Entry(locationModel).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(locationModel);
        }

        /// <summary>
        /// GET | Delete a location.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LocationModel locationModel = db.Locations.Find(id);
            if (locationModel == null)
            {
                return HttpNotFound();
            }
            return View(locationModel);
        }

        /// <summary>
        /// POST | Delete a location.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            LocationModel locationModel = db.Locations.Find(id);
            db.Locations.Remove(locationModel);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
