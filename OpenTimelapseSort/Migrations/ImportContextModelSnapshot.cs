﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using OpenTimelapseSort.Contexts;

namespace OpenTimelapseSort.Migrations
{
    [DbContext(typeof(ImportContext))]
    partial class ImportContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "5.0.0");

            modelBuilder.Entity("SDirectory", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<string>("ImportId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Target")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("ImportId");

                    b.ToTable("ImageDirectory");
                });

            modelBuilder.Entity("SImage", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<long>("Colors")
                        .HasColumnType("INTEGER");

                    b.Property<double>("Lumen")
                        .HasColumnType("REAL");

                    b.Property<string>("DirectoryId")
                        .HasColumnType("TEXT");

                    b.Property<long>("FileSize")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("FileTime")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("ParentInstance")
                        .HasColumnType("TEXT");

                    b.Property<string>("Target")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("DirectoryId");

                    b.ToTable("Image");
                });

            modelBuilder.Entity("SImport", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<string>("importDate")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("length")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Target")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Import");
                });

            modelBuilder.Entity("SDirectory", b =>
                {
                    b.HasOne("SImport", "ParentImport")
                        .WithMany("directories")
                        .HasForeignKey("ImportId")
                        .HasConstraintName("FK_Import_Identifier");

                    b.Navigation("ParentImport");
                });

            modelBuilder.Entity("SImage", b =>
                {
                    b.HasOne("SDirectory", "ParentDirectory")
                        .WithMany("ImageList")
                        .HasForeignKey("DirectoryId")
                        .HasConstraintName("FK_Directory_Identifier");

                    b.Navigation("ParentDirectory");
                });

            modelBuilder.Entity("SDirectory", b =>
                {
                    b.Navigation("ImageList");
                });

            modelBuilder.Entity("SImport", b =>
                {
                    b.Navigation("directories");
                });
#pragma warning restore 612, 618
        }
    }
}
