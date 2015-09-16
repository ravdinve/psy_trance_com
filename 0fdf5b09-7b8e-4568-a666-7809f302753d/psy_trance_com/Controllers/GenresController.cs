using System.Linq;
using System.Net.Http;
using System.Web.Http;

using psy_trance_com.DataAccessLayer;
using psy_trance_com.DataAccessLayer.Models;

namespace psy_trance_com.Controllers
{
    public class GenresController : ApiController
    {
        [HttpGet]
        public HttpResponseMessage Index(string genreName = null)
        {
            using (var dbContext = new DbContext())
            {
                IQueryable<Genre> genres = dbContext.Genres.OrderBy(genre => genre.Name);

                if (genreName != null)
                {
                    genres = genres.Where(x => x.Name.Contains(genreName));
                }

                return Request.CreateResponse(genres.Select(genre => new Models.Genre
                {
                    Id = genre.Id,
                    Name = genre.Name
                }).ToList());
            }
        } 
    }
}