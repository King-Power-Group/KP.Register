using Microsoft.Owin;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

[assembly: OwinStartup(typeof(KP.Register.Web.API.Startup))]

namespace KP.Register.Web.API
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
        //public void ConfigureServices(IServiceCollection services)
        //{

        //}
    }
}