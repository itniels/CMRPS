using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using CMRPS.Web.Models;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace CMRPS.Web.Hubs
{
    [HubName("TimeHub")]
    public class Connected : Hub
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public void CurrentTime()
        {
            Clients.All.Updated(DateTime.Now.ToLongTimeString());
        }
    }
}