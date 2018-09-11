using DCOClearinghouse.Models;
using Microsoft.EntityFrameworkCore;

namespace DCOClearinghouse.Data
{
    public class ResourceContext : DbContext
    {
        public ResourceContext(DbContextOptions<ResourceContext> options): base(options) { }

        public DbSet<Resource> Resources { get; set; }
        public DbSet<ResourceCategory> ResourceCategories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Resource>().ToTable("Resource")
                .Property(r => r.Status).HasDefaultValue(ResourceStatus.New);
            modelBuilder.Entity<ResourceCategory>().ToTable("ResourceCategory");
        }
    }
}