using System.Net.Http;
using System.Web.Http;

namespace psy_trance_com.Controllers
{
    public class DefaultController : ApiController
    {
        [HttpGet]
        public HttpResponseMessage Index()
        {
            var httpResponseMessage = Request.CreateResponse();

            return httpResponseMessage;
        }
    }
}