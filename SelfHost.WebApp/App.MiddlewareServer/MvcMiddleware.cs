using System;
using System.Collections.Generic;
using Microsoft.Owin;
using App.MiddlewareServer.Library;
using App.MiddlewareServer.RequestHandler;

namespace App.MiddlewareServer
{
    public class MvcMiddleware
    {
        private IEnumerable<Route> Routes { get; set; }
        private readonly IOwinContext Context;
        private string Layout { get; set; }

        public MvcMiddleware(IOwinContext context, IEnumerable<Route> routes, string layout = null)
        {
            Context = context;
            Routes = routes;
            Layout = layout;
        }

        public async void Invoke()
        {
            try
            {
                var handler = RequestFactory.Get(Context, Routes, Layout);
                await handler.HandleMvc();
            }
            catch (Exception ex)
            {
                Context.Response.ContentType = "text/html";
                await Context.Response.WriteAsync("An error occurred on the server.");
            }

        }

    }
}
