//using System.Linq;
//using System.Net.Http;
//using System.Web.Http;

//namespace psy_trance_com.Controllers
//{
//    public class FoldersController : ApiController
//    {
//        [HttpGet]
//        public bool Index()
//        {
//            var folderNames = System.IO.Directory.GetDirectories(@"C:\mp3", "*", System.IO.SearchOption.AllDirectories).ToList();

//            folderNames.ForEach(folderName =>
//            {
//                if (System.IO.Directory.GetFiles(folderName, "*.mp3", System.IO.SearchOption.AllDirectories).Length > 0)
//                    return true;
//            });
//            return false;
//        }
//    }
//}