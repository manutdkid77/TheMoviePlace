using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheMoviePlace.Entities
{
    public class GenderReference
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int GenderReferenceID { get; set; }

        [Required]
        [MaxLength(10)]
        public string Type { get; set; }
    }
}