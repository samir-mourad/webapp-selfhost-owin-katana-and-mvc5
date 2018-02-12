using System.Web.Http;

namespace App.WebApi.Controllers
{
    public class HomeController : ApiController
    {
        [HttpGet]
        public IHttpActionResult Get()
        {
            return Json(new { Result = "This is an API self-hosted." });
        }
    }
}
