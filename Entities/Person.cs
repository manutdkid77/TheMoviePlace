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
        [MaxLength(50,ErrorMessage="Name can be a maximum number of 200 characters only")]
        public string Name { get; set; }

        [MaxLength(2000,ErrorMessage="Bio can be a maximum number of 2000 characters only")]
        public string Bio { get; set; }

        [DataType(DataType.Date)]
        public DateTime DateOfBirth {get; set;}

        [ForeignKey("GenderReferenceID")]
        public GenderReference GenderReference { get; set; }
        [Display(Name="Gender")]
        public int GenderReferenceID { get; set; }
    }
}