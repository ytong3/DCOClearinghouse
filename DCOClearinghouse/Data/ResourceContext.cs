using DCOClearinghouse.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Newtonsoft.Json;

namespace DCOClearinghouse.Data
{
    public class ResourceContext : DbContext
    {
        public ResourceContext(DbContextOptions<ResourceContext> options): base(options) { }
        public DbSet<Resource> Resources { get; set; }
        public DbSet<ResourceCategory> ResourceCategories { get; set; }
        public DbSet<ResourceType> ResourceTypes { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<ResourceTag> ResourceTags {get; set;}


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Resource>().ToTable("Resource")
                .Property(r => r.Status).HasDefaultValue(ResourceStatus.New);
            modelBuilder.Entity<Resource>()
                .Property(r => r.Contact)
                .HasConversion(
                    c => JsonConvert.SerializeObject(c),
                    c => JsonConvert.DeserializeObject<ContactInfo>(c));
            modelBuilder.Entity<Resource>(builder =>
                {
                    builder.Property(r => r.CreateDate).Metadata.AfterSaveBehavior = PropertySaveBehavior.Ignore;
                });

            modelBuilder.Entity<ResourceCategory>().ToTable("ResourceCategory");

            modelBuilder.Entity<ResourceType>().ToTable("ResourceType");

            modelBuilder.Entity<ResourceTag>().ToTable("ResourceTag");

            modelBuilder.Entity<Tag>().ToTable("Tag");

            modelBuilder.Entity<ResourceTag>()
                .HasKey(rt => new {rt.ResourceID, rt.TagID});

            modelBuilder.Entity<ResourceTag>()
                .HasOne(rt => rt.Resource)
                .WithMany(r => r.ResourceTags)
                .HasForeignKey(rt => rt.ResourceID);

            modelBuilder.Entity<ResourceTag>()
                .HasOne(rt => rt.Tag)
                .WithMany(t => t.ResourceTags)
                .HasForeignKey(rt => rt.TagID);
        }
    }
}