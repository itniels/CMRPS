using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Security;
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
        /// Scheduler | Power Recycle.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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

        /// <summary>
        /// User | Reboot a computer in the system.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool PowerRecycle(ComputerModel model)
        {
            ApplicationDbContext db = new ApplicationDbContext();
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
            catch (Exception ex)
            {
                throw ex;
            }
            return true;
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
                throw ex;
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
                throw ex;
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
                throw ex;
            }
            return false;
        }
    }
}