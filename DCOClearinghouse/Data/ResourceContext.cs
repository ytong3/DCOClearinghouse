using DCOClearinghouse.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace DCOClearinghouse.Data
{
    public class ResourceContext : DbContext
    {
        public ResourceContext(DbContextOptions<ResourceContext> options): base(options) { }
        public DbSet<Resource> Resources { get; set; }
        public DbSet<ResourceCategory> ResourceCategories { get; set; }
        public DbSet<ResourceType> ResourceTypes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Resource>().ToTable("Resource")
                .Property(r => r.Status).HasDefaultValue(ResourceStatus.New);
            modelBuilder.Entity<Resource>()
                .Property(r => r.Contact)
                .HasConversion(
                    c => JsonConvert.SerializeObject(c),
                    c => JsonConvert.DeserializeObject<ContactInfo>(c));

            modelBuilder.Entity<ResourceCategory>().ToTable("ResourceCategory");
            

            modelBuilder.Entity<ResourceType>().ToTable("ResourceType");
        }
    }
}