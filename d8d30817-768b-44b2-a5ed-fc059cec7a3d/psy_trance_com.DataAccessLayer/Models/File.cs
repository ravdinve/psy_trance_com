using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace psy_trance_com.DataAccessLayer.Models
{
    public class File : IEquatable<File>
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

        public virtual Song Song { get; set; }

        public bool Equals(File file)
        {
            return Name.Equals(file.Name);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}