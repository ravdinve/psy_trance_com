using System;
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

        public bool Equals(Folder folder)
        {
            return Name.Equals(folder.Name);
        }
    }
}