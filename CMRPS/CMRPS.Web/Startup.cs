using CMPRS.Web.App_Start;
using Hangfire;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CMRPS.Web.Startup))]
namespace CMRPS.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            GlobalConfiguration.Configuration.UseSqlServerStorage("DefaultConnection");

            Site.SettingsLoad();
            ConfigureAuth(app);
            app.MapSignalR();
            app.UseHangfireDashboard();
            app.UseHangfireServer();
        }
    }
}
