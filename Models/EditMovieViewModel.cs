using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using TheMoviePlace.Entities;
using TheMoviePlace.Helpers;

namespace TheMoviePlace.Models
{
    public class EditMovieViewModel
    {
        [Required]
        [MaxLength(200,ErrorMessage="Name can be a maximum number of 200 characters only")]
        public string Name { get; set; }

        [Display(Name = "Year Of Release")]
        [DataType(DataType.Date)]
        public DateTime YearOfRelease { get; set; }

        [MaxLength(255,ErrorMessage="The Plot can be a maximum no of 255 characters only")]
        public string Plot { get; set; }
        public IFormFile Poster { get; set; }
        public string PosterUrl {get; set;}
        public List<Role> Roles {get; set;}
        public Person NewPerson { get; set; }
    }
}