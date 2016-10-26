using System;
using System.Collections.Generic;
using System.Configuration;
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
using System.Web;
using System.Web.Mvc;
using CMPRS.Web.App_Start;
using CMPRS.Web.Enums;
using CMPRS.Web.Models;
using CMRPS.Web.Models;

namespace CMRPS.Web.Controllers
{
    public class ActionController : Controller
    {
        private static ApplicationDbContext db = new ApplicationDbContext();

        // Declarations of imports
        [DllImport("iphlpapi.dll", ExactSpelling = true)]
        public static extern int SendARP(int DestIP, int SrcIP, [Out] byte[] pMacAddr, ref int PhyAddrLen);

        // =======================================================================================
        // Public Methods
        // =======================================================================================
        public static bool Ping(int ComputerID)
        {
            ComputerModel computer = db.Computers.Single(x => x.Id == ComputerID);

            if (computer != null)
            {
                Ping pinger = new Ping();
                try
                {
                    // Ping computer
                    PingReply reply = pinger.Send(computer.Hostname);
                    //Update database
                    computer.IP = GetIP(computer.Hostname, computer.IP);
                    computer.Status = GetStatus(reply);
                    computer.MAC = GetMAC(computer.IP, computer.MAC);
                }
                catch (PingException)
                {
                    computer.Status = false;
                }
                db.Computers.AddOrUpdate(computer);
                db.SaveChanges();
                return computer.Status;
            }
            return false;
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

        private static string GetIP(string hostname, string oldIP)
        {
            try
            {
                var addresses = Dns.GetHostAddresses(hostname);
                foreach (IPAddress address in addresses)
                {
                    if (address.AddressFamily == AddressFamily.InterNetwork)
                        return address.ToString();
                }
                return "No IPv4 address found!";
            }
            catch (Exception)
            {
                return oldIP;
            }
        }

        

        private static string GetMAC(string IP, string oldMAC)
        {
            try
            {
                // Parse IP
                IPAddress address = IPAddress.Parse(IP);
                // Get INT
                int intAddress = BitConverter.ToInt32(address.GetAddressBytes(), 0);

                byte[] macAddr = new byte[6];
                int macAddrLen = (int)macAddr.Length;

                if (SendARP(intAddress, 0, macAddr, ref macAddrLen) != 0)
                    return oldMAC;

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
                return oldMAC;
            }
        }

        public static bool PowerOn(ComputerModel computer)
        {
            SettingsModel settings = db.Settings.FirstOrDefault();
            try
            {
                if (settings.StartupMethod == StartupMethod.Winwake)
                {
                    return WakeupWinwake(computer.MAC);
                }
                else if (settings.StartupMethod == StartupMethod.Packet)
                {
                    return WakeupPacket(computer.MAC);
                }
            }
            catch (Exception){ }
            
            return false;
        }

        public static bool PowerOff(ComputerModel computer)
        {
            SettingsModel settings = db.Settings.FirstOrDefault();
            try
            {
                if (settings.ShutdownMethod == ShutdownMethod.CMD)
                {
                    if (settings.AdminUsername.Length > 0 && settings.AdminPassword.Length > 0)
                    {
                        ShutdownCMDCredentials(
                            computer.Hostname,
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
                            computer.Hostname,
                            settings.ShutdownTimeout,
                            settings.ShutdownForce,
                            settings.ShutdownMessage
                            );
                    }
                }
                else if (settings.ShutdownMethod == ShutdownMethod.WMI)
                {
                    ShutdownWMI(
                        computer.Hostname,
                        settings.ShutdownForce, 
                        settings.AdminUsername,
                        settings.AdminPassword, 
                        settings.AdminDomain
                        );
                }
            }
            catch (Exception) { }

            return false;
        }

        public static bool PowerRecycle(ComputerModel computer)
        {
            SettingsModel settings = db.Settings.FirstOrDefault();
            try
            {
                if (settings.RebootMethod == RebootMethod.CMD)
                {
                    if (settings.AdminUsername.Length > 0 && settings.AdminPassword.Length > 0)
                    {
                        RestartCMDCredentials(
                            computer.Hostname,
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
                            computer.Hostname,
                            settings.ShutdownTimeout,
                            settings.ShutdownForce,
                            settings.ShutdownMessage
                            );
                    }
                }
                else if (settings.RebootMethod == RebootMethod.WMI)
                {
                    RestartWMI(
                        computer.Hostname,
                        settings.ShutdownForce,
                        settings.AdminUsername,
                        settings.AdminPassword,
                        settings.AdminDomain
                        );
                }
            }
            catch (Exception) { }

            return false;
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
            catch (Exception) { }
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
                catch (Exception)
                {
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
            catch (Exception){ }
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
            catch (Exception){ }
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
            catch (Exception){ }
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
            catch (Exception) { }
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
            catch (Exception) { }
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
            catch (Exception) { }
            return false;
        }
    }
}