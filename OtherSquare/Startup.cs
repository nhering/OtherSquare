using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(OtherSquare.Startup))]
namespace OtherSquare
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
