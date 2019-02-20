

using Microsoft.EntityFrameworkCore;

namespace TheMoviePlace.Entities
{
    public class TheMoviePlaceDBContext : DbContext
    {
        public TheMoviePlaceDBContext(DbContextOptions options) : base(options)
        {
            Database.Migrate();
        }

        public DbSet<Movie> Movies { get; set; }
        public DbSet<Person> Persons { get; set; }
        public DbSet<GenderReference> GenderReferences { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<RoleReference> RoleReferences { get; set; }
    }
}