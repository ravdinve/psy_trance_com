using System.Data.Entity;

using psy_trance_com.DataAccessLayer.Models;

namespace psy_trance_com.DataAccessLayer
{
    public class DbContext : System.Data.Entity.DbContext
    {
        public DbContext()
            : base("u425815")
        {
        }

        public DbSet<Album> Albums { get; set; }
        public DbSet<AlbumArtist> AlbumArtists { get; set; }
        public DbSet<Artist> Artists { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Label> Labels { get; set; }
        public DbSet<Song> Songs { get; set; }

        public DbSet<Folder> Folders { get; set; }
        public DbSet<File> Files { get; set; }

        public DbSet<Jpeg> Jpeg { get; set; }
        public DbSet<Torrent> Torrent { get; set; }
    }
}