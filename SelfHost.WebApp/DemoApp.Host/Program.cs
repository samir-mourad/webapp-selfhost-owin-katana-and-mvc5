using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;

namespace DemoApp.Host
{
    class Program
    {
        static void Main(string[] args)
        {
            HostFactory.Run(x => 
            {
                x.Service<HostingConfiguration>();
                x.RunAsLocalSystem();

                x.SetDescription("Owin com ASP.NET WEB.API/ASP.NET MVC.");
                x.SetDisplayName("selfhost.owin.webapp");
                x.SetServiceName("selfhost.owin.webapp");
            });

        }
    }
}
