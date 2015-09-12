using System.Net.Http;
using System.Web.Http;

namespace psy_trance_com.Controllers
{
    [Route("announce")]
    public class AnnounceController : ApiController
    {
        [HttpGet]
        public HttpResponseMessage Index()
        {
            var httpResponseMessage = Request.CreateResponse();

            return httpResponseMessage;
        }
    }
}
