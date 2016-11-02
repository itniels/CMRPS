using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CMRPS.Web.Enums;
using CMRPS.Web.Models;
using CMRPS.Web.ModelsView;
using Microsoft.AspNet.Identity;
using OfficeOpenXml;

namespace CMRPS.Web.Controllers
{
    public class ComputerController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        /// <summary>
        /// GET | Gets the list of computers in teh system.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public ActionResult Index()
        {
            IndexComputerViewModels model = new IndexComputerViewModels();
            model.ComputerTypes = db.ComputerTypes.ToList();
            model.Computers = db.Computers.ToList();
            model.ComputerColors = db.Colors.ToList();
            return View(model);
        }

        /// <summary>
        /// GET | View details of a computer in a partial view for a modal display box.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult Details(int id)
        {
            ComputerModel model = db.Computers
                .Include(a => a.Color)
                .Include(a => a.Location)
                .Include(a => a.Type)
                .Single(x => x.Id == id);

            return PartialView("_PartialDetails", model);
        }

        /// <summary>
        /// GET | Delete a Computer in the system.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        public ActionResult Delete(int id)
        {
            ComputerModel model = db.Computers.Single(x => x.Id == id);
            db.Computers.Remove(model);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        /// <summary>
        /// GET | Create a new computer.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public ActionResult Create()
        {
            // Send lists of items for dropdowns.
            ViewBag.Colors = db.Colors.ToList();
            ViewBag.Locations = db.Locations.ToList();
            ViewBag.Types = db.ComputerTypes.ToList();
            CreateComputerViewModel model = new CreateComputerViewModel();
            return View(model);
        }

        /// <summary>
        /// POST | Create computer from form data.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreateComputerViewModel model)
        {
            // Get models from dropdowns
            model.Computer.Type = db.ComputerTypes.SingleOrDefault(x => x.Name == model.SelectedType);
            model.Computer.Color = db.Colors.SingleOrDefault(x => x.Name == model.SelectedColor);
            model.Computer.Location = db.Locations.SingleOrDefault(x => x.Location == model.SelectedLocation);

            // Set the friendly name if not supplied.
            if (model.Computer.Name == null)
                model.Computer.Name = model.Computer.Hostname.ToUpper();

            if (ModelState.IsValid)
            {
                // Save to db if valid
                db.Computers.Add(model.Computer);
                db.SaveChanges();
                return RedirectToAction("Index", "Computer");
            }

            // If NOT valid
            // Send lists of items for dropdowns.
            ViewBag.Colors = db.Colors.ToList();
            ViewBag.Locations = db.Locations.ToList();
            ViewBag.Types = db.ComputerTypes.ToList();
            // Return model with errors.
            return View(model);
        }

        /// <summary>
        /// GET | Create a new computer.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public ActionResult CreateMultiple()
        {
            // Send lists of items for dropdowns.
            ViewBag.Colors = db.Colors.ToList();
            ViewBag.Locations = db.Locations.ToList();
            ViewBag.Types = db.ComputerTypes.ToList();
            CreateComputerViewModel model = new CreateComputerViewModel();
            return View(model);
        }

        /// <summary>
        /// POST | Create computer from form data.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult CreateMultiple(CreateComputerViewModel model)
        {
            // Get models from dropdowns
            model.Computer.Type = db.ComputerTypes.SingleOrDefault(x => x.Name == model.SelectedType);
            model.Computer.Color = db.Colors.SingleOrDefault(x => x.Name == model.SelectedColor);
            model.Computer.Location = db.Locations.SingleOrDefault(x => x.Location == model.SelectedLocation);

            // Get array of computers to create
            string[] hostnames = model.Computer.Hostname.Split(',');

            if (ModelState.IsValid)
            {
                // Save to db if valid
                foreach (string hostname in hostnames)
                {
                    // Make sure name/hostname is OK.
                    string cname = hostname.Replace(" ", "").Replace(Environment.NewLine, "");
                    model.Computer.Hostname = cname;
                    model.Computer.Name = cname.ToUpper();
                    if (model.Computer.Hostname.Length > 0 && model.Computer.Name.Length > 0)
                    {
                        db.Computers.Add(model.Computer);
                        db.SaveChanges();
                    }
                }
                

                return RedirectToAction("Index", "Computer");
            }

            // If NOT valid
            // Send lists of items for dropdowns.
            ViewBag.Colors = db.Colors.ToList();
            ViewBag.Locations = db.Locations.ToList();
            ViewBag.Types = db.ComputerTypes.ToList();
            // Return model with errors.
            return View(model);
        }

        /// <summary>
        /// GET | Returns the view for edit with the computer to edit.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public ActionResult Edit(int id)
        {
            // Send lists of items for dropdowns.
            ViewBag.Colors = db.Colors.ToList();
            ViewBag.Locations = db.Locations.ToList();
            ViewBag.Types = db.ComputerTypes.ToList();
            // Create view model and add model
            CreateComputerViewModel model = new CreateComputerViewModel();
            //Replace line breaks with HTML code.
            //model.Computer.Description?.Replace(Environment.NewLine, "<br/>");

            model.Computer = db.Computers.SingleOrDefault(x => x.Id == id);
            model.SelectedType = model.Computer.Type.ToString();
            model.SelectedColor = model.Computer.Color.ToString();
            model.SelectedLocation = model.Computer.Location.ToString();
            return View(model);
        }

        /// <summary>
        /// POST | Saves the edited computer to the database.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CreateComputerViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Save primitive type to 'local' first without committing to DB.
                db.Computers.AddOrUpdate(model.Computer);

                // Get model back into context including dependencie objects (color,type,location) and primitive types saved above.
                ComputerModel computer = db.Computers.Single(x => x.Id == model.Computer.Id);
                computer.Type = db.ComputerTypes.SingleOrDefault(x => x.Name == model.SelectedType);
                computer.Color = db.Colors.SingleOrDefault(x => x.Name == model.SelectedColor);
                computer.Location = db.Locations.SingleOrDefault(x => x.Location == model.SelectedLocation);
                db.Computers.AddOrUpdate(computer);
                db.SaveChanges();

                return RedirectToAction("Index", "Computer");
            }

            // If NOT valid
            // Send lists of items for dropdowns.
            ViewBag.Colors = db.Colors.ToList();
            ViewBag.Locations = db.Locations.ToList();
            ViewBag.Types = db.ComputerTypes.ToList();
            // Return model with errors.
            return View(model);
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
                                ComputerModel item = new ComputerModel();
                                // ID
                                int id = -1;
                                try
                                {
                                    int.TryParse(sheet.Cells[i, 1].Value.ToString(), out id);
                                }
                                catch (Exception) { }

                                if (id >= 0)
                                    item.Id = id;
                                // Properties
                                item.Name = sheet.Cells[i, 2].Value.ToString();
                                //TODO
                                db.Computers.AddOrUpdate(item);
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
                ev.Description = "Imported Computers";
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
                ExcelWorksheet sheet = package.Workbook.Worksheets.Add("Computers");

                // Header text
                sheet.Cells[1, 1].Value = "Id";
                sheet.Cells[1, 2].Value = "Name";
                //TODO
                // Format cells
                sheet.Cells[1, 1].Style.Font.Bold = true;
                sheet.Cells[1, 2].Style.Font.Bold = true;
                sheet.Cells[1, 3].Style.Font.Bold = true;
                sheet.Cells[1, 4].Style.Font.Bold = true;
                sheet.Column(2).Width = 25;
                sheet.Column(3).Width = 25;
                sheet.Column(4).Width = 25;

                DateTime date = DateTime.Now;

                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;  filename=Computers_" + date.ToShortDateString() + ".xlsx");
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
                ExcelWorksheet sheet = package.Workbook.Worksheets.Add("Computers");

                // Header text
                sheet.Cells[1, 1].Value = "Id";
                sheet.Cells[1, 2].Value = "Name";
                //TODO
                // Format cells
                sheet.Cells[1, 1].Style.Font.Bold = true;
                sheet.Cells[1, 2].Style.Font.Bold = true;
                sheet.Cells[1, 3].Style.Font.Bold = true;
                sheet.Cells[1, 4].Style.Font.Bold = true;
                sheet.Column(2).Width = 25;
                sheet.Column(3).Width = 25;
                sheet.Column(4).Width = 25;

                // Add data
                int row = 2;    // Start after headers
                foreach (ColorModel item in model)
                {
                    sheet.Cells[row, 1].Value = item.Id;
                    sheet.Cells[row, 2].Value = item.Name;
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
                Response.AddHeader("content-disposition", "attachment;  filename=Computers_" + date.ToShortDateString() + ".xlsx");
                Response.BinaryWrite(package.GetAsByteArray());
            }
            catch (Exception)
            {

            }
        }
    }
}