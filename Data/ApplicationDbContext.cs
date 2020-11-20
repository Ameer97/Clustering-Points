using Clustering.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Clustering.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Server=localhost;Port=5432;Database=Clustering;User Id=postgres;Password=postgres;Timeout=300;",
            o => o.UseNetTopologySuite());
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.HasPostgresExtension("postgis");

            modelBuilder.Entity<Point>()
                .Property(b => b.Geom)
                .HasComputedColumnSql("public.ST_SetSRID(ST_MakePoint(\"Lan\",\"Lat\"),4326)");
        }

        public DbSet<Point> Points { get; set; }
        public DbSet<Type> Types { get; set; }
        public DbSet<City> Cities { get; set; }
    }
}
