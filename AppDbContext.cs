using Microsoft.EntityFrameworkCore;

namespace GitВГТУ
{
    public class AppDbContext : DbContext
    {
        public DbSet<users> Users {  get; set; }
        public DbSet<repos> Repos { get; set; }

        public AppDbContext()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=GitDB;Username=postgres;Password=root");
        }
    }
}
