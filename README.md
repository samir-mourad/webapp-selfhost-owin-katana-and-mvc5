## Middleware (Owin) to handle MVC Applications
This is my own OWIN Middleware, responsible to handling http requests, invoking mvc controllers and render views with razor engine, Json and Static Files). It's also running by a self host with topshelf.

It's possible too self-hosting WebApi with a little bit of code. We will see the both of examples.

#### References:
- Owin ([Katana](https://github.com/aspnet/AspNetKatana))
- [Topshelf](http://topshelf-project.com/)
- [RazorEngine](https://github.com/Antaris/RazorEngine) 

## Self Hosting your MVC 4 or 5 Web Application
We will need to build two classes into AppStart folder in your own mvc application.
``Startup.cs`` and ``Extensions.cs`` .

#### Startup.cs
```cs
using Owin;

namespace App.Mvc
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseMvc();
            app.UseStaticFiles();
        }
    }
}
```
#### Extensions.cs 

```cs

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
```
Finally, you will need to change the `` HostingConfiguration.cs `` class. Change the ``<Startup>`` to your own class created on the previous step.
```cs
 public bool Start(HostControl hostControl) {
     Console.WriteLine("Starting service...");

     //change <TName> to your own Startup class in your MVC application
     _webApplication = WebApp.Start<Startup>(ConfigurationManager.AppSettings["Host.Url"].ToString());

     Console.WriteLine("Service was started.");
     return true;
}
```


## Self Hosting your WebApi Application.

First of all, you will need to build your Startup.cs class. 
```csharp
using Owin;
using System.Web.Http;

namespace App.WebApi
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var config = new HttpConfiguration();

            config.Routes.MapHttpRoute(
                "DefaultApi"
               , "api/{controller}/{id}"
               , new { id = RouteParameter.Optional });

            app.UseWebApi(config);
        }
    }
}

```

Second, you will need to change the `` HostingConfiguration.cs `` class. Change the ``<Startup>`` to your own class created on the previous step.
```cs
 public bool Start(HostControl hostControl) {
     Console.WriteLine("Starting service...");

     ////change <TName> to your own Startup class in your WebApi application
     _webApplication = WebApp.Start<Startup>(ConfigurationManager.AppSettings["Host.Url"].ToString());

     Console.WriteLine("Service was started.");
     return true;
}
```

This line provides your own WebApi. All you need is create any Controller that inherits the ``System.Web.Http.ApiController `` class.

## LAUNCHING APPLICATION
Start only ``App.Host`` project and that's it. Topshelf will be responsible for host your application. You don't need to have IIS installed neither IIS express to launch your application.
