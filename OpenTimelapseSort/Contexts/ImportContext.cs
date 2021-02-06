﻿using System;
using System.Diagnostics;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using OpenTimelapseSort.Models;

namespace OpenTimelapseSort.Contexts
{
    public class ImportContext : DbContext
    {
        public DbSet<SImport> Imports { get; set; }
        public DbSet<SDirectory> ImageDirectories { get; set; }
        public DbSet<SImage> Images { get; set; }

        public ImportContext()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(@"Data Source=OpenTimelapseSort.db;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            try
            {
                modelBuilder.Entity<SImport>().ToTable("Import");
                modelBuilder.Entity<SDirectory>().ToTable("ImageDirectory");
                modelBuilder.Entity<SImage>().ToTable("Image");


                modelBuilder.Entity<SImport>(entity =>
                {
                    entity.Property(e => e.Id)
                        .IsRequired();

                    entity.Property(e => e.Origin)
                        .IsRequired();

                    entity.Property(e => e.Name)
                        .IsRequired();

                    entity.Property(e => e.Timestamp)
                        .IsRequired();

                    entity.Property(e => e.ImportDate)
                        .IsRequired();

                    entity.HasMany(directory => directory.Directories)
                        .WithOne(import => import.ParentImport);
                });

                modelBuilder.Entity<SDirectory>(entity =>
                {
                    entity.Property(e => e.Origin)
                        .IsRequired();

                    entity.Property(e => e.Name)
                        .IsRequired();

                    entity.HasMany(directory => directory.ImageList)
                        .WithOne(image => image.ParentDirectory);

                    entity.HasOne(directory => directory.ParentImport)
                        .WithMany(import => import.Directories)
                        .HasForeignKey(directory => directory.ImportId)
                        .HasConstraintName("FK_Import_Identifier");
                });

                modelBuilder.Entity<SImage>(entity =>
                {
                    entity.Property(e => e.Id)
                        .IsRequired();

                    entity.Property(e => e.Origin)
                        .IsRequired();

                    entity.Property(e => e.Name)
                        .IsRequired();

                    entity.HasOne(image => image.ParentDirectory)
                        .WithMany(directory => directory.ImageList)
                        .HasForeignKey(image => image.DirectoryId)
                        .HasConstraintName("FK_Directory_Identifier");
                });
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.InnerException);
                throw;
            }
        }
    }
}
