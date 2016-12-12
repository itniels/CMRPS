using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using CMPRS.Web.Models;
using CMRPS.Web.Controllers;
using CMRPS.Web.Hubs;
using CMRPS.Web.Models;
using Hangfire;
using Microsoft.AspNet.SignalR;

namespace CMRPS.Web.Core
{
    public partial class Jobs
    {
        /// <summary>
        /// HangFire | Enqueue job to retrieve computer information.
        /// </summary>
        [AutomaticRetry(Attempts = 0, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
        [DisableConcurrentExecution(50)]
        public static void Enqueue()
        {
            ApplicationDbContext db = new ApplicationDbContext();
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
            Core.Actions.FlushDNS();

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

        /// <summary>
        /// Enqueue (Helper) | Executes a specific queue of computers to retrieve information, and update database.
        /// </summary>
        /// <param name="wq"></param>
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
                    UpdateComputer(id);
                }
                // Tell the world we are done.
                wq.isEnqueued = false;
            }
        }

        /// <summary>
        /// HangFire | Called to update computer info.
        /// </summary>
        /// <param name="ComputerID"></param>
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        private static void UpdateComputer(int id)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            ComputerModel computer = db.Computers.Single(x => x.Id == id);

            if (computer != null)
            {
                // Ping computer
                computer.IsOnline = Core.Actions.Ping(computer);
                // Get Info (but only if it is online)
                if (computer.IsOnline)
                {
                    computer.LastSeen = DateTime.Now;
                    computer.IP = Core.Actions.GetIP(computer);
                    computer.MAC = Core.Actions.GetMAC(computer);
                }
                else
                {
                    if (computer.LastSeen.Year < 1990)
                    {
                        computer.LastSeen = computer.PurchaseDate;
                    }
                }
                // Update Database
                db.Computers.AddOrUpdate(computer);
                db.SaveChanges();

                // Call SignalR
                var context = GlobalHost.ConnectionManager.GetHubContext<LiveUpdatesHub>();
                string lastSeen = computer.LastSeen.ToShortDateString() + " " + computer.LastSeen.ToShortTimeString();
                context.Clients.All.UpdateListView(id, computer.IsOnline, computer.IP, computer.MAC, lastSeen);
                context.Clients.All.UpdateOverview(id, computer.IsOnline, computer.IP, computer.MAC, lastSeen);
                context.Clients.All.UpdateComputers(id, computer.IsOnline, computer.IP, computer.MAC, lastSeen);
            }
        }

    }
}