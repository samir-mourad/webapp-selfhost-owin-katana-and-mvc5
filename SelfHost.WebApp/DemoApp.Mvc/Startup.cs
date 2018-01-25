using Owin;
using DemoApp.Mvc.Server;
using System;
using DemoApp.Mvc.Server.Library;
using System.Collections.Generic;
using DemoApp.Mvc.Controllers;

namespace DemoApp.Mvc
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.Use(new Func<object, object>(
                x => new Middleware(new List<Route>
                {
                    new Route("Home", typeof(SamirController))
                })
            ));
        }
    }
}