using System;
using System.ComponentModel.DataAnnotations;
using TheMoviePlace.Entities;

namespace TheMoviePlace.Models {
    public class AddPersonDTO {
        [Required]
        [MaxLength (50, ErrorMessage = "Name can be a maximum number of 200 characters only")]
        public string Name { get; set; }

        [MaxLength (2000, ErrorMessage = "Bio can be a maximum number of 2000 characters only")]
        public string Bio { get; set; }

        [DataType (DataType.Date)]
        public DateTime DateOfBirth { get; set; }
        public int GenderReferenceID { get; set; }
        public int MovieID { get; set; }
        public int RoleReferenceID { get; set; }
    }
}