using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(DashBoard.Startup))]
namespace DashBoard
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
