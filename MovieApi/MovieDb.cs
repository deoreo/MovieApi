using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MovieApi
{
    public class MovieDb : IdentityDbContext<IdentityUser>
    {
        public MovieDb(DbContextOptions<MovieDb> options) : base(options) 
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Movie>().HasQueryFilter(m => m.IsDeleted == false);
        }

        public DbSet<Movie> Movies { get; set; }
    }
}
