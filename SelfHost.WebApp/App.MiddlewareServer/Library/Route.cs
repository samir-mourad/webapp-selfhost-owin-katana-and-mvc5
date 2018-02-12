using System;

namespace App.MiddlewareServer.Library
{
    public class Route
    {
        public Type Controller { get; private set; }
        public string Name { get; private set; }

        public Route(string name, Type controller)
        {
            this.Name = name;
            this.Controller = controller;
        }
    }
}
