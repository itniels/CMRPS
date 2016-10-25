using CMPRS.Web.Models;
using CMRPS.Web.Models;

namespace CMRPS.Web.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<CMRPS.Web.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "CMRPS.Web.Models.ApplicationDbContext";
        }

        protected override void Seed(CMRPS.Web.Models.ApplicationDbContext context)
        {
            // Default settings
            context.Settings.AddOrUpdate(x => x.Id,
                new SettingsModel
                {
                    Id = 1,
                    AdminUsername = "",
                    AdminPassword = "",
                    AdminDomain = "",
                    StartupMethod = 0,
                    ShutdownMethod = 0,
                    ShutdownForce = true,
                    ShutdownMessage = "Shutdown in 2 minutes by CMRPS.",
                    ShutdownTimeout = 120,
                    RebootMethod = 0,
                    RebootForce = true,
                    RebootMessage = "Rebooting in 2 minutes by CMRPS.",
                    RebootTimeout = 120,
                    PingInterval = 1,
                }

            );

            
        }
    }
}
