using System.Linq;
using System.Net.Http;
using System.Web.Http;

using psy_trance_com.DataAccessLayer;
using psy_trance_com.DataAccessLayer.Models;

namespace psy_trance_com.Controllers
{
    public class ArtistsController : ApiController
    {
        [HttpGet]
        public HttpResponseMessage Index(string artistName = null, int? genreId = null)
        {
            using (var dbContext = new DbContext())
            {
                IQueryable<Artist> artists = dbContext.Artists.OrderBy(artist => artist.Name);

                if (artistName != null)
                {
                    artists = artists.Where(x => x.Name.Contains(artistName));
                }

                if (genreId != null)
                {
                    var genre = dbContext.Genres.FirstOrDefault(x => x.Id == genreId);

                    artists = artists.Where(x => x.Genres.Contains(genre));
                }

                return Request.CreateResponse(artists.Select(artist => new Models.Artist
                {
                    Id = artist.Id,
                    Name = artist.Name
                }).ToList());
            }
        } 
    }
}