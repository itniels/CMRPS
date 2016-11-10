using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using CMPRS.Web.Models;
using CMRPS.Web.Hubs;
using CMRPS.Web.Models;
using Hangfire;
using Hangfire.Common;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;

namespace CMRPS.Web.Controllers
{
    public class JobsController : Controller
    {
        private static ApplicationDbContext db = new ApplicationDbContext();
        /// <summary>
        /// HangFire | Pings all computers and gets info from online devices (IP/MAC).
        /// </summary>
        [AutomaticRetry(Attempts = 0, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
        [DisableConcurrentExecution(50)]
        public static void Enqueue()
        {
            // Set startup variables.
            List<ComputerModel> computers = db.Computers.ToList();
            List<WorkerQueue> workerQueues = new List<WorkerQueue>();
            SettingsModel settings = db.Settings.First();

            // Options
            int queues = settings.WorkerQueues;

            // Create new queues.
            for (int i = 0; i < queues; i++)
            {
                WorkerQueue item = new WorkerQueue();
                item.Computers = new List<int>();
                item.Name = "Q" + i;
                item.Computers = new List<int>();
                workerQueues.Add(item);
            }

            // Add computer IDs to queues.
            int qid = 0;
            foreach (ComputerModel computer in computers)
            {
                var queue = workerQueues.SingleOrDefault(x => x.Name == "Q" + qid);
                queue.Computers.Add(computer.Id);

                // Itterate through the queues to spread the load.
                if (qid == queues - 1)
                    qid = 0;
                else
                    qid++;
            }

            // Flush the local DNS before resolving hostnames
            ActionController.FlushDNS();

            // Update queues in database.
            foreach (WorkerQueue wq in workerQueues)
            {
                var thread = new Thread(new ThreadStart(() => ExecuteQueue(wq)));
                thread.Name = wq.Name;
                thread.Start();
            }

            // Make sure we don't quit untill we are done!
            bool completed = false;
            while (!completed)
            {
                completed = true;
                foreach (WorkerQueue wq in workerQueues)
                {
                    if (wq.isEnqueued)
                        completed = false;
                }
            }

            // Call SignalR
            var context = GlobalHost.ConnectionManager.GetHubContext<LiveUpdatesHub>();
            context.Clients.All.UpdateHomePage();
        }


        private static void ExecuteQueue(WorkerQueue wq)
        {
            // Check if ready
            if (wq != null && wq.Computers.Count > 0)
            {
                // Tell the rest of the world we are working really hard!
                wq.isEnqueued = true;

                // Work the queue.
                foreach (int id in wq.Computers)
                {
                    ActionController.UpdateComputer(id);
                }
                // Tell the world we are done.
                wq.isEnqueued = false;
            }
        }

        /// <summary>
        /// HangFire | Cleans the logs if set in settings.
        /// </summary>
        public static void CleanLogs()
        {
            ApplicationDbContext dbc = new ApplicationDbContext();
            SettingsModel settings = dbc.Settings.FirstOrDefault();

            if (settings.CleanLogs)
            {
                DateTime target = DateTime.Now.AddDays(-settings.KeepLogsFor);
                // Logins
                List<SysLogin> logins = dbc.Logins.Where(x => x.Timestamp.CompareTo(target) < 0).ToList();
                dbc.Logins.RemoveRange(logins);
                // Events
                List<SysEvent> events = dbc.Events.Where(x => x.Timestamp.CompareTo(target) < 0).ToList();
                dbc.Events.RemoveRange(events);
                // Commit to DB
                dbc.SaveChanges();

                // Call SignalR
                var context = GlobalHost.ConnectionManager.GetHubContext<LiveUpdatesHub>();
                context.Clients.All.UpdateHomePage();
            }
        }

        [AutomaticRetry(Attempts = 0, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
        [DisableConcurrentExecution(50)]
        public static void Scheduler()
        {
            ApplicationDbContext context = new ApplicationDbContext();
            List<ScheduledModel> Schedules = context.Schedules.ToList();
            DayOfWeek currentDay = DateTime.Now.DayOfWeek;

            foreach (ScheduledModel schedule in Schedules)
            {
                bool isToday = false;
                bool hasRun = schedule.LastRun.DayOfWeek == DateTime.Now.DayOfWeek;
                bool isTime = false;
                if (schedule.Active)
                {
                    // Is it today
                    switch (currentDay)
                    {
                        case DayOfWeek.Monday:
                            isToday = schedule.DayMonday;
                            break;
                        case DayOfWeek.Tuesday:
                            isToday = schedule.DayTuesday;
                            break;
                        case DayOfWeek.Wednesday:
                            isToday = schedule.DayWednsday;
                            break;
                        case DayOfWeek.Thursday:
                            isToday = schedule.DayThursday;
                            break;
                        case DayOfWeek.Friday:
                            isToday = schedule.DayFriday;
                            break;
                        case DayOfWeek.Saturday:
                            isToday = schedule.DaySaturday;
                            break;
                        case DayOfWeek.Sunday:
                            isToday = schedule.DaySunday;
                            break;
                    }


                    DateTime runTime = DateTime.Today;
                    runTime = runTime.AddHours(schedule.Hour);
                    runTime = runTime.AddMinutes(schedule.Minute);

                    TimeSpan ts = DateTime.Now - runTime;
                    isTime = ts.TotalSeconds >= 0;

                    if (isToday && !hasRun && isTime)
                    {
                        // Execute schedule
                        ActionController.ScheduleExecute(schedule.Id);
                    }
                }
            }
        }



    }
}