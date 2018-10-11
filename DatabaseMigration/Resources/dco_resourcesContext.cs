using Microsoft.EntityFrameworkCore;

namespace DatabaseMigration.Resources
{
    public partial class dco_resourcesContext : DbContext
    {
        public dco_resourcesContext()
        {
        }

        public dco_resourcesContext(DbContextOptions<dco_resourcesContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Categories> Categories { get; set; }
        public virtual DbSet<Listings> Listings { get; set; }
        public virtual DbSet<Subscribers> Subscribers { get; set; }
        public virtual DbSet<CategoriesListings> CategoryListingBindings { get; set; }

        // Unable to generate entity type for table 'categories_listings'. Please see the warning messages.
        // Unable to generate entity type for table 'schema_info'. Please see the warning messages.

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseMySql("Server=localhost;Port=3306;Database=dco_resources;Uid=root;Pwd=root;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Categories>(entity =>
            {
                entity.ToTable("categories");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Lft)
                    .HasColumnName("lft")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.ParentId)
                    .HasColumnName("parent_id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Rgt)
                    .HasColumnName("rgt")
                    .HasColumnType("int(11)");
            });

            modelBuilder.Entity<Listings>(entity =>
            {
                entity.ToTable("listings");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Address1)
                    .HasColumnName("address_1")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.Address2)
                    .HasColumnName("address_2")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.City)
                    .HasColumnName("city")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.ContactFirstName)
                    .HasColumnName("contact_first_name")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.ContactLastName)
                    .HasColumnName("contact_last_name")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.ContactTitle)
                    .HasColumnName("contact_title")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasColumnType("datetime");

                entity.Property(e => e.Department)
                    .HasColumnName("department")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.Description)
                    .HasColumnName("description")
                    .HasColumnType("text");

                entity.Property(e => e.Email)
                    .HasColumnName("email")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.Phone)
                    .HasColumnName("phone")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.SanFranciscoNeighborhood)
                    .HasColumnName("san_francisco_neighborhood")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.State)
                    .HasColumnName("state")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnName("updated_at")
                    .HasColumnType("datetime");

                entity.Property(e => e.Website)
                    .HasColumnName("website")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.Zipcode)
                    .HasColumnName("zipcode")
                    .HasColumnType("varchar(255)");
            });

            modelBuilder.Entity<Subscribers>(entity =>
            {
                entity.ToTable("subscribers");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Email)
                    .HasColumnName("email")
                    .HasColumnType("varchar(255)");
            });

            modelBuilder.Entity<CategoriesListings>(entity =>
            {
                entity.ToTable("categories_listings");

                entity.HasKey(i => new {i.CategoryId, i.ListId});

                entity.Property(i => i.CategoryId)
                    .HasColumnName("category_id")
                    .HasColumnType("int(11)");

                entity.Property(i => i.ListId)
                    .HasColumnName("listing_id")
                    .HasColumnType("int(11)");
            });
        }
    }
}
