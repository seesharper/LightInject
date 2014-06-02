using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Mvc5.Startup))]
namespace Mvc5
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
