using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Almacen.Web.Startup))]
namespace Almacen.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
