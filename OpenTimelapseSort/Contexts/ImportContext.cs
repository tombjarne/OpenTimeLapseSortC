using Microsoft.EntityFrameworkCore;

namespace OpenTimelapseSort.Contexts
{
    public class ImportContext : DbContext
    {
        public DbSet<SImport> Imports { get; set; }
        public DbSet<SDirectory> ImageDirectories { get; set; }
        public DbSet<SImage> Images { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(@"Data Source=OpenTimelapseSort.db;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
            modelBuilder.Entity<SImport>().ToTable("Import");
            modelBuilder.Entity<SDirectory>().ToTable("ImageDirectory");
            modelBuilder.Entity<SImage>().ToTable("Image");

            
            modelBuilder.Entity<SImport>(entity =>
            {
                entity.Property(e => e.id)
                    .IsRequired();

                entity.Property(e => e.target)
                    .IsRequired();

                entity.Property(e => e.name)
                    .IsRequired();

                entity.Property(e => e.timestamp)
                    .IsRequired();

                entity.Property(e => e.importDate)
                    .IsRequired();

                entity.HasMany(directory => directory.directories)
                    .WithOne(import => import.parentImport);
            });

            modelBuilder.Entity<SImport>()
                .HasKey(i => i.id)
                .HasName("PK_Import_Identifier");

            modelBuilder.Entity<SDirectory>(entity =>
            {
                entity.Property(e => e.target)
                    .IsRequired();

                entity.Property(e => e.name)
                    .IsRequired();

                entity.HasMany(directory => directory.imageList)
                    .WithOne(image => image.parentDirectory);

                entity.HasOne(directory => directory.parentImport)
                    .WithMany(import => import.directories)
                    .HasForeignKey(directory => directory.importId)
                    .HasConstraintName("FK_Import_Identifier");
            });

            modelBuilder.Entity<SDirectory>()
                .HasKey(d => d.id)
                .HasName("PK_Directory_Identifier");

            modelBuilder.Entity<SImage>(entity =>
            {
                entity.Property(e => e.id)
                    .IsRequired();

                entity.Property(e => e.target)
                    .IsRequired();

                entity.Property(e => e.name)
                    .IsRequired();

                entity.HasOne(image => image.parentDirectory)
                    .WithMany(directory => directory.imageList)
                    .HasForeignKey(image => image.directoryId)
                    .HasConstraintName("FK_Directory_Identifier");

            });

            modelBuilder.Entity<SImage>()
                .HasKey(i => i.id)
                .HasName("PK_Image_Identifier");
        }
    }
}
