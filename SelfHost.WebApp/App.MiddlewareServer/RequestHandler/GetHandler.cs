using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.Owin;
using App.MiddlewareServer.Library;

namespace App.MiddlewareServer.RequestHandler
{
    internal class GetHandler : RequestHandlerBase
    {
        public GetHandler(IOwinContext context, IEnumerable<Route> routes, string template = null)
            : base(context, routes, template)
        { }

        public GetHandler(IOwinContext context)
            : base(context)
        { }

        public async override Task HandleMvc()
        {
            var controllerAndAction = GetControllerAndAction();

            var controller = controllerAndAction[0];
            var action = controllerAndAction[1];
            var route = GetRoute(controller);
            var parameters = Helper.HandleRequestQueryString(Context);

            if (route != null)
                await RenderResult(route, controller, action, parameters);
        }

        public async override Task HandleStaticFiles()
        {
            if (IsStaticFile())
                await RenderStaticFiles();
        }

        private async Task RenderStaticFiles()
        {
            await WriteResponse();
        }

        private Task RenderResult(Route route, string controller, string action, object[] parameters = null)
        {
            var obj = InvokeController(route.Controller, action, parameters);

            if (IsViewResult(obj))
            {
                var view = (ViewResult)obj;
                var viewPath = GetViewPath(controller, view.ViewName);
                return WriteResponse(viewPath, view.Model);
            }
            else if(IsFilePathResult(obj))
            {
                var file = (FilePathResult)obj;
                return WriteResponse(file);
            } else
            {
                var data = (ContentResult)obj;
                return WriteResponse(data.Content);
            }
        }
    }
}
