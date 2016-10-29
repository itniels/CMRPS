using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CMRPS.Web.Models;
using CMRPS.Web.ModelsView;
using OfficeOpenXml;

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
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
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
            return PartialView("Delete", locationModel);
        }

        /// <summary>
        /// POST | Delete a location.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult Delete(int id)
        {
            LocationModel locationModel = db.Locations.Find(id);
            db.Locations.Remove(locationModel);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // =================================================================================
        // IMPORT
        // =================================================================================

        /// <summary>
        /// GET | Get the import view.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public ActionResult Import()
        {
            ImportViewModel model = new ImportViewModel();
            return View(model);
        }

        /// <summary>
        /// POST | Import locations to database.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public ActionResult Import(ImportViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.File != null)
                {
                    try
                    {
                        ExcelPackage package = new ExcelPackage(model.File.InputStream);
                        ExcelWorksheet sheet = package.Workbook.Worksheets[1];
                        int rows = sheet.Dimension.Rows;
                        for (int i = 1; i <= rows; i++)
                        {
                            if (i > 1)
                            {
                                LocationModel item = new LocationModel();
                                // ID
                                int id = -1;
                                try
                                {
                                    int.TryParse(sheet.Cells[i, 1].Value.ToString(), out id);
                                }
                                catch (Exception) { }

                                if (id >= 0)
                                    item.Id = id;

                                item.Location = sheet.Cells[i, 2].Value.ToString();
                                db.Locations.AddOrUpdate(item);
                                db.SaveChanges();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        return View(model);
                    }
                }

                return RedirectToAction("Index");
            }
            return View(model);
        }

        // =================================================================================
        // EXPORT
        // =================================================================================

        /// <summary>
        /// DOWNLOAD | Get an empty template.
        /// </summary>
        [Authorize]
        public void EmptyExport()
        {
            try
            {
                ExcelPackage package = new ExcelPackage();
                ExcelWorksheet sheet = package.Workbook.Worksheets.Add("Locations");

                // Header text
                sheet.Cells[1, 1].Value = "Id";
                sheet.Cells[1, 2].Value = "Name";
                // Format cells
                sheet.Cells[1, 1].Style.Font.Bold = true;
                sheet.Cells[1, 2].Style.Font.Bold = true;
                sheet.Column(2).Width = 100;

                DateTime date = DateTime.Now;

                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;  filename=Locations_" + date.ToShortDateString() + ".xlsx");
                Response.BinaryWrite(package.GetAsByteArray());
            }
            catch (Exception)
            {
                
            }
        }

        [Authorize]
        public void Export()
        {
            List<LocationModel> model = db.Locations.ToList();
            try
            {
                ExcelPackage package = new ExcelPackage();
                ExcelWorksheet sheet = package.Workbook.Worksheets.Add("Locations");

                // Header text
                sheet.Cells[1, 1].Value = "Id";
                sheet.Cells[1, 2].Value = "Name";
                // Format cells
                sheet.Cells[1, 1].Style.Font.Bold = true;
                sheet.Cells[1, 2].Style.Font.Bold = true;
                sheet.Column(2).Width = 100;

                // Add data
                int row = 2;    // Start after headers
                foreach (LocationModel item in model)
                {
                    sheet.Cells[row, 1].Value = item.Id;
                    sheet.Cells[row, 2].Value = item.Location;
                    row++;
                }

                DateTime date = DateTime.Now;

                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;  filename=LocationsExport_" + date.ToShortDateString() + ".xlsx");
                Response.BinaryWrite(package.GetAsByteArray());
            }
            catch (Exception)
            {

            }
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
