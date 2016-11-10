using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Web;
using System.Web.Mvc;
using CMRPS.Web.Controllers;
using CMRPS.Web.Enums;
using CMRPS.Web.Models;
using Microsoft.AspNet.Identity;

namespace CMRPS.Web.Core
{
    public partial class Actions
    {
        public static bool Ping(ComputerModel computer)
        {
            Ping pinger = new Ping();
            try
            {
                // Ping computer
                PingReply reply = pinger.Send(computer.Hostname);
                return reply.Status == IPStatus.Success;
            }
            catch (PingException)
            {
                return false;
            }
        }

        /// <summary>
        /// User | Ping a computer via hostname or IP.
        /// </summary>
        /// <param name="hostname"></param>
        /// <returns></returns>
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public static bool Ping(string hostname)
        {
            Ping pinger = new Ping();
            try
            {
                // Ping computer
                PingReply reply = pinger.Send(hostname);
                return reply.Status == IPStatus.Success;
            }
            catch (PingException ex)
            {
                throw ex;
            }
        }
    }
}