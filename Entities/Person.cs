using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheMoviePlace.Entities
{
    public class Person
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PersonID { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [MaxLength(2000)]
        public string Bio { get; set; }

        [DataType(DataType.Date)]
        public DateTime DateOfBirth {get; set;}

        [ForeignKey("GenderReferenceID")]
        public GenderReference GenderReference { get; set; }

        public int GenderReferenceID { get; set; }
    }
}