using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CMRPS.Web.Models;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace CMRPS.Web.Hubs
{
    [HubName("HomeHub")]
    public class HomeHub : Hub
    {
        public void Listen(List<ComputerModel> computers)
        {
            Clients.All.DataChanged(computers);
        }
    }
}