using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CMRPS.Web.Enums;
using CMRPS.Web.Models;
using CMRPS.Web.ModelsView;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using OfficeOpenXml;
using Action = System.Action;

namespace CMRPS.Web.Controllers
{
    public class ColorController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // =================================================================================
        // INDEX
        // =================================================================================

        /// <summary>
        /// GET | List of colors.
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public ActionResult Index()
        {
            return View(db.Colors.OrderBy(x => x.Name).ToList());
        }

        // =================================================================================
        // DETAILS
        // =================================================================================

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

        // =================================================================================
        // CREATE
        // =================================================================================

        /// <summary>
        /// GET | Create color
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public ActionResult Create()
        {
            ColorModel model = new ColorModel();
            model.ColorText = "#202020";
            model.ColorLabel = "#ffffff";
            return View(model);
        }

        /// <summary>
        /// POST | Create new color.
        /// </summary>
        /// <param name="colorModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ColorModel colorModel)
        {
            if (ModelState.IsValid)
            {
                // Event
                SysEvent ev = new SysEvent();
                ev.Action = Enums.Action.Info;
                ev.Description = "Added color: " + colorModel.Name;
                ev.ActionStatus = ActionStatus.OK;
                LogsController.AddEvent(ev, User.Identity.GetUserId());

                db.Colors.Add(colorModel);
                db.SaveChanges();
                
                return RedirectToAction("Index");
            }
            return View(colorModel);
        }

        // =================================================================================
        // EDIT
        // =================================================================================

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
                // Event
                SysEvent ev = new SysEvent();
                ev.Action = Enums.Action.Info;
                ev.Description = "Edited color: " + colorModel.Name;
                ev.ActionStatus = ActionStatus.OK;
                LogsController.AddEvent(ev, User.Identity.GetUserId());

                db.Entry(colorModel).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(colorModel);
        }

        // =================================================================================
        // DELETE
        // =================================================================================

        /// <summary>
        /// GET | Delete a color.
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
            ColorModel colorModel = db.Colors.Find(id);
            if (colorModel == null)
            {
                return HttpNotFound();
            }
            return PartialView("Delete", colorModel);
        }

        /// <summary>
        /// POST | Delete a color.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult Delete(int id)
        {
            ColorModel colorModel = db.Colors.Find(id);
            // Event
            SysEvent ev = new SysEvent();
            ev.Action = Enums.Action.Info;
            ev.Description = "Deleted color: " + colorModel.Name;
            ev.ActionStatus = ActionStatus.OK;
            LogsController.AddEvent(ev, User.Identity.GetUserId());

            
            db.Colors.Remove(colorModel);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // =================================================================================
        // GET HEX COLORS USING AJAX
        // =================================================================================

        /// <summary>
        /// GET | Gets the hex colors in a Json list.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public string GetColors(string name)
        {
            try
            {
                List<string> colors = new List<string>();
                ColorModel color = db.Colors.SingleOrDefault(x => x.Name == name);
                colors.Add(color.ColorText);
                colors.Add(color.ColorLabel);
                return JsonConvert.SerializeObject(colors);
            }
            catch (Exception){}
            return null;
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
                                ColorModel item = new ColorModel();
                                // ID
                                int id = -1;
                                try
                                {
                                    int.TryParse(sheet.Cells[i, 1].Value.ToString(), out id);
                                }
                                catch (Exception){ }
                                
                                if (id >= 0)
                                    item.Id = id;
                                // Properties
                                item.Name = sheet.Cells[i, 2].Value.ToString();
                                item.ColorLabel = sheet.Cells[i, 3].Value.ToString();
                                item.ColorText = sheet.Cells[i, 4].Value.ToString();
                                db.Colors.AddOrUpdate(item);
                                db.SaveChanges();
                            }
                        }
                    }
                    catch (Exception)
                    {
                        return View(model);
                    }
                }

                // Event
                SysEvent ev = new SysEvent();
                ev.Action = Enums.Action.Info;
                ev.Description = "Imported colors";
                ev.ActionStatus = ActionStatus.OK;
                LogsController.AddEvent(ev, User.Identity.GetUserId());

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
                ExcelWorksheet sheet = package.Workbook.Worksheets.Add("Colors");

                // Header text
                sheet.Cells[1, 1].Value = "Id";
                sheet.Cells[1, 2].Value = "Name";
                sheet.Cells[1, 3].Value = "Label Color";
                sheet.Cells[1, 4].Value = "Text Color";
                // Format cells
                sheet.Cells[1, 1].Style.Font.Bold = true;
                sheet.Cells[1, 2].Style.Font.Bold = true;
                sheet.Cells[1, 3].Style.Font.Bold = true;
                sheet.Cells[1, 4].Style.Font.Bold = true;

                sheet.Cells[1, 1].Style.Font.Color.SetColor(Color.Orange);
                sheet.Cells[1, 2].Style.Font.Color.SetColor(Color.LightCoral);
                sheet.Cells[1, 3].Style.Font.Color.SetColor(Color.LightCoral);
                sheet.Cells[1, 4].Style.Font.Color.SetColor(Color.LightCoral);

                sheet.Column(2).Width = 25;
                sheet.Column(3).Width = 25;
                sheet.Column(4).Width = 25;

                DateTime date = DateTime.Now;

                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;  filename=Colors_" + date.ToShortDateString() + ".xlsx");
                Response.BinaryWrite(package.GetAsByteArray());
            }
            catch (Exception)
            {

            }
        }

        [Authorize]
        public void Export()
        {
            List<ColorModel> model = db.Colors.ToList();
            try
            {
                ExcelPackage package = new ExcelPackage();
                ExcelWorksheet sheet = package.Workbook.Worksheets.Add("Colors");

                // Header text
                sheet.Cells[1, 1].Value = "Id";
                sheet.Cells[1, 2].Value = "Name";
                sheet.Cells[1, 3].Value = "Label Color";
                sheet.Cells[1, 4].Value = "Text Color";
                // Format cells
                sheet.Cells[1, 1].Style.Font.Bold = true;
                sheet.Cells[1, 2].Style.Font.Bold = true;
                sheet.Cells[1, 3].Style.Font.Bold = true;
                sheet.Cells[1, 4].Style.Font.Bold = true;

                sheet.Cells[1, 1].Style.Font.Color.SetColor(Color.Orange);
                sheet.Cells[1, 2].Style.Font.Color.SetColor(Color.LightCoral);
                sheet.Cells[1, 3].Style.Font.Color.SetColor(Color.LightCoral);
                sheet.Cells[1, 4].Style.Font.Color.SetColor(Color.LightCoral);

                sheet.Column(2).Width = 25;
                sheet.Column(3).Width = 25;
                sheet.Column(4).Width = 25;

                // Add data
                int row = 2;    // Start after headers
                foreach (ColorModel item in model)
                {
                    sheet.Cells[row, 1].Value = item.Id;
                    sheet.Cells[row, 2].Value = item.Name;
                    sheet.Cells[row, 3].Value = item.ColorLabel;
                    sheet.Cells[row, 4].Value = item.ColorText;
                    row++;
                }

                DateTime date = DateTime.Now;

                // Event
                SysEvent ev = new SysEvent();
                ev.Action = Enums.Action.Info;
                ev.Description = "Exported colors";
                ev.ActionStatus = ActionStatus.OK;
                LogsController.AddEvent(ev, User.Identity.GetUserId());

                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;  filename=ColorsExport_" + date.ToShortDateString() + ".xlsx");
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
