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
