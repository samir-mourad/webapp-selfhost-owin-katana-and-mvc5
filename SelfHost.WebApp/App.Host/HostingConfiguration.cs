using Microsoft.Owin.Hosting;
using System;
using Topshelf;
using System.Configuration;

namespace App.Host
{
    internal class HostingConfiguration : ServiceControl
    {
        private IDisposable _webApplication;

        public bool Start(HostControl hostControl)
        {
            Console.WriteLine("Starting service...");

            //swap <TName> between WebApi.Startup and Mvc.Startup.
            _webApplication = WebApp.Start<Mvc.Startup>(ConfigurationManager.AppSettings["Host.Url"].ToString());

            Console.WriteLine("Service was started.");
            return true;
        }

        public bool Stop(HostControl hostControl)
        {
            Console.WriteLine("Stopping service...");

            _webApplication.Dispose();

            Console.WriteLine("Service was stopped.");

            return true;
        }
    }
}
