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
        //private static ApplicationDbContext db = new ApplicationDbContext();
        private static string exception = "";

        // Declarations of imports
        // ARP
        [DllImport("iphlpapi.dll", ExactSpelling = true)]
        public static extern int SendARP(int DestIP, int SrcIP, [Out] byte[] pMacAddr, ref int PhyAddrLen);

        // Flush DNS
        [DllImport("dnsapi.dll", EntryPoint = "DnsFlushResolverCache")]
        private static extern UInt32 DnsFlushResolverCache();

        // =======================================================================================
        // Public Methods
        // =======================================================================================

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

        /// <summary>
        /// HangFire | Called to update computer info.
        /// </summary>
        /// <param name="ComputerID"></param>
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public static void UpdateComputer(int id)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            ComputerModel computer = db.Computers.Single(x => x.Id == id);

            if (computer != null)
            {
                // Ping computer
                computer.Status = Ping(computer);
                // Get Info (but only if it is online)
                if (computer.Status)
                {
                    computer.LastSeen = DateTime.Now;
                    computer.IP = GetIP(computer);
                    computer.MAC = GetMAC(computer);
                }
                else
                {
                    if (computer.LastSeen.Year < 1990)
                    {
                        computer.LastSeen = computer.PurchaseDate;
                    }
                }
                // Update Database
                db.Computers.AddOrUpdate(computer);
                db.SaveChanges();

                // Call SignalR
                var context = GlobalHost.ConnectionManager.GetHubContext<LiveUpdatesHub>();
                context.Clients.All.UpdateListView(id, computer.Status, computer.IP, computer.MAC);
                context.Clients.All.UpdateOverview(id, computer.Status, computer.IP, computer.MAC);
                context.Clients.All.UpdateComputers(id, computer.Status, computer.IP, computer.MAC);
            }
        }

        public static void ScheduleExecute(int id)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            ScheduledModel schedule = db.Schedules.SingleOrDefault(x => x.Id == id);
            List<int> computerIds = new List<int>();

            if (schedule.Type == ScheduledType.Individual)
            {
                computerIds = JsonConvert.DeserializeObject<List<int>>(schedule.JsonComputerList);
            }
            else if (schedule.Type == ScheduledType.Color)
            {
                computerIds = db.Colors.SingleOrDefault(x => x.Id == schedule.ColorId).Computers.Select(x => x.Id).ToList();
            }
            else if (schedule.Type == ScheduledType.Location)
            {
                computerIds = db.Locations.SingleOrDefault(x => x.Id == schedule.LocationId).Computers.Select(x => x.Id).ToList();
            }
            else if (schedule.Type == ScheduledType.Type)
            {
                computerIds = db.ComputerTypes.SingleOrDefault(x => x.Id == schedule.TypeId).Computers.Select(x => x.Id).ToList();
            }

            foreach (int cid in computerIds)
            {
                try
                {
                    if (schedule.Action == ScheduledAction.Wakeup)
                    {
                        SchedulerPowerOn(cid);
                    }
                    else if (schedule.Action == ScheduledAction.Reboot)
                    {
                        SchedulerPowerRecycle(cid);
                    }
                    else if (schedule.Action == ScheduledAction.Shutdown)
                    {
                        SchedulerPowerOff(cid);
                    }
                }
                catch (Exception ex)
                {
                    var brk = 0;
                }
                
            }

            schedule.LastRun = DateTime.Now;
            db.Schedules.AddOrUpdate(schedule);
            db.SaveChanges();
        }

        // ======================================================================================
        // System Methods
        // ======================================================================================
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
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }

        public static bool SchedulerPowerRecycle(int id)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            ComputerModel model = db.Computers.Single(x => x.Id == id);
            SettingsModel settings = db.Settings.FirstOrDefault();

            try
            {
                if (settings.RebootMethod == RebootMethod.CMD)
                {
                    if (settings.AdminUsername.Length > 0 && settings.AdminPassword.Length > 0)
                    {
                        RestartCMDCredentials(
                            model.Hostname,
                            settings.ShutdownTimeout,
                            settings.ShutdownForce,
                            settings.ShutdownMessage,
                            settings.AdminUsername,
                            settings.AdminPassword,
                            settings.AdminDomain
                            );
                    }
                    else
                    {
                        RestartCMD(
                            model.Hostname,
                            settings.ShutdownTimeout,
                            settings.ShutdownForce,
                            settings.ShutdownMessage
                            );
                    }
                }
                else if (settings.RebootMethod == RebootMethod.WMI)
                {
                    RestartWMI(
                        model.Hostname,
                        settings.ShutdownForce,
                        settings.AdminUsername,
                        settings.AdminPassword,
                        settings.AdminDomain
                        );
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public static bool SchedulerPowerOff(int id)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            ComputerModel model = db.Computers.Single(x => x.Id == id);
            SettingsModel settings = db.Settings.FirstOrDefault();

            try
            {
                if (settings.ShutdownMethod == ShutdownMethod.CMD)
                {
                    if (settings.AdminUsername.Length > 0 && settings.AdminPassword.Length > 0)
                    {
                        ShutdownCMDCredentials(
                            model.Hostname,
                            settings.ShutdownTimeout,
                            settings.ShutdownForce,
                            settings.ShutdownMessage,
                            settings.AdminUsername,
                            settings.AdminPassword,
                            settings.AdminDomain
                            );
                    }
                    else
                    {
                        ShutdownCMD(
                            model.Hostname,
                            settings.ShutdownTimeout,
                            settings.ShutdownForce,
                            settings.ShutdownMessage
                            );
                    }
                }
                else if (settings.ShutdownMethod == ShutdownMethod.WMI)
                {
                    ShutdownWMI(
                        model.Hostname,
                        settings.ShutdownForce,
                        settings.AdminUsername,
                        settings.AdminPassword,
                        settings.AdminDomain
                        );
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        // ======================================================================================
        // User Methods
        // ======================================================================================
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public bool Ping(string hostname)
        {
            // Event
            SysEvent ev = new SysEvent();
            ev.Action = Enums.Action.Power;
            ev.Description = "Pinged: " + hostname;

            Ping pinger = new Ping();
            try
            {
                // Ping computer
                PingReply reply = pinger.Send(hostname);

                ev.ActionStatus = ActionStatus.OK;
                LogsController.AddEvent(ev, User.Identity.GetUserId());

                return reply.Status == IPStatus.Success;
            }
            catch (PingException ex)
            {
                ev.ActionStatus = ActionStatus.Error;
                ev.Exception = ex.ToString();
                exception = "";
                LogsController.AddEvent(ev, User.Identity.GetUserId());
                return false;
            }
        }

        [System.Web.Mvc.Authorize]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public bool PowerOn(int id)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            ComputerModel model = db.Computers.Single(x => x.Id == id);
            SettingsModel settings = db.Settings.FirstOrDefault();

            // Event
            SysEvent ev = new SysEvent();
            ev.Action = Enums.Action.Power;
            ev.Description = "Powered on: " + model.Name;


            try
            {
                if (settings.StartupMethod == StartupMethod.Winwake)
                {
                    bool result = WakeupWinwake(model.MAC);
                    ev.ActionStatus = result ? ActionStatus.OK : ActionStatus.Error;
                    ev.Exception = exception.ToString();
                    exception = "";
                    LogsController.AddEvent(ev, User.Identity.GetUserId());
                    return result;
                }
                if (settings.StartupMethod == StartupMethod.Packet)
                {
                    bool result = WakeupPacket(model.MAC);
                    ev.ActionStatus = result ? ActionStatus.OK : ActionStatus.Error;
                    ev.Exception = exception.ToString();
                    exception = "";
                    LogsController.AddEvent(ev, User.Identity.GetUserId());
                    return result;
                }
            }
            catch (Exception ex)
            {
                ev.ActionStatus = ActionStatus.Error;
                ev.Exception = ex.ToString();
                LogsController.AddEvent(ev, User.Identity.GetUserId());
                exception = "";
                return false;
            }

            return true;
        }

        [System.Web.Mvc.Authorize]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public bool PowerOff(int id)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            ComputerModel model = db.Computers.Single(x => x.Id == id);
            SettingsModel settings = db.Settings.FirstOrDefault();

            // Event
            SysEvent ev = new SysEvent();
            ev.Action = Enums.Action.Power;
            ev.Description = "Powered off: " + model.Name;

            try
            {
                if (settings.ShutdownMethod == ShutdownMethod.CMD)
                {
                    if (settings.AdminUsername.Length > 0 && settings.AdminPassword.Length > 0)
                    {
                        ShutdownCMDCredentials(
                            model.Hostname,
                            settings.ShutdownTimeout,
                            settings.ShutdownForce,
                            settings.ShutdownMessage,
                            settings.AdminUsername,
                            settings.AdminPassword,
                            settings.AdminDomain
                            );
                    }
                    else
                    {
                        ShutdownCMD(
                            model.Hostname,
                            settings.ShutdownTimeout,
                            settings.ShutdownForce,
                            settings.ShutdownMessage
                            );
                    }
                }
                else if (settings.ShutdownMethod == ShutdownMethod.WMI)
                {
                    ShutdownWMI(
                        model.Hostname,
                        settings.ShutdownForce,
                        settings.AdminUsername,
                        settings.AdminPassword,
                        settings.AdminDomain
                        );
                }


            }
            catch (Exception ex)
            {
                ev.ActionStatus = ActionStatus.Error;
                ev.Exception = ex.ToString();
                LogsController.AddEvent(ev, User.Identity.GetUserId());
                exception = "";
                return false;
            }
            ev.ActionStatus = ActionStatus.OK;
            LogsController.AddEvent(ev, User.Identity.GetUserId());
            return true;
        }

        [System.Web.Mvc.Authorize]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public bool PowerRecycle(int id)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            ComputerModel model = db.Computers.Single(x => x.Id == id);
            SettingsModel settings = db.Settings.FirstOrDefault();

            // Event
            SysEvent ev = new SysEvent();
            ev.Action = Enums.Action.Power;
            ev.Description = "Rebooted: " + model.Name;

            try
            {
                if (settings.RebootMethod == RebootMethod.CMD)
                {
                    if (settings.AdminUsername.Length > 0 && settings.AdminPassword.Length > 0)
                    {
                        RestartCMDCredentials(
                            model.Hostname,
                            settings.ShutdownTimeout,
                            settings.ShutdownForce,
                            settings.ShutdownMessage,
                            settings.AdminUsername,
                            settings.AdminPassword,
                            settings.AdminDomain
                            );
                    }
                    else
                    {
                        RestartCMD(
                            model.Hostname,
                            settings.ShutdownTimeout,
                            settings.ShutdownForce,
                            settings.ShutdownMessage
                            );
                    }
                }
                else if (settings.RebootMethod == RebootMethod.WMI)
                {
                    RestartWMI(
                        model.Hostname,
                        settings.ShutdownForce,
                        settings.AdminUsername,
                        settings.AdminPassword,
                        settings.AdminDomain
                        );
                }
            }
            catch (Exception ex)
            {
                ev.ActionStatus = ActionStatus.Error;
                ev.Exception = ex.ToString();
                LogsController.AddEvent(ev, User.Identity.GetUserId());
                exception = "";
                return false;
            }

            ev.ActionStatus = ActionStatus.OK;
            LogsController.AddEvent(ev, User.Identity.GetUserId());
            return true;
        }

        // =======================================================================================
        // Ping Methods
        // =======================================================================================
        private static bool Ping(ComputerModel computer)
        {
            Ping pinger = new Ping();
            try
            {
                // Ping computer
                PingReply reply = pinger.Send(computer.Hostname);
                return reply.Status == IPStatus.Success;
            }
            catch (PingException ex)
            {
                return false;
            }
        }

        private static bool GetStatus(PingReply ping)
        {
            try
            {
                return ping.Status == IPStatus.Success;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static string GetIP(ComputerModel computer)
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

        private static string GetMAC(ComputerModel computer)
        {
            try
            {
                // Parse IP
                IPAddress address = IPAddress.Parse(computer.IP);
                // Get INT
                int intAddress = BitConverter.ToInt32(address.GetAddressBytes(), 0);

                byte[] macAddr = new byte[6];
                int macAddrLen = (int)macAddr.Length;

                if (SendARP(intAddress, 0, macAddr, ref macAddrLen) != 0)
                    return computer.MAC;

                StringBuilder macAddressString = new StringBuilder();
                for (int i = 0; i < macAddr.Length; i++)
                {
                    if (macAddressString.Length > 0)
                        macAddressString.Append(":");

                    macAddressString.AppendFormat("{0:x2}", macAddr[i]);
                }
                return macAddressString.ToString().ToUpper();
            }
            catch (Exception)
            {
                return computer.MAC;
            }
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
                exception = ex.ToString();
            }
            return false;

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
                    //set sending bites
                    int counter = 0;
                    //buffer to be send
                    byte[] bytes = new byte[1024]; // more than enough :-)
                    //first 6 bytes should be 0xFF
                    for (int y = 0; y < 6; y++)
                        bytes[counter++] = 0xFF;
                    //now repeate MAC 16 times
                    for (int y = 0; y < 16; y++)
                    {
                        int i = 0;
                        for (int z = 0; z < 6; z++)
                        {
                            bytes[counter++] =
                                byte.Parse(macAdr.Substring(i, 2),
                                    NumberStyles.HexNumber);
                            i += 2;
                        }
                    }

                    // Now send wake up packet
                    int returnValue = client.Send(bytes, 1024);

                    // Check if all bytes were sent OK.
                    bool length = returnValue == bytes.Length;
                    return true;
                }
                catch (Exception ex)
                {
                    exception = ex.ToString();
                    return false;
                }
            }
            else
            {
                return false;
            }

        }

        // =======================================================================================
        // Power OFF Methods
        // =======================================================================================
        private static bool ShutdownCMD(string hostname, int timeout, bool force, string message)
        {
            string useForce = force ? "-f" : "";
            string useMessage = message.Length > 0 ? "-c " + message : "";
            try
            {
                string args = String.Format("-s -m \\\\{0} -t {1} {2} {3}", hostname, timeout, useForce, useMessage);
                Process.Start("shutdown", args);
            }
            catch (Exception ex)
            {
                exception = ex.ToString();
            }
            return false;
        }

        private static bool ShutdownCMDCredentials(string hostname, int timeout, bool force, string message, string username, string password, string domain)
        {
            try
            {
                // Make secure password
                SecureString securePassword = new SecureString();
                foreach (char c in password)
                {
                    securePassword.AppendChar(c);
                }

                // Create arguments
                string useForce = force ? "-f" : "";
                string useMessage = message.Length > 0 ? "-c " + message : "";
                string args = String.Format("-s -m \\\\{0} -t {1} {2} {3}", hostname, timeout, useForce, useMessage);

                // Create the process
                Process psi = new Process();
                psi.StartInfo.UseShellExecute = false;
                psi.StartInfo.CreateNoWindow = false;
                psi.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                psi.StartInfo.RedirectStandardOutput = true;
                psi.StartInfo.UserName = username;
                psi.StartInfo.Password = securePassword;
                psi.StartInfo.Domain = domain;
                psi.StartInfo.FileName = @"shutdown.exe";
                psi.StartInfo.Arguments = args;

                // Start the process and get output
                psi.Start();
                string output = psi.StandardOutput.ReadToEnd();
                psi.WaitForExit();

                return true;
            }
            catch (Exception ex)
            {
                exception = ex.ToString();
            }
            return false;
        }

        private static bool ShutdownWMI(string hostname, bool force, string username, string password, string domain)
        {
            try
            {
                ConnectionOptions options = new ConnectionOptions();
                options.EnablePrivileges = true;

                options.Username = username;
                options.Password = password;
                options.Authority = "ntlmdomain:" + domain;

                ManagementScope scope = new ManagementScope("\\\\" + hostname + "\\root\\CIMV2", options);
                scope.Connect();

                SelectQuery query = new SelectQuery("Win32_OperatingSystem");
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);

                foreach (ManagementObject os in searcher.Get())
                {
                    // Obtain in-parameters for the method
                    ManagementBaseObject inParams = os.GetMethodParameters("Win32Shutdown");

                    // LogOff = 0,
                    // ForcedLogOff = 4,
                    // Shutdown = 1,
                    // ForcedShutdown = 5,
                    // Reboot = 2,
                    // ForcedReboot = 6,
                    // PowerOff = 8,
                    // ForcedPowerOff = 12
                    inParams["Flags"] = force ? 5 : 1;

                    // Execute the method and obtain the return values.
                    ManagementBaseObject outParams = os.InvokeMethod("Win32Shutdown", inParams, null);
                }
                return true;
            }
            catch (Exception ex)
            {
                exception = ex.ToString();
            }
            return false;
        }

        // =======================================================================================
        // Power Recycle Methods
        // =======================================================================================
        private static bool RestartCMD(string hostname, int timeout, bool force, string message)
        {
            string useForce = force ? "-f" : "";
            string useMessage = message.Length > 0 ? "-c " + message : "";
            try
            {
                string args = String.Format("-r -m \\\\{0} -t {1} {2} {3}", hostname, timeout, useForce, useMessage);
                Process.Start("shutdown", args);
            }
            catch (Exception ex)
            {
                exception = ex.ToString();
            }
            return false;
        }

        private static bool RestartCMDCredentials(string hostname, int timeout, bool force, string message, string username, string password, string domain)
        {
            try
            {
                // Make secure password
                SecureString securePassword = new SecureString();
                foreach (char c in password)
                {
                    securePassword.AppendChar(c);
                }

                // Create arguments
                string useForce = force ? "-f" : "";
                string useMessage = message.Length > 0 ? "-c " + message : "";
                string args = String.Format("-r -m \\\\{0} -t {1} {2} {3}", hostname, timeout, useForce, useMessage);

                // Create the process
                Process psi = new Process();
                psi.StartInfo.UseShellExecute = false;
                psi.StartInfo.CreateNoWindow = false;
                psi.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                psi.StartInfo.RedirectStandardOutput = true;
                psi.StartInfo.UserName = username;
                psi.StartInfo.Password = securePassword;
                psi.StartInfo.Domain = domain;
                psi.StartInfo.FileName = @"shutdown.exe";
                psi.StartInfo.Arguments = args;

                // Start the process and get output
                psi.Start();
                string output = psi.StandardOutput.ReadToEnd();
                psi.WaitForExit();

                return true;
            }
            catch (Exception ex)
            {
                exception = ex.ToString();
            }
            return false;
        }

        private static bool RestartWMI(string hostname, bool force, string username, string password, string domain)
        {
            try
            {
                ConnectionOptions options = new ConnectionOptions();
                options.EnablePrivileges = true;

                options.Username = username;
                options.Password = password;
                options.Authority = "ntlmdomain:" + domain;

                ManagementScope scope = new ManagementScope("\\\\" + hostname + "\\root\\CIMV2", options);
                scope.Connect();

                SelectQuery query = new SelectQuery("Win32_OperatingSystem");
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);

                foreach (ManagementObject os in searcher.Get())
                {
                    // Obtain in-parameters for the method
                    ManagementBaseObject inParams = os.GetMethodParameters("Win32Shutdown");

                    // LogOff = 0,
                    // ForcedLogOff = 4,
                    // Shutdown = 1,
                    // ForcedShutdown = 5,
                    // Reboot = 2,
                    // ForcedReboot = 6,
                    // PowerOff = 8,
                    // ForcedPowerOff = 12
                    inParams["Flags"] = force ? 6 : 2;

                    // Execute the method and obtain the return values.
                    ManagementBaseObject outParams = os.InvokeMethod("Win32Shutdown", inParams, null);
                }
                return true;
            }
            catch (Exception ex)
            {
                exception = ex.ToString();
            }
            return false;
        }
    }
}