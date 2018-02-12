using System;
using Microsoft.Owin;
using App.MiddlewareServer.RequestHandler;

namespace App.MiddlewareServer
{
    public class StaticFilesMiddleware
    {
        private readonly IOwinContext Context;

        public StaticFilesMiddleware(IOwinContext context)
        {
            Context = context;
        }

        public async void Invoke()
        {
            try
            {
                var handler = RequestFactory.Get(Context);
                await handler.HandleStaticFiles();
            }
            catch (Exception)
            {
                Context.Response.ContentType = "text/html";
                await Context.Response.WriteAsync("An error occurred on the server.");
            }
        }

    }
}
