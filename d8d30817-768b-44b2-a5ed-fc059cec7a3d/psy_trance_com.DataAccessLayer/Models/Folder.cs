using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace psy_trance_com.DataAccessLayer.Models
{
    public class Folder : IEquatable<Folder>
    {
        [Key]
        public int Id { get; set; }
        [Index(IsUnique = true), StringLength(450)]
        public string Name { get; set; }

        public int BitRate { get; set; }
        public int SampleRate { get; set; }
        public int Channels { get; set; }
        public int BitsPerSample { get; set; }

        public long Size { get; set; }

        public virtual Album Album { get; set; }

        public virtual Torrent Torrent { get; set; }

        public bool Equals(Folder folder)
        {
            return Name.Equals(folder.Name);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}