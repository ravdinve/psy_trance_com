using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace psy_trance_com.DataAccessLayer.Models
{
    public class Album : IEquatable<Album>
    {
        [Key]
        public int Id { get; set; }
        [Index, StringLength(450)]
        public string Name { get; set; }

        //public virtual List<Album> Albums { get; set; }
        public virtual List<Artist> Artists { get; set; }
        public virtual List<Country> Countries { get; set; }
        public virtual List<Genre> Genres { get; set; }
        public virtual List<Label> Labels { get; set; }
        public virtual List<Song> Songs { get; set; }

        [Index(IsUnique = true), StringLength(450)]
        public string Folder { get; set; }
        public virtual List<Folder> Folders { get; set; }

        //public int Year { get; set; }

        public virtual List<Jpeg> Jpeg { get; set; }
        public virtual List<Torrent> Torrent { get; set; }

        public bool Equals(Album album)
        {
            return Folder.Equals(album.Folder);
        }

        public override int GetHashCode()
        {
            return Folder.GetHashCode();
        }
    }
}