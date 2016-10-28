using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CMRPS.Web.Models;

namespace CMRPS.Web.ModelsView
{
    public class HomeIndexViewModels
    {
        // Devices
        public int DevicesTotal { get; set; }
        public int DevicesOnline { get; set; }
        public double DevicesOnlinePercentage { get; set; }
        public int DevicesOffline { get; set; }
        public double DevicesOfflinePercentage { get; set; }

        // Events
        public List<SysEvent> Events { get; set; }

        // Logins
        public List<SysLogin> Logins { get; set; }
    }
}