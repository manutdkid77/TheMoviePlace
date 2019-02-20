using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheMoviePlace.Entities
{
    public class RoleReference
    {   
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RoleReferenceID { get; set; }

        [Required]
        [MaxLength(50)]
        public string Type { get; set; }
    }
}