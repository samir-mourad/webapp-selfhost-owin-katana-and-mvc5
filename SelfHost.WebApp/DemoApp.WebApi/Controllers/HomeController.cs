using System.Web.Http;

namespace DemoApp.WebApi.Controllers
{
    public class HomeController : ApiController
    {
        [HttpGet]
        public IHttpActionResult Get()
        {
            return Json(new { Result = "Esta é uma API com self host." });
        }
    }
}
