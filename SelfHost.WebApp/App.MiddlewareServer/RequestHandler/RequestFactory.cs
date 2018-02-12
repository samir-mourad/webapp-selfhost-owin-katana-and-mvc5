using System;
using System.Collections.Generic;
using Microsoft.Owin;
using App.MiddlewareServer.Library;

namespace App.MiddlewareServer.RequestHandler
{
    internal static class RequestFactory
    {
        public static RequestHandlerBase Get(IOwinContext context, IEnumerable<Route> routes, string layout = null)
        {
            var httpMethod = context.Request.Method;

            switch (httpMethod)
            { 
                case "GET": return new GetHandler(context, routes, layout);
                case "POST": return new PostHandler(context, routes, layout);
                default: throw new NotImplementedException("No handler found for: " + httpMethod);
            }
        }

        public static RequestHandlerBase Get(IOwinContext context)
        {
            var httpMethod = context.Request.Method;

            switch (httpMethod)
            {
                case "GET": return new GetHandler(context);
                default: throw new NotImplementedException("No handler found for: " + httpMethod);
            }
        }

    }
}
