using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WebAppEtronix20180824.Startup))]
namespace WebAppEtronix20180824
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
