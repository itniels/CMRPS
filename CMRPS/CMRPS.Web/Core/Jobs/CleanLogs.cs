using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CMPRS.Web.Models;
using CMRPS.Web.Hubs;
using CMRPS.Web.Models;
using Microsoft.AspNet.SignalR;

namespace CMRPS.Web.Core
{
    public partial class Jobs
    {
        /// <summary>
        /// HangFire | Cleans the logs if set in settings.
        /// </summary>
        public static void CleanLogs()
        {
            ApplicationDbContext db = new ApplicationDbContext();
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

                // Call SignalR
                var context = GlobalHost.ConnectionManager.GetHubContext<LiveUpdatesHub>();
                context.Clients.All.UpdateHomePage();
            }
        }
    }
}