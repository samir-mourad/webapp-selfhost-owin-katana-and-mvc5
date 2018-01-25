using Microsoft.Owin.Hosting;
using System;
using Topshelf;

namespace DemoApp.Host
{
    internal class HostingConfiguration : ServiceControl
    {
        private IDisposable _webApplication;

        public bool Start(HostControl hostControl)
        {
            Console.WriteLine("Iniciando o serviço...");

            //change the startup class between WebApi.Startup and Mvc.Startup to test both applications 
            _webApplication = WebApp.Start<Mvc.Startup>("http://localhost:1234");

            Console.WriteLine("Serviço iniciado.");
            return true;
        }

        public bool Stop(HostControl hostControl)
        {
            Console.WriteLine("Encerrando o serviço...");

            _webApplication.Dispose();

            Console.WriteLine("Serviço Encerrado.");

            return true;
        }
    }
}
