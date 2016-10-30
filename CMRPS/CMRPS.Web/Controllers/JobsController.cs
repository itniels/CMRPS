using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CMPRS.Web.Models;
using CMRPS.Web.Models;
using Hangfire;
using Hangfire.Common;

namespace CMRPS.Web.Controllers
{
    public class JobsController : Controller
    {
        private static ApplicationDbContext db = new ApplicationDbContext();

        /// <summary>
        /// HangFire | Pings all computers and gets info from online devices.
        /// </summary>
        public static void GetComputerInfo()
        {
            List<ComputerModel> computers = db.Computers.ToList();
            SettingsModel settings = db.Settings.First();
            foreach (ComputerModel computer in computers)
            {
                // Check if it has expired and needs to be queued again.
                try
                {
                    if (computer.isBusy)
                    {
                        
                        //DateTime expired = DateTime.Now.AddMinutes(-(settings.PingInterval + 5));
                        TimeSpan ts = DateTime.Now - computer.Enqueued;
                        computer.isBusy = ts.Minutes < 5;
                    }
                }
                catch (Exception)
                {
                    computer.isBusy = false;
                }
                

                // Make sure they do not get enqueued twice!
                if (!computer.isBusy)
                {
                    computer.isBusy = true;
                    computer.Enqueued = DateTime.Now;
                    db.Computers.AddOrUpdate(computer);
                    db.SaveChanges();
                    // Enqueue
                    BackgroundJob.Enqueue(() => ActionController.UpdateComputer(computer.Id));
                }
            }
        }

        /// <summary>
        /// HangFire | Cleans the logs if set in settings.
        /// </summary>
        public static void CleanLogs()
        {
            SettingsModel settings = db.Settings.FirstOrDefault();
            if (settings.CleanLogs)
            {
                DateTime target = DateTime.Now.AddDays(-settings.KeepLogsFor);
                // Logins
                List<SysLogin> logins = db.Logins.Where(x => x.Timestamp.CompareTo(target) < 0).ToList();
                db.Logins.RemoveRange(logins);
                // Events
                List<SysEvent> events = db.Events.Where(x => x.Timestamp.CompareTo(target) < 0).ToList();
                db.Events.RemoveRange(events);
                // Commit to DB
                db.SaveChanges();
            }
        }

    }
}