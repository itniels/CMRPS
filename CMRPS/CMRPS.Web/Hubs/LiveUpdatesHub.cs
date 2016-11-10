using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace CMRPS.Web.Hubs
{
    public class LiveUpdatesHub : Hub
    {
        public void UpdateOverview(string id, string status, string ip, string mac, string lastSeen)
        {
            Clients.All.UpdateOverview(id, status, ip, mac, lastSeen);
        }

        public void UpdateListView(string id, string status, string ip, string mac, string lastSeen)
        {
            Clients.All.UpdateListView(id, status, ip, mac, lastSeen);
        }

        public void UpdateComputers(string id, string status, string ip, string mac, string lastSeen)
        {
            Clients.All.UpdateComputers(id, status, ip, mac, lastSeen);
        }

        public void UpdateSchedules(string id, string lastRun)
        {
            Clients.All.UpdateSchedules(id, lastRun);
        }

        public void UpdateHomePage()
        {
            Clients.All.UpdateHomePage();
        }
    }
}