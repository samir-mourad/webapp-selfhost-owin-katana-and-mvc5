using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.Owin;
using App.MiddlewareServer.Library;

namespace App.MiddlewareServer.RequestHandler
{
    internal class PostHandler : RequestHandlerBase
    {
        public PostHandler(IOwinContext context, IEnumerable<Route> routes, string template = null)
            : base(context, routes, template) { }

        public async override Task HandleMvc()
        {
            var controllerAndAction = base.GetControllerAndAction();

            var controller = controllerAndAction[0];
            var action = controllerAndAction[1];
            var route = GetRoute(controller);
            var parameters = Helper.HandleRequestBody(Context);
            var obj = InvokeController(route.Controller, action, parameters);

            await RenderResult(obj);
        }

        private async Task RenderResult(ActionResult obj)
        {
            if (obj != null)
            {
                if (IsFilePathResult(obj))
                    await base.WriteResponse((FilePathResult)obj);
                else
                {
                    var json = (JsonResult)obj;
                    await base.WriteResponse(json);
                }
            }
            else
                await Context.Response.WriteAsync(String.Empty);
        }

        public override Task HandleStaticFiles()
        {
            throw new NotImplementedException();
        }

        private new ActionResult InvokeController(Type controller, string actionName, object[] parameters)
        {
            var controllerInstance = Activator.CreateInstance(controller, false);
            var actionMethod = controller.GetMethod(actionName);
            var castedParameters = parameters.Cast(actionMethod.GetParameters());

            if (actionMethod == null)
                throw new Exception("Action not found: " + actionName);

            return (ActionResult)actionMethod.Invoke(controllerInstance, castedParameters);
        }
    }
}
