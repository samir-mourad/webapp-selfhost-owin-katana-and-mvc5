using System.Collections.Generic;
using System.Threading.Tasks;
using Seal.Server.Library;
using Seal.Server.RequestHandler;

namespace Seal.Server
{
    public class Middleware
    {
        private IEnumerable<Route> Routes { get; set; }

        public Middleware(IEnumerable<Route> routes)
        {
            this.Routes = routes;
        }

        public async Task<object> Invoke(IDictionary<string, object> env)
        {
            var handler = RequestFactory.Get(env, this.Routes);
            await handler.Handle();

            return Task.FromResult<object>(null);
        }
    }
}
