
using App.MiddlewareServer;
using App.MiddlewareServer.Library;
using App.Mvc.Controllers;
using Owin;
using System.Collections.Generic;

namespace App.Mvc
{
    public static class Extensions
    {
        public static void UseMvc(this IAppBuilder app)
        {
            var routes = new List<Route>() {
                new Route("Home", typeof(HomeController))
            };

            var layout = "_Layout.cshtml";

            app.Use((context, next) => {

                if (!Helper.IsStaticFile(context.Request.Path.Value))
                {
                    var mvc = new MvcMiddleware(context, routes, layout);
                    mvc.Invoke();
                }

                return next();
            });
        }

        public static void UseStaticFiles(this IAppBuilder app)
        {
            app.Use((context, next) => {
                if (Helper.IsStaticFile(context.Request.Path.Value))
                {
                    var staticFiles = new StaticFilesMiddleware(context);
                    staticFiles.Invoke();
                }

                return next();
            });
        }

    }
}