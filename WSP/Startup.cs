using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WSP.Startup))]
namespace WSP
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
