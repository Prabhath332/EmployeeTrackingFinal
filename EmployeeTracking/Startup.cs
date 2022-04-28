using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(EmployeeTracking.Startup))]
namespace EmployeeTracking
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
