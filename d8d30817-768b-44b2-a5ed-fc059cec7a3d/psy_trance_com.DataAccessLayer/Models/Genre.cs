using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace psy_trance_com.DataAccessLayer.Models
{
    public class Genre : IEquatable<Genre>
    {
        [Key]
        public int Id { get; set; }
        [Index(IsUnique = true), StringLength(450)]
        public string Name { get; set; }

        public virtual List<Album> Albums { get; set; }
        public virtual List<Artist> Artists { get; set; }
        public virtual List<Country> Countries { get; set; }
        //public virtual List<Genre> Genres { get; set; }
        public virtual List<Label> Labels { get; set; }
        public virtual List<Song> Songs { get; set; }

        public bool Equals(Genre genre)
        {
            return Name.Equals(genre.Name);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}