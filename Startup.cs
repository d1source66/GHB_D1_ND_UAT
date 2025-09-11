using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(GHB_D1.Startup))]
namespace GHB_D1
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
