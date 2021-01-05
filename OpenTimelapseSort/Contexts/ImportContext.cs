using Microsoft.EntityFrameworkCore;

namespace OpenTimelapseSort.Contexts
{
    public class ImportContext : DbContext
    {
        public DbSet<Import> Imports { get; set; }
        public DbSet<ImageDirectory> ImageDirectories { get; set; }
        public DbSet<Image> Images { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(@"Data Source=OpenTimelapseSort.db;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
            modelBuilder.Entity<Import>().ToTable("Import");
            modelBuilder.Entity<ImageDirectory>().ToTable("ImageDirectory");
            modelBuilder.Entity<Image>().ToTable("Image");

            /*
            modelBuilder.Entity<Import>(entity =>
            {
                entity.Property(import => import.timestamp)
                    .IsRequired();

                entity.HasMany(import => import.directories)
                    .WithMany(directories => directories);
            });

            // TODO: replace above with detailed code below to match specs
            
            modelBuilder.Entity<Department>(entity =>
            {
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });
 
            modelBuilder.Entity<Employee>(entity =>
            {
                entity.Property(e => e.Designation)
                    .IsRequired()
                    .HasMaxLength(25)
                    .IsUnicode(false);
 
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);
 
                entity.HasOne(d => d.Department)
                    .WithMany(p => p.Employee) <- this makes the relationships between db entries
                    .HasForeignKey(d => d.DepartmentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Employee_Department");
            });
            */

        }

    }
}
