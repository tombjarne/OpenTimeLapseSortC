using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Sqlite;

using System;
using System.Collections.Generic;
using System.Text;

using OpenTimelapseSort.Models;


namespace OpenTimelapseSort.Contexts
{
    class ImportContext : DbContext
    {
        public DbSet<Import> Import { get; set; }
        public DbSet<ImageDirectory> ImageDirectory { get; set; }
        public DbSet<Image> Image { get; set; }

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
