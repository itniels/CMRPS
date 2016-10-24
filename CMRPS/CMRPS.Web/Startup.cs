using System.Linq;
using CMPRS.Web.App_Start;
using CMPRS.Web.Models;
using CMRPS.Web.Controllers;
using CMRPS.Web.Models;
using Hangfire;
using Hangfire.Common;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CMRPS.Web.Startup))]
namespace CMRPS.Web
{
    public partial class Startup
    {
        ApplicationDbContext db = new ApplicationDbContext();
        public void Configuration(IAppBuilder app)
        {
            GlobalConfiguration.Configuration.UseSqlServerStorage("DefaultConnection");

            Site.SettingsLoad();
            ConfigureAuth(app);
            app.MapSignalR();
            app.UseHangfireDashboard();
            app.UseHangfireServer();

            // Add hangfire jobs
            SettingsModel settings = db.Settings.SingleOrDefault(x => x.Id == 1);
            var manager = new RecurringJobManager();
            if (settings != null)
            {
                manager.AddOrUpdate("Ping", Job.FromExpression(() => JobsController.Ping()), Cron.MinuteInterval(settings.PingInterval));
                //RecurringJob.AddOrUpdate(() => JobsController.Ping(), Cron.MinuteInterval(settings.PingInterval));
            }
            else
            {
                manager.AddOrUpdate("Ping", Job.FromExpression(() => JobsController.Ping()), Cron.Minutely());
                //RecurringJob.AddOrUpdate(() => "Ping", JobsController.Ping(), Cron.Minutely);
            }
            
        }
    }
}
