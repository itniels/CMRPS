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
    public class ColorController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        /// <summary>
        /// GET | List of colors.
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public ActionResult Index()
        {
            return View(db.Colors.ToList());
        }

        /// <summary>
        /// GET | Details for a color.
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
            ColorModel colorModel = db.Colors.Single(x => x.Id == id);
            if (colorModel == null)
            {
                return HttpNotFound();
            }
            return PartialView("_PartialDetails", colorModel);
        }

        /// <summary>
        /// GET | Create color
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public ActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// POST | Create new color.
        /// </summary>
        /// <param name="colorModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,ColorLabel,ColorText")] ColorModel colorModel)
        {
            if (ModelState.IsValid)
            {
                db.Colors.Add(colorModel);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(colorModel);
        }

        /// <summary>
        /// GET | Edit a color.
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
            ColorModel colorModel = db.Colors.Find(id);
            if (colorModel == null)
            {
                return HttpNotFound();
            }
            return View(colorModel);
        }

        /// <summary>
        /// POST | Edit a color.
        /// </summary>
        /// <param name="colorModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,ColorLabel,ColorText")] ColorModel colorModel)
        {
            if (ModelState.IsValid)
            {
                db.Entry(colorModel).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(colorModel);
        }

        /// <summary>
        /// GET | Delete a color.
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
            ColorModel colorModel = db.Colors.Find(id);
            if (colorModel == null)
            {
                return HttpNotFound();
            }
            return View(colorModel);
        }

        /// <summary>
        /// POST | Delete a color.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ColorModel colorModel = db.Colors.Find(id);
            db.Colors.Remove(colorModel);
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
