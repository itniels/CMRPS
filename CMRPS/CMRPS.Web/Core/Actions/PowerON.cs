using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Web;
using System.Web.Mvc;
using CMPRS.Web.Enums;
using CMPRS.Web.Models;
using CMRPS.Web.Controllers;
using CMRPS.Web.Enums;
using CMRPS.Web.Models;
using Microsoft.AspNet.Identity;

namespace CMRPS.Web.Core
{
    public partial class Actions
    {
        /// <summary>
        /// Scheduler | Power on.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool SchedulerPowerOn(int id)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            ComputerModel model = db.Computers.Single(x => x.Id == id);
            SettingsModel settings = db.Settings.FirstOrDefault();

            try
            {
                if (settings.StartupMethod == StartupMethod.Winwake)
                {
                    return WakeupWinwake(model.MAC);
                }
                if (settings.StartupMethod == StartupMethod.Packet)
                {
                    return WakeupPacket(model.MAC);
                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// User | Power on a computer in the system.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool PowerOn(ComputerModel model)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            SettingsModel settings = db.Settings.FirstOrDefault();

            try
            {
                if (settings.StartupMethod == StartupMethod.Winwake)
                {
                    return WakeupWinwake(model.MAC);
                }
                if (settings.StartupMethod == StartupMethod.Packet)
                {
                    return WakeupPacket(model.MAC);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return true;
        }

        // =======================================================================================
        // Power ON Methods
        // =======================================================================================

        private static bool WakeupWinwake(string mac)
        {
            // Clean MAC
            string cleanMac = mac.Replace(":", "").Replace("-", "");
            try
            {
                string args = String.Format("{0}", cleanMac);
                string path = @"c:\WINWAKE.exe";
                Process.Start(path, args);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        private static bool WakeupPacket(string mac)
        {
            IPAddress broadcast = IPAddress.Parse("255.255.255.255");
            Int32 port = 0x2fff; // port=12287 let's use this one 

            if (mac.Length > 0)
            {
                // Clean MAC.
                string macAdr = mac.Replace(":", "").Replace("-", "");
                try
                {
                    UdpClient client = new UdpClient();
                    client.Connect(broadcast, port);
                    client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 0);
                    // Set sending bites
                    int counter = 0;
                    // Buffer to be send
                    byte[] bytes = new byte[1024]; // more than enough :-)
                    // First 6 bytes should be 0xFF
                    for (int i = 0; i < 6; i++)
                        bytes[counter++] = 0xFF;
                    // Now repeate MAC 16 times
                    for (int i = 0; i < 16; i++)
                    {
                        int y = 0;
                        for (int z = 0; z < 6; z++)
                        {
                            bytes[counter++] =
                                byte.Parse(macAdr.Substring(y, 2),
                                    NumberStyles.HexNumber);
                            y += 2;
                        }
                    }

                    // Now send wake up packet
                    int returnValue = client.Send(bytes, 1024);

                    // Check if all bytes were sent OK.
                    return returnValue == bytes.Length;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            else
            {
                return false;
            }

        }
    }
}