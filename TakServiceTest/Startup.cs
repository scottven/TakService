using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TakServiceTest.Startup))]
namespace TakServiceTest
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}
