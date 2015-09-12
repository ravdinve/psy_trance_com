using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace psy_trance_com.DataAccessLayer.Models
{
    public class Torrent : IEquatable<Torrent>
    {
        [Key]
        public int Id { get; set; }
        [Index(IsUnique = true), StringLength(450)]
        public string Name { get; set; }

        public byte[] Data { get; set; }

        public virtual Folder Folder { get; set; }

        public bool Equals(Torrent torrent)
        {
            return Name.Equals(torrent.Name);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}