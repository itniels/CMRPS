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

        // GET: Location
        public ActionResult Index()
        {
            return View(db.Locations.ToList());
        }

        // GET: Location/Details/5
        public ActionResult Details(int? id)
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

        // GET: Location/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Location/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
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

        // GET: Location/Edit/5
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

        // POST: Location/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
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

        // GET: Location/Delete/5
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

        // POST: Location/Delete/5
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
