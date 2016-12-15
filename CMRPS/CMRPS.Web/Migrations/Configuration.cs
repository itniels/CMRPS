using CMPRS.Web.Enums;
using CMPRS.Web.Models;
using CMRPS.Web.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

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
                    // Credentials
                    AdminUsername = "",
                    AdminPassword = "",
                    AdminDomain = "",
                    // Startup
                    StartupMethod = 0,
                    // Shutdown
                    ShutdownMethod = ShutdownMethod.CMD,
                    ShutdownForce = true,
                    ShutdownMessage = "Shutdown in 2 minutes by CMRPS.",
                    ShutdownTimeout = 120,
                    // Reboot
                    RebootMethod = RebootMethod.CMD,
                    RebootForce = true,
                    RebootMessage = "Rebooting in 2 minutes by CMRPS.",
                    RebootTimeout = 120,
                    // HangFire
                    PingInterval = 1,
                    WorkerQueues = 10,  // Number of queues to start work
                    CleanLogs = true,   // Keep logs clear
                    KeepLogsFor = 90,   // 3 Months
                }
            );

            // Users
            if (!context.Users.Any(u => u.UserName == "eaaa"))
            {
                var store = new UserStore<ApplicationUser>(context);
                var manager = new UserManager<ApplicationUser>(store);
                var user = new ApplicationUser { UserName = "eaaa", Email = "eaaa@cmrps.dk", Firstname = "EAAA", Lastname = "User"};

                manager.Create(user, "Eaaa2016");
            }
            if (!context.Users.Any(u => u.UserName == "user"))
            {
                var store = new UserStore<ApplicationUser>(context);
                var manager = new UserManager<ApplicationUser>(store);
                var user = new ApplicationUser { UserName = "user", Email = "user@cmrps.dk", Firstname = "Demo", Lastname = "User" };

                manager.Create(user, "Eaaa2016");
            }

            // Colors
            context.Colors.AddOrUpdate(x => x.Name,
                new ColorModel
                {
                    Name = "Black/White",
                    ColorLabel = "#404040",
                    ColorText = "#ffffff",
                },
                new ColorModel
                {
                    Name = "Blue/White",
                    ColorLabel = "#5e86ff",
                    ColorText = "#ffffff",
                },
                new ColorModel
                {
                    Name = "Pink/White",
                    ColorLabel = "#ff80b8",
                    ColorText = "#ffffff",
                },
                new ColorModel
                {
                    Name = "White/Black",
                    ColorLabel = "#ffffff",
                    ColorText = "#202020",
                });

            // Locations
            context.Locations.AddOrUpdate(x => x.Location,
                new LocationModel{ Location = "Lokale A12"},
                new LocationModel { Location = "Lokale A13" },
                new LocationModel { Location = "Lokale B12" },
                new LocationModel { Location = "Bibliotek" },
                new LocationModel { Location = "Datacenter" }
                );

            // Computer Types
            context.ComputerTypes.AddOrUpdate(x => x.Name,
                new ComputerTypeModel
                {
                    Name = "Desktop",
                    ImagePath = "/Content/TypeImages/Desktop.png",
                    Filename = "Desktop.png"
                },
                new ComputerTypeModel
                {
                    Name = "Laptop",
                    ImagePath = "/Content/TypeImages/Laptop.png",
                    Filename = "Laptop.png"
                });
            context.SaveChanges();
            // Computers

            context.Computers.AddOrUpdate(x => x.Name,
                new ComputerModel
                {
                    Name = "CMRPS Server",
                    Hostname = "cmrps.nsit.dk",
                    Price = 495.00,
                    PurchaseDate = DateTime.Now,
                    LastSeen = new DateTime(2016,1,1),
                    Type = context.ComputerTypes.SingleOrDefault(x => x.Name == "Desktop"),
                    Color = context.Colors.SingleOrDefault(x => x.Name == "Blue/White"),
                    Location = context.Locations.SingleOrDefault(x => x.Location == "Datacenter")
                },
                new ComputerModel
                {
                    Name = "Demo Laptop",
                    Hostname = "demo-laptop",
                    Price = 4999.00,
                    PurchaseDate = DateTime.Now,
                    LastSeen = new DateTime(2016, 1, 1),
                    Type = context.ComputerTypes.SingleOrDefault(x => x.Name == "Laptop"),
                    Color = context.Colors.SingleOrDefault(x => x.Name == "Black/White"),
                    Location = context.Locations.SingleOrDefault(x => x.Location == "Lokale A12")
                },
                new ComputerModel
                {
                    Name = "Demo Desktop",
                    Hostname = "demo-desktop",
                    Price = 3999.00,
                    PurchaseDate = DateTime.Now,
                    LastSeen = new DateTime(2016, 1, 1),
                    Type = context.ComputerTypes.SingleOrDefault(x => x.Name == "Desktop"),
                    Color = context.Colors.SingleOrDefault(x => x.Name == "Black/White"),
                    Location = context.Locations.SingleOrDefault(x => x.Location == "Lokale A13")
                });
        }
    }
}
