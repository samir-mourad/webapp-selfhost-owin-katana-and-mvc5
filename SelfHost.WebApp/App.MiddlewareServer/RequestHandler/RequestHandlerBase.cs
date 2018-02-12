using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.Owin;
using Newtonsoft.Json;
using App.MiddlewareServer.Library;
using RazorEngine;
using RazorEngine.Templating;

namespace App.MiddlewareServer.RequestHandler
{
    internal abstract class RequestHandlerBase
    {
        
        private Dictionary<string, string> ContentAllowed
        {
            get {
                return Helper.StaticFiles;
            }
        }
        private string[] ImagesAndFontsAllowed
        {
            get
            {
                return Helper.ImagesAndFonts;
            }
        }
        protected IOwinContext Context { get; private set; }
        protected IEnumerable<Route> Routes { get; private set; }
        protected string RequestPath {
            get {
                var index = Context.Request.Path.Value.IndexOf('/');

                return Context.Request.Path.Value.Substring(index + 1);
            }
            private set { }
        }
        protected string RequestExtensionFile {
            get {
                var content = RequestPath;

                if (content.Contains('?'))
                    content = content.Substring(0, content.IndexOf('?'));

                content = RequestPath.Substring(RequestPath.LastIndexOf('.') + 1);

                return content;
            } private set { } }

        private string Layout { get; set; }
        private string LayoutPath {
            get {
                return GetLayoutPath(Layout);
            }
        }


        public RequestHandlerBase(IOwinContext context, IEnumerable<Route> routes, string layout = null)
        {
            Context = context;
            Routes = routes;
            Layout = layout;
        }

        public RequestHandlerBase(IOwinContext context)
        {
            Context = context;
        }
        
        private void SetResponseContentType(string type)
        {
            Context.Response.Headers.Add("Content-Type", new[] { type });
        }

        private void SetResponseContentTypeAndLength(long contentLength)
        {
            if(ImagesAndFontsAllowed.Contains(RequestExtensionFile))
            {
                Context.Response.ContentLength = contentLength;
                SetResponseContentType(ContentAllowed[RequestExtensionFile]);
            } else
                SetResponseContentType(ContentAllowed[RequestExtensionFile]);
        }

        private string GetLayoutPath(string template)
        {
            return Path.Combine(Helper.GetApplicationRootPath(), "Views", "Shared", template);
        }

        protected string GetViewPath(string controller, string viewName)
        {
            return Path.Combine(Helper.GetApplicationRootPath(), "Views", controller, $"{viewName}.cshtml");
        }

        protected string GetContentPath()
        {
            return Path.Combine(Helper.GetApplicationRootPath(), RequestPath.Replace('/','\\'));
        }

        protected Route GetRoute(string routeName)
        {
            return Routes.FirstOrDefault(x => x.Name.ToLower() == routeName.ToLower());
        }
    
        protected string[] GetControllerAndAction()
        {
            var result = new string[2];
            var requestPath = RequestPath.Split('/');

            if (requestPath.Length > 1)
            {
                result[0] = requestPath[0];
                result[1] = requestPath[1];
            }
            else
            {
                var route = GetRoute(requestPath[0]);
                if (route == null)
                {
                    result[0] = ConfigurationManager.AppSettings["Host.App.Mvc.DefaultController"].ToString();
                    result[1] = requestPath[0];
                }
                else
                {
                    result[0] = requestPath[0];
                    result[1] = ConfigurationManager.AppSettings["Host.App.Mvc.DefaultAction"].ToString();
                }
            }
            return result;
        }

        protected bool IsStaticFile()
        {
            return ContentAllowed.ContainsKey(RequestExtensionFile);
        }

        protected bool IsFilePathResult(ActionResult obj)
        {
            var properties = obj.GetType().GetProperties();
            return properties.Any(x => x.Name == "FileName");
        }

        protected bool IsViewResult(ActionResult obj)
        {
            var properties = obj.GetType().GetProperties();
            return properties.Any(x => x.Name == "ViewName");
        }

        protected ActionResult InvokeController(Type controller, string actionName, object[] parameters = null)
        {
            var controllerInstance = Activator.CreateInstance(controller, false);
            var actionMethod = controller.GetMethod(actionName);
            var castedParameters = null as object[];

            if(parameters != null)
                castedParameters = parameters.Cast(actionMethod.GetParameters());
            
            if (actionMethod == null)
                throw new Exception($"Action not found: {actionName}");

            return (ActionResult)actionMethod.Invoke(controllerInstance, castedParameters);
        }
        

        protected async Task WriteResponse(JsonResult data)
        {
            SetResponseContentType("application/json");
            var json = JsonConvert.SerializeObject(data.Data);

            await Context.Response.WriteAsync(json);
        }

        protected async Task WriteResponse(string data)
        {
            SetResponseContentType("text/html");
            await Context.Response.WriteAsync(data);
        }

        protected async Task WriteResponse(FilePathResult data)
        {
            if (!File.Exists(data.FileName))
                throw new Exception($"File not found. Path: {data.FileName}");

            SetResponseContentType("text/html");

            var reader = new StreamReader(data.FileName).ReadToEnd();
            await Context.Response.WriteAsync(reader);
        }

        protected async Task WriteResponse(string viewPath, object model)
        {
            if (!File.Exists(viewPath))
                throw new Exception($"File not found. Path: {viewPath}");

            SetResponseContentType("text/html");
            
            var razorParse = ParseRazor(viewPath, model);

            await Context.Response.WriteAsync(razorParse);
        }

        private string ParseRazor(string viewPath, object model)
        {
            var service = Engine.Razor;

            if(!string.IsNullOrWhiteSpace(LayoutPath))
                service.AddTemplate(Layout, File.ReadAllText(LayoutPath));

            service.AddTemplate(viewPath, File.ReadAllText(viewPath));
            service.Compile(viewPath);

            return service.Run(viewPath, typeof(object), model);
        }

        protected async Task WriteResponse()
        {
            var path = GetContentPath();

            if (File.Exists(path))
            {
                var buffer = File.ReadAllBytes(path);
                var stream = new MemoryStream(buffer);

                SetResponseContentTypeAndLength(stream.Length);

                await Context.Response.WriteAsync(stream.ToArray());
            }
        }


        public abstract Task HandleMvc();
        public abstract Task HandleStaticFiles();
    }
}

