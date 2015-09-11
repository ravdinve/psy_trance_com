using System.Net.Http;
using System.Web.Http;

namespace psy_trance_com.Controllers
{
    public class FoldersController : ApiController
    {
        [HttpGet]
        public HttpResponseMessage Index(string folderName = null)
        {
            var httpResponseMessage = Request.CreateResponse();

            return httpResponseMessage;
        }
    }
}