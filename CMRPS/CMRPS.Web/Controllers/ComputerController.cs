using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Net;
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
        [HttpGet]
        [Authorize]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ComputerModel model = db.Computers.Find(id);
            if (model == null)
            {
                return HttpNotFound();
            }
            return PartialView("Delete", model);
        }

        /// <summary>
        /// GET | Delete a Computer in the system.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
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

            // Make sure hostname looks ok!
            string hostname = model.Computer.Hostname.Replace(" ", "").Replace(Environment.NewLine, "");
            model.Computer.Hostname = hostname.ToLower();

            // Set Last Seen
            model.Computer.LastSeen = DateTime.Now;

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

            // Set Last Seen
            model.Computer.LastSeen = DateTime.Now;

            if (ModelState.IsValid)
            {
                // Save to db if valid
                foreach (string hostname in hostnames)
                {
                    // Make sure name/hostname is OK.
                    string cname = hostname.Replace(" ", "").Replace(Environment.NewLine, "");
                    model.Computer.Hostname = cname.ToLower();
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

                // Check friendly name
                if (model.Computer.Name == null)
                    computer.Name = model.Computer.Hostname.ToUpper();

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
                                item.LastSeen = DateTime.Now;

                                // ID
                                int id = -1;
                                try
                                {
                                    int.TryParse(sheet.Cells[i, 1].Value.ToString(), out id);
                                }
                                catch (Exception) { }

                                if (id >= 0)
                                    item.Id = id;

                                // Get types
                                string typeName = sheet.Cells[i, 4].Value.ToString();
                                string colorName = sheet.Cells[i, 5].Value.ToString();
                                string locationName = sheet.Cells[i, 6].Value.ToString();

                                // Convert
                                double price = 0;
                                try
                                {
                                    price = sheet.Cells[i, 20].Value != null ? Convert.ToDouble(sheet.Cells[i, 20].Value) : 0;
                                }
                                catch (Exception){}
                                

                                // Properties
                                // We have to check for NULL on all the optional ones!
                                item.Name = sheet.Cells[i, 2].Value.ToString();
                                item.Hostname = sheet.Cells[i, 3].Value.ToString();
                                item.Type = db.ComputerTypes.SingleOrDefault(x => x.Name == typeName);
                                item.Color = db.Colors.SingleOrDefault(x => x.Name == colorName);
                                item.Location = db.Locations.SingleOrDefault(x => x.Location == locationName);
                                item.PurchaseDate = DateTime.Parse(sheet.Cells[i, 7].Value.ToString());
                                item.Description = sheet.Cells[i, 8].Value != null ? sheet.Cells[i, 8].Value.ToString() : "";
                                item.Manufacturer = sheet.Cells[i, 9].Value != null ? sheet.Cells[i, 9].Value.ToString() : "";
                                item.Model = sheet.Cells[i, 10].Value != null ? sheet.Cells[i, 10].Value.ToString() : "";
                                item.CPU = sheet.Cells[i, 11].Value != null ? sheet.Cells[i, 11].Value.ToString() : "";
                                item.CPUCores = sheet.Cells[i, 12].Value != null ? sheet.Cells[i, 12].Value.ToString() : "";
                                item.RAM = sheet.Cells[i, 13].Value != null ? sheet.Cells[i, 13].Value.ToString() : "";
                                item.RAMSize = sheet.Cells[i, 14].Value != null ? sheet.Cells[i, 14].Value.ToString() : "";
                                item.Disk = sheet.Cells[i, 15].Value != null ? sheet.Cells[i, 15].Value.ToString() : "";
                                item.DiskSize = sheet.Cells[i, 16].Value != null ? sheet.Cells[i, 16].Value.ToString() : "";
                                item.EthernetCable = sheet.Cells[i, 17].Value != null ? sheet.Cells[i, 17].Value.ToString() : "";
                                item.EthernetWifi = sheet.Cells[i, 18].Value != null ? sheet.Cells[i, 18].Value.ToString() : "";
                                item.OS = sheet.Cells[i, 19].Value != null ? sheet.Cells[i, 19].Value.ToString() : "";
                                item.Price = price;

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
                sheet.Cells[1, 3].Value = "Hostname";
                sheet.Cells[1, 4].Value = "Type";
                sheet.Cells[1, 5].Value = "Color";
                sheet.Cells[1, 6].Value = "Location";
                sheet.Cells[1, 7].Value = "Purchase Date";
                sheet.Cells[1, 8].Value = "Description";
                sheet.Cells[1, 9].Value = "Manufacturer";
                sheet.Cells[1, 10].Value = "Model";
                sheet.Cells[1, 11].Value = "CPU";
                sheet.Cells[1, 12].Value = "CPU Cores";
                sheet.Cells[1, 13].Value = "RAM";
                sheet.Cells[1, 14].Value = "RAM Size";
                sheet.Cells[1, 15].Value = "Disk";
                sheet.Cells[1, 16].Value = "Disk Size";
                sheet.Cells[1, 17].Value = "Ethernet Cable";
                sheet.Cells[1, 18].Value = "Ethernet WiFi";
                sheet.Cells[1, 19].Value = "OS";
                sheet.Cells[1, 20].Value = "Price";

                // Format cells
                sheet.Cells[1, 1].Style.Font.Bold = true;
                sheet.Cells[1, 2].Style.Font.Bold = true;
                sheet.Cells[1, 3].Style.Font.Bold = true;
                sheet.Cells[1, 4].Style.Font.Bold = true;
                sheet.Cells[1, 5].Style.Font.Bold = true;
                sheet.Cells[1, 6].Style.Font.Bold = true;
                sheet.Cells[1, 7].Style.Font.Bold = true;
                sheet.Cells[1, 8].Style.Font.Bold = true;
                sheet.Cells[1, 9].Style.Font.Bold = true;
                sheet.Cells[1, 10].Style.Font.Bold = true;
                sheet.Cells[1, 11].Style.Font.Bold = true;
                sheet.Cells[1, 12].Style.Font.Bold = true;
                sheet.Cells[1, 13].Style.Font.Bold = true;
                sheet.Cells[1, 14].Style.Font.Bold = true;
                sheet.Cells[1, 15].Style.Font.Bold = true;
                sheet.Cells[1, 16].Style.Font.Bold = true;
                sheet.Cells[1, 17].Style.Font.Bold = true;
                sheet.Cells[1, 18].Style.Font.Bold = true;
                sheet.Cells[1, 19].Style.Font.Bold = true;
                sheet.Cells[1, 20].Style.Font.Bold = true;
                sheet.Column(2).Width = 15;
                sheet.Column(3).Width = 15;
                sheet.Column(7).Width = 22;
                sheet.Column(8).Width = 15;
                sheet.Column(9).Width = 15;
                sheet.Column(10).Width = 15;
                sheet.Column(12).Width = 10;
                sheet.Column(14).Width = 10;
                sheet.Column(17).Width = 15;
                sheet.Column(18).Width = 15;

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
            List<ComputerModel> model = db.Computers
                .Include(c => c.Type)
                .Include(c => c.Color)
                .Include(c => c.Location)
                .ToList();
            try
            {
                ExcelPackage package = new ExcelPackage();
                ExcelWorksheet sheet = package.Workbook.Worksheets.Add("Computers");

                // Header text
                sheet.Cells[1, 1].Value = "Id";
                sheet.Cells[1, 2].Value = "Name";
                sheet.Cells[1, 3].Value = "Hostname";
                sheet.Cells[1, 4].Value = "Type";
                sheet.Cells[1, 5].Value = "Color";
                sheet.Cells[1, 6].Value = "Location";
                sheet.Cells[1, 7].Value = "Purchase Date";
                sheet.Cells[1, 8].Value = "Description";
                sheet.Cells[1, 9].Value = "Manufacturer";
                sheet.Cells[1, 10].Value = "Model";
                sheet.Cells[1, 11].Value = "CPU";
                sheet.Cells[1, 12].Value = "CPU Cores";
                sheet.Cells[1, 13].Value = "RAM";
                sheet.Cells[1, 14].Value = "RAM Size";
                sheet.Cells[1, 15].Value = "Disk";
                sheet.Cells[1, 16].Value = "Disk Size";
                sheet.Cells[1, 17].Value = "Ethernet Cable";
                sheet.Cells[1, 18].Value = "Ethernet WiFi";
                sheet.Cells[1, 19].Value = "OS";
                sheet.Cells[1, 20].Value = "OS";

                // Format cells
                sheet.Cells[1, 1].Style.Font.Bold = true;
                sheet.Cells[1, 2].Style.Font.Bold = true;
                sheet.Cells[1, 3].Style.Font.Bold = true;
                sheet.Cells[1, 4].Style.Font.Bold = true;
                sheet.Cells[1, 5].Style.Font.Bold = true;
                sheet.Cells[1, 6].Style.Font.Bold = true;
                sheet.Cells[1, 7].Style.Font.Bold = true;
                sheet.Cells[1, 8].Style.Font.Bold = true;
                sheet.Cells[1, 9].Style.Font.Bold = true;
                sheet.Cells[1, 10].Style.Font.Bold = true;
                sheet.Cells[1, 11].Style.Font.Bold = true;
                sheet.Cells[1, 12].Style.Font.Bold = true;
                sheet.Cells[1, 13].Style.Font.Bold = true;
                sheet.Cells[1, 14].Style.Font.Bold = true;
                sheet.Cells[1, 15].Style.Font.Bold = true;
                sheet.Cells[1, 16].Style.Font.Bold = true;
                sheet.Cells[1, 17].Style.Font.Bold = true;
                sheet.Cells[1, 18].Style.Font.Bold = true;
                sheet.Cells[1, 19].Style.Font.Bold = true;
                sheet.Cells[1, 20].Style.Font.Bold = true;
                sheet.Column(2).Width = 15;
                sheet.Column(3).Width = 15;
                sheet.Column(7).Width = 22;
                sheet.Column(8).Width = 15;
                sheet.Column(9).Width = 15;
                sheet.Column(10).Width = 15;
                sheet.Column(12).Width = 10;
                sheet.Column(14).Width = 10;
                sheet.Column(17).Width = 15;
                sheet.Column(18).Width = 15;

                // Add data
                int row = 2;    // Start after headers
                foreach (ComputerModel item in model)
                {
                    sheet.Cells[row, 1].Value = item.Id;
                    sheet.Cells[row, 2].Value = item.Name;
                    sheet.Cells[row, 3].Value = item.Hostname;
                    sheet.Cells[row, 4].Value = item.Type.Name;
                    sheet.Cells[row, 5].Value = item.Color.Name;
                    sheet.Cells[row, 6].Value = item.Location.Location;
                    sheet.Cells[row, 7].Value = item.PurchaseDate.Date.ToString();
                    sheet.Cells[row, 8].Value = item.Description;
                    sheet.Cells[row, 9].Value = item.Manufacturer;
                    sheet.Cells[row, 10].Value = item.Model;
                    sheet.Cells[row, 11].Value = item.CPU;
                    sheet.Cells[row, 12].Value = item.CPUCores;
                    sheet.Cells[row, 13].Value = item.RAM;
                    sheet.Cells[row, 14].Value = item.RAMSize;
                    sheet.Cells[row, 15].Value = item.Disk;
                    sheet.Cells[row, 16].Value = item.DiskSize;
                    sheet.Cells[row, 17].Value = item.EthernetCable;
                    sheet.Cells[row, 18].Value = item.EthernetWifi;
                    sheet.Cells[row, 19].Value = item.OS;
                    sheet.Cells[row, 20].Value = item.Price;
                    row++;
                }

                DateTime date = DateTime.Now;

                // Event
                SysEvent ev = new SysEvent();
                ev.Action = Enums.Action.Info;
                ev.Description = "Exported Computers";
                ev.ActionStatus = ActionStatus.OK;
                LogsController.AddEvent(ev, User.Identity.GetUserId());

                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;  filename=Computers_" + date.ToShortDateString() + ".xlsx");
                Response.BinaryWrite(package.GetAsByteArray());
            }
            catch (Exception){}
        }
    }
}