using System.Collections.Generic;
using System.Threading.Tasks;
using DemoApp.Mvc.Server.Library;

namespace DemoApp.Mvc.Server.RequestHandler
{
    internal class GetHandler : RequestHandlerBase
    {
        public GetHandler(IDictionary<string, object> env, IEnumerable<Route> routes) 
            : base(env, routes) 
        { }

        public async override Task<object> Handle()
        {
            var controllerAndAction =  base.GetControllerAndAction();
            var route = base.GetRoute(controllerAndAction[0]);
            var view = base.InvokeController(route.Controller, controllerAndAction[1]);
            var viewPath = base.GetViewPath(controllerAndAction[0], view.ViewName);
            await base.WriteResponse(viewPath, view.Model);

            return Task.FromResult<object>(null);
        }
    }
}
