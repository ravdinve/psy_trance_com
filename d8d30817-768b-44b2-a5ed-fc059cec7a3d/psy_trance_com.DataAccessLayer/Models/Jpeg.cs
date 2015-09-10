using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace psy_trance_com.DataAccessLayer.Models
{
    public class Jpeg : IEquatable<Jpeg>
    {
        [Key]
        public int Id { get; set; }
        [Index(IsUnique = true), StringLength(450)]
        public string Name { get; set; }

        public byte[] Data { get; set; }

        public bool Equals(Jpeg jpeg)
        {
            return Name.Equals(jpeg.Name);
        }
    }
}