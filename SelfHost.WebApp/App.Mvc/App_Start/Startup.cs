using Owin;

namespace App.Mvc
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseMvc();
            app.UseStaticFiles();
        }
    }
}