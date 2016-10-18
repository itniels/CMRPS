using CMPRS.Web.App_Start;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CMRPS.Web.Startup))]
namespace CMRPS.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            Site.SettingsLoad();
            ConfigureAuth(app);
        }
    }
}
