using Topshelf;

namespace App.Host
{
    class Program
    {
        static void Main(string[] args)
        {
            HostFactory.Run(x => 
            {
                x.Service<HostingConfiguration>();
                x.RunAsLocalSystem();

                x.SetDescription("WebServer SelfHost");
                x.SetDisplayName("samirmourad.app.host");
                x.SetServiceName("samirmourad.app.host");

                x.EnableServiceRecovery(i => {
                    i.OnCrashOnly();
                    i.RestartService(0);
                });
            });

        }
    }
}
