using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using CMPRS.Web.Enums;

namespace CMPRS.Web.Models
{
    [Table("SiteSettings")]
    public class SettingsModel
    {
        public int Id { get; set; }

        // Administrator Credetials
        [Display(Name = "Administrator Username")]
        public string AdminUsername { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Administrator Password")]
        public string AdminPassword { get; set; }

        [Display(Name = "Administrator Domain")]
        public string AdminDomain { get; set; }

        // Shutdown
        [Display(Name = "Method")]
        public ShutdownMethod ShutdownMethod { get; set; }

        [Display(Name = "Force")]
        public bool ShutdownForce { get; set; }
        
        //CMD only
        [Display(Name = "Timeout (Only CMD)")]
        public int ShutdownTimeout { get; set; }

        [Display(Name = "Message (Only CMD)")]
        public string ShutdownMessage { get; set; }

        // Reboot
        [Display(Name = "Method")]
        public RebootMethod RebootMethod { get; set; }

        [Display(Name = "Force")]
        public bool RebootForce { get; set; }
        
        //CMD only
        [Display(Name = "Timeout (Only CMD)")]
        public int RebootTimeout { get; set; }

        [Display(Name = "Message (Only CMD)")]
        public string RebootMessage { get; set; }

        // Startup
        [Display(Name = "Method")]
        public StartupMethod StartupMethod { get; set; }

        // HangFire
        [Display(Name = "Interval in minutes to ping")]
        public int PingInterval { get; set; }

        [Display(Name = "Keep logs clean")]
        public bool CleanLogs { get; set; }

        [Display(Name = "Keep for (days)")]
        public int KeepLogsFor { get; set; }
    }
}