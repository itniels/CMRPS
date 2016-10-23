using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CMRPS.Web.Models;
using CMRPS.Web.ModelsView;

namespace CMRPS.Web.Controllers
{
    public class ComputerTypeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ComputerType
        public ActionResult Index()
        {
            return View(db.ComputerTypes.ToList());
        }

        // GET: ComputerType/Details/5
        public ActionResult Details(int? id)
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
            return View(computerTypeModel);
        }

        // GET: ComputerType/Create
        [HttpGet]
        [Authorize]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreateComputerTypeViewModel model)
        {
            
            if (ModelState.IsValid)
            {
                // uploaded file
                
                if (model.UploadedFile != null)
                {
                    string directory = Server.MapPath("~/Content/TypeImages/");
                    var path = Path.Combine(directory, model.ComputerType.Name + Path.GetExtension(model.UploadedFile.FileName));

                    if (!Directory.Exists(directory))
                        Directory.CreateDirectory(directory);

                    model.UploadedFile.SaveAs(path);
                    model.ComputerType.ImagePath = path;
                }

                // Database
                db.ComputerTypes.Add(model.ComputerType);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(model);
        }

        // GET: ComputerType/Edit/5
        public ActionResult Edit(int? id)
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
            return View(computerTypeModel);
        }

        // POST: ComputerType/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,ImagePath")] ComputerTypeModel computerTypeModel)
        {
            if (ModelState.IsValid)
            {
                db.Entry(computerTypeModel).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(computerTypeModel);
        }

        // GET: ComputerType/Delete/5
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
            return View(computerTypeModel);
        }

        // POST: ComputerType/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ComputerTypeModel computerTypeModel = db.ComputerTypes.Find(id);
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
