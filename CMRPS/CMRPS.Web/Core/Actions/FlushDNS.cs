using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CMRPS.Web.Controllers;
using CMRPS.Web.Models;

namespace CMRPS.Web.Core
{
    public partial class Actions
    {
        /// <summary>
        /// Flush local DNS resolver cache.
        /// </summary>
        public static void FlushDNS()
        {
            try { var flush = DnsFlushResolverCache(); }
            catch (Exception ex)
            {
                // Event
                SysEvent ev = new SysEvent();
                ev.Action = Enums.Action.Power;
                ev.Description = "Flush DNS";
                ev.ActionStatus = Enums.ActionStatus.Error;
                ev.Exception = ex.ToString();
                ev.Username = "Sys";
                ev.Name = "System";
                LogsController.AddEvent(ev);
            }
        }
    }
}