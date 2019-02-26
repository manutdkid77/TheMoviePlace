using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheMoviePlace.Entities
{
    public class Movie
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MovieID { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; }

        [DataType(DataType.Date)]
        public DateTime YearOfRelease { get; set; }

        [MaxLength(255)]
        public string Plot { get; set; }

        [MaxLength(50)]
        public string Poster { get; set; }

        public List<Role> Roles { get; set; }
    }
}