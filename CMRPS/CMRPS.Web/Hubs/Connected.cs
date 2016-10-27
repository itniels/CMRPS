using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace CMRPS.Web.Hubs
{
    [HubName("ConnectedHub")]
    public class Connected : Hub
    {
        public void isConnected()
        {
            Clients.All.hello();
        }
    }
}