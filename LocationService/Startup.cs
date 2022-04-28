using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Owin;
using Owin;
using System.Threading;
using LocationService.App_Codes;

[assembly: OwinStartupAttribute(typeof(LocationService.Startup))]

namespace LocationService
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
