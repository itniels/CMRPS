using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using CMRPS.Web.Models;
using CMRPS.Web.ModelsView;

namespace CMRPS.Web.Controllers
{
    public class ComputerTypeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // =================================================================================
        // INDEX
        // =================================================================================

        /// <summary>
        /// GET | List the computer types
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public ActionResult Index()
        {
            return View(db.ComputerTypes.OrderBy(x => x.Name).ToList());
        }

        // =================================================================================
        // DETAILS
        // =================================================================================

        /// <summary>
        /// GET | Details about the Computer type.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult Details(int? id)
        {
            if (id == null) { return new HttpStatusCodeResult(HttpStatusCode.BadRequest); }
            ComputerTypeModel model = db.ComputerTypes.Single(x => x.Id == id);
            if (model == null) { return HttpNotFound(); }
            return PartialView("_PartialDetails", model);
        }

        // =================================================================================
        // CREATE
        // =================================================================================

        /// <summary>
        /// GET | Create a new computer type.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public ActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// POST | Create a new computer type.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreateComputerTypeViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Save to DB to get the ID
                db.ComputerTypes.Add(model.ComputerType);
                db.SaveChanges();

                // uploaded file
                if (model.UploadedFile != null && model.UploadedFile.ContentLength > 0)
                {
                    string directory = Server.MapPath("~/Content/TypeImages/");
                    string filename = model.ComputerType.Id + Path.GetExtension(model.UploadedFile.FileName);
                    var path = Path.Combine(directory, filename);

                    if (!Directory.Exists(directory))
                        Directory.CreateDirectory(directory);

                    model.UploadedFile.SaveAs(path);
                    model.ComputerType.ImagePath = "/Content/TypeImages/" + filename;
                    model.ComputerType.Filename = filename;

                    // Save again to save the image properties.
                    db.ComputerTypes.AddOrUpdate(model.ComputerType);
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }

            return View(model);
        }

        // =================================================================================
        // EDIT
        // =================================================================================

        /// <summary>
        /// GET | Edit a computer type
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
            CreateComputerTypeViewModel model = new CreateComputerTypeViewModel();
            model.ComputerType = db.ComputerTypes.Find(id);

            if (model.ComputerType == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        /// <summary>
        /// POST | Edit a computer type
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CreateComputerTypeViewModel model)
        {
            if (ModelState.IsValid)
            {
                // uploaded file will overwrite the old one.
                if (model.UploadedFile != null && model.UploadedFile.ContentLength > 0)
                {
                    string directory = Server.MapPath("~/Content/TypeImages/");
                    string filename = model.ComputerType.Id + Path.GetExtension(model.UploadedFile.FileName);
                    var path = Path.Combine(directory, filename);

                    if (!Directory.Exists(directory))
                        Directory.CreateDirectory(directory);

                    model.UploadedFile.SaveAs(path);
                    model.ComputerType.ImagePath = "/Content/TypeImages/" + filename;
                    model.ComputerType.Filename = filename;
                }

                // Database
                db.ComputerTypes.AddOrUpdate(model.ComputerType);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
        }

        // =================================================================================
        // DELETE
        // =================================================================================

        /// <summary>
        /// GET | Confirmation | Delete computer type.
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
            ComputerTypeModel computerTypeModel = db.ComputerTypes.Find(id);
            if (computerTypeModel == null)
            {
                return HttpNotFound();
            }
            return PartialView(computerTypeModel);
        }

        /// <summary>
        /// POST | Delete computer type.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult Delete(int id)
        {
            ComputerTypeModel computerTypeModel = db.ComputerTypes.Find(id);
            try
            {
                // Delete the image drom filesystem.
                string file = Server.MapPath("~" + computerTypeModel.ImagePath);
                System.IO.File.Delete(file);
            }
            catch (Exception) { }
            db.ComputerTypes.Remove(computerTypeModel);
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
