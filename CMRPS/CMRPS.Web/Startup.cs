using System.Collections.Generic;
using System.Linq;
using System.Text;
using CMPRS.Web.App_Start;
using CMPRS.Web.Models;
using CMRPS.Web.Controllers;
using CMRPS.Web.Models;
using Hangfire;
using Hangfire.Common;
using Hangfire.Dashboard;
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

            // Hangfire settings
            BackgroundJobServerOptions opt = new BackgroundJobServerOptions();
            opt.WorkerCount = 20;
            opt.Queues = new[] {"critical", "default"};

            Site.SettingsLoad();
            ConfigureAuth(app);
            app.MapSignalR();
            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                AuthorizationFilters = new[] { new MyRestrictiveAuthorizationFilter() }
            });
            app.UseHangfireServer(opt);

            // Add hangfire jobs
            SettingsModel settings = db.Settings.First();
            var manager = new RecurringJobManager();
            if (settings != null)
            {
                manager.AddOrUpdate("UpdateComputers", Job.FromExpression(() => JobsController.Enqueue()), Cron.MinuteInterval(settings.PingInterval), new RecurringJobOptions {QueueName = "default"});
                manager.AddOrUpdate("Scheduler", Job.FromExpression(() => JobsController.Scheduler()), Cron.Minutely(), new RecurringJobOptions { QueueName = "critical" });
                manager.AddOrUpdate("CleanLogs", Job.FromExpression(() => JobsController.CleanLogs()), Cron.HourInterval(1), new RecurringJobOptions { QueueName = "critical" });
            }
            else
            {
                manager.AddOrUpdate("UpdateComputers", Job.FromExpression(() => JobsController.Enqueue()), Cron.Minutely(), new RecurringJobOptions { QueueName = "default" });
                manager.AddOrUpdate("Scheduler", Job.FromExpression(() => JobsController.Scheduler()), Cron.Minutely(), new RecurringJobOptions { QueueName = "critical" });
                manager.AddOrUpdate("CleanLogs", Job.FromExpression(() => JobsController.CleanLogs()), Cron.HourInterval(1), new RecurringJobOptions { QueueName = "critical" });
            }
            
        }
    }
}


public class MyRestrictiveAuthorizationFilter : IAuthorizationFilter
{
    public bool Authorize(IDictionary<string, object> owinEnvironment)
    {
        // In case you need an OWIN context, use the next line,
        // `OwinContext` class is the part of the `Microsoft.Owin` package.
        var context = new OwinContext(owinEnvironment);

        // Allow all authenticated users to see the Dashboard (potentially dangerous).
        return context.Authentication.User.Identity.IsAuthenticated;
    }
}