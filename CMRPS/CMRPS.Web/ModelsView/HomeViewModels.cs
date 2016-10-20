using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMRPS.Web.ModelsView
{
    public class HomeIndexViewModels
    {
        // Devices
        public int DevicesTotal { get; set; }
        public int DevicesOnline { get; set; }
        public int DevicesOffline { get; set; }
    }
}