using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Web;
using CMRPS.Web.Models;

namespace CMRPS.Web.Core
{
    public partial class Actions
    {
        /// <summary>
        /// HangFire | Gets the IP.
        /// </summary>
        /// <param name="computer"></param>
        /// <returns></returns>
        public static string GetIP(ComputerModel computer)
        {
            try
            {
                var addresses = Dns.GetHostAddresses(computer.Hostname);
                foreach (IPAddress address in addresses)
                {
                    if (address.AddressFamily == AddressFamily.InterNetwork)
                        return address.ToString();
                }
                return computer.IP;
            }
            catch (Exception)
            {
                return computer.IP;
            }
        }
    }
}