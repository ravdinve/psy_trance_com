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
        public HttpResponseMessage Index(int genreId = 0, string artistName = null)
        {
            using (var dbContext = new DbContext())
            {
                IQueryable<Artist> artists = dbContext.Artists
                    .OrderBy(artist => artist.Name);

                if (artistName != null)
                {
                    artists = artists.Where(x => x.Name.Contains(artistName));
                }

                if (genreId != 0)
                {
                    artists = artists.Where(x => x.Genres.Select(y => y.Id).Contains(genreId));
                }

                return Request.CreateResponse(artists.Select(artist => new Models.Artist
                {
                    Id = artist.Id,
                    Name = artist.Name,

                    Albums = artist.Albums.Count
                }).ToList());
            }
        } 
    }
}