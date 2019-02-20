using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheMoviePlace.Entities
{
    public class Role
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RoleID { get; set; }

        [ForeignKey("MovieID")]
        public Movie Movie {get; set;}
        public int MovieID { get; set; }

        [ForeignKey("PersonID")]
        public Person Person { get; set; }
        public int PersonID { get; set; }

        [ForeignKey("RoleReferenceID")]
        public RoleReference RoleReference { get; set; }

        public int RoleReferenceID { get; set; }
    }
}