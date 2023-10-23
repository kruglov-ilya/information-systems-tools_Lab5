using Lab5.Models;
using Microsoft.EntityFrameworkCore;

namespace Lab5
{
    public sealed class ApplicationContext: DbContext
    {
        public DbSet<District> Districts { get; set; } = null!;
        public DbSet<Material> Materials { get; set; } = null!;
        public DbSet<RealEstate> RealEstates { get; set; } = null!;
        public DbSet<Criteria> Criteria { get; set; } = null!;
        public DbSet<Evaluation> Evaluations { get; set; } = null!;
        public DbSet<Realtor> Realtors { get; set; } = null!;
        public DbSet<Sale> Sales { get; set; } = null!;

        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
