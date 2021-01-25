﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using OpenTimelapseSort.Contexts;

namespace OpenTimelapseSort.Migrations
{
    [DbContext(typeof(ImportContext))]
    internal partial class ImportContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "5.0.0");

            modelBuilder.Entity("OpenTimelapseSort.Models.SDirectory", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<string>("ImportId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Origin")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Target")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("ImportId");

                    b.ToTable("ImageDirectory");
                });

            modelBuilder.Entity("OpenTimelapseSort.Models.SImage", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<long>("Colors")
                        .HasColumnType("INTEGER");

                    b.Property<string>("DirectoryId")
                        .HasColumnType("TEXT");

                    b.Property<long>("FileSize")
                        .HasColumnType("INTEGER");

                    b.Property<long>("FileTime")
                        .HasColumnType("INTEGER");

                    b.Property<double>("Lumen")
                        .HasColumnType("REAL");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Origin")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("ParentInstance")
                        .HasColumnType("TEXT");

                    b.Property<string>("Target")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("DirectoryId");

                    b.ToTable("Image");
                });

            modelBuilder.Entity("OpenTimelapseSort.Models.SImport", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<string>("ImportDate")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Origin")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Import");
                });

            modelBuilder.Entity("OpenTimelapseSort.Models.SDirectory", b =>
                {
                    b.HasOne("OpenTimelapseSort.Models.SImport", "ParentImport")
                        .WithMany("Directories")
                        .HasForeignKey("ImportId")
                        .HasConstraintName("FK_Import_Identifier");

                    b.Navigation("ParentImport");
                });

            modelBuilder.Entity("OpenTimelapseSort.Models.SImage", b =>
                {
                    b.HasOne("OpenTimelapseSort.Models.SDirectory", "ParentDirectory")
                        .WithMany("ImageList")
                        .HasForeignKey("DirectoryId")
                        .HasConstraintName("FK_Directory_Identifier");

                    b.Navigation("ParentDirectory");
                });

            modelBuilder.Entity("OpenTimelapseSort.Models.SDirectory", b =>
                {
                    b.Navigation("ImageList");
                });

            modelBuilder.Entity("OpenTimelapseSort.Models.SImport", b =>
                {
                    b.Navigation("Directories");
                });
#pragma warning restore 612, 618
        }
    }
}
