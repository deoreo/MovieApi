using Microsoft.EntityFrameworkCore;

namespace MovieApi
{
    public class MovieDb : DbContext
    {
        public MovieDb(DbContextOptions<MovieDb> options) : base(options) 
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Movie>().HasQueryFilter(m => m.IsDeleted == false);
        }

        public DbSet<Movie> Movies { get; set; }
    }
}
