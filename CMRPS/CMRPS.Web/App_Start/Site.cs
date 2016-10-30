using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using CMPRS.Web.Models;
using CMRPS.Web.Models;

namespace CMPRS.Web.App_Start
{
    public static class Site
    {
        private static ApplicationDbContext db = new ApplicationDbContext();
        public static SettingsModel Settings = new SettingsModel();

        public static bool SettingsLoad()
        {
            try
            {
                Settings = db.Settings.FirstOrDefault();
                if (Settings == null)
                {
                    Settings = new SettingsModel();
                    db.Settings.Add(Settings);
                    db.SaveChanges();
                }    
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
    }
}