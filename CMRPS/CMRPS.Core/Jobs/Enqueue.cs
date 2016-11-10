using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CMRPS.Core.Jobs
{
    class Enqueue
    {
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
    }
}
