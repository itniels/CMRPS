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
using CMRPS.Web.Models;
using Hangfire;
using Hangfire.Common;
using Microsoft.Ajax.Utilities;
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
            //TODO = Make this adjustable in DB settings
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