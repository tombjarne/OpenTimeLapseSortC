using Microsoft.EntityFrameworkCore;
using System.Collections;

using OpenTimelapseSort.Models;

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
            modelBuilder.Entity<Import>().ToTable("Imports");
            modelBuilder.Entity<ImageDirectory>().ToTable("Directories");
            modelBuilder.Entity<Image>().ToTable("Image");
        }

    }
}
