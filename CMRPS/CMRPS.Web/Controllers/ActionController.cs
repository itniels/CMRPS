using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Web.Mvc;
using CMPRS.Web.Enums;
using CMPRS.Web.Models;
using CMRPS.Web.Enums;
using CMRPS.Web.Hubs;
using CMRPS.Web.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;

namespace CMRPS.Web.Controllers
{
    public class ActionController : Controller
    {

        /// <summary>
        /// GET | Ping a hostname
        /// </summary>
        /// <param name="hostname"></param>
        /// <returns></returns>
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public bool Ping(string hostname)
        {
            // Event
            SysEvent ev = new SysEvent();
            ev.Action = Enums.Action.Power;
            ev.Description = "Pinged: " + hostname;
            try
            {
                bool result = Core.Actions.Ping(hostname);
                ev.ActionStatus = ActionStatus.OK;
                LogsController.AddEvent(ev, User.Identity.GetUserId());
                return result;
            }
            catch (Exception ex)
            {
                ev.ActionStatus = ActionStatus.Error;
                ev.Exception = ex.ToString();
                LogsController.AddEvent(ev, User.Identity.GetUserId());
                return false;
            }
        }

        /// <summary>
        /// GET | Power on a computer by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [System.Web.Mvc.Authorize]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public bool PowerOn(int id)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            ComputerModel model = db.Computers.SingleOrDefault(x => x.Id == id);
            // Event
            SysEvent ev = new SysEvent();
            ev.Action = Enums.Action.Power;
            ev.Description = "Powered on: " + model.Name;

            try
            {
                bool result = Core.Actions.PowerOn(model);
                ev.ActionStatus = ActionStatus.OK;
                LogsController.AddEvent(ev, User.Identity.GetUserId());
                return result;
            }
            catch (Exception ex)
            {
                ev.ActionStatus = ActionStatus.Error;
                ev.Exception = ex.ToString();
                LogsController.AddEvent(ev, User.Identity.GetUserId());
            }
            return false;
        }

        /// <summary>
        /// GET | Power off a computer by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [System.Web.Mvc.Authorize]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public bool PowerOff(int id)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            ComputerModel model = db.Computers.SingleOrDefault(x => x.Id == id);
            // Event
            SysEvent ev = new SysEvent();
            ev.Action = Enums.Action.Power;
            ev.Description = "Powered off: " + model.Name;

            try
            {
                bool result = Core.Actions.PowerOff(model);
                ev.ActionStatus = ActionStatus.OK;
                LogsController.AddEvent(ev, User.Identity.GetUserId());
                return result;
            }
            catch (Exception ex)
            {
                ev.ActionStatus = ActionStatus.Error;
                ev.Exception = ex.ToString();
                LogsController.AddEvent(ev, User.Identity.GetUserId());
            }
            return false;
        }

        /// <summary>
        /// GET | Reboot a computer by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [System.Web.Mvc.Authorize]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public bool PowerRecycle(int id)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            ComputerModel model = db.Computers.SingleOrDefault(x => x.Id == id);
            // Event
            SysEvent ev = new SysEvent();
            ev.Action = Enums.Action.Power;
            ev.Description = "Rebooted: " + model.Name;

            try
            {
                bool result = Core.Actions.PowerRecycle(model);
                ev.ActionStatus = ActionStatus.OK;
                LogsController.AddEvent(ev, User.Identity.GetUserId());
                return result;
            }
            catch (Exception ex)
            {
                ev.ActionStatus = ActionStatus.Error;
                ev.Exception = ex.ToString();
                LogsController.AddEvent(ev, User.Identity.GetUserId());
            }
            return false;
        }
    }
}