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
        public void UpdateOverview(string id, string status, string ip, string mac)
        {
            Clients.All.UpdateOverview(id, status, ip, mac);
        }

        public void UpdateListView(string id, string status, string ip, string mac)
        {
            Clients.All.UpdateListView(id, status, ip, mac);
        }

        public void UpdateComputers(string id, string status, string ip, string mac)
        {
            Clients.All.UpdateComputers(id, status, ip, mac);
        }

        public void UpdateHomePage()
        {
            Clients.All.UpdateHomePage();
        }
    }
}