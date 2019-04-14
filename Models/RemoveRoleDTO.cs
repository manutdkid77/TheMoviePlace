using System;
using System.ComponentModel.DataAnnotations;

namespace TheMoviePlace.Models
{
    public class RemoveRoleDTO
    {
        [Required]
        [Range(1,Int32.MaxValue)]
        public int PersonID { get; set; }
        
        [Required]
        [Range(1,Int32.MaxValue)]
        public int MovieID { get; set; }

        [Required]
        [Range(1,Int32.MaxValue)]
        public int RoleReferenceID { get; set; }
    }
}