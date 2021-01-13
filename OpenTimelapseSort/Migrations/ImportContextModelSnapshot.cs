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
                    b.Property<string>("id")
                        .HasColumnType("TEXT");

                    b.Property<string>("importId")
                        .HasColumnType("TEXT");

                    b.Property<string>("name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("target")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("timestamp")
                        .HasColumnType("TEXT");

                    b.HasKey("id");

                    b.HasIndex("importId");

                    b.ToTable("ImageDirectory");
                });

            modelBuilder.Entity("SImage", b =>
                {
                    b.Property<string>("id")
                        .HasColumnType("TEXT");

                    b.Property<long>("Colors")
                        .HasColumnType("INTEGER");

                    b.Property<double>("Lumen")
                        .HasColumnType("REAL");

                    b.Property<string>("directoryId")
                        .HasColumnType("TEXT");

                    b.Property<long>("fileSize")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("fileTime")
                        .HasColumnType("TEXT");

                    b.Property<string>("name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("parentInstance")
                        .HasColumnType("TEXT");

                    b.Property<string>("target")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("id");

                    b.HasIndex("directoryId");

                    b.ToTable("Image");
                });

            modelBuilder.Entity("SImport", b =>
                {
                    b.Property<string>("id")
                        .HasColumnType("TEXT");

                    b.Property<string>("importDate")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("length")
                        .HasColumnType("INTEGER");

                    b.Property<string>("name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("target")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("timestamp")
                        .HasColumnType("TEXT");

                    b.HasKey("id");

                    b.ToTable("Import");
                });

            modelBuilder.Entity("SDirectory", b =>
                {
                    b.HasOne("SImport", "parentImport")
                        .WithMany("directories")
                        .HasForeignKey("importId")
                        .HasConstraintName("FK_Import_Identifier");

                    b.Navigation("parentImport");
                });

            modelBuilder.Entity("SImage", b =>
                {
                    b.HasOne("SDirectory", "parentDirectory")
                        .WithMany("imageList")
                        .HasForeignKey("directoryId")
                        .HasConstraintName("FK_Directory_Identifier");

                    b.Navigation("parentDirectory");
                });

            modelBuilder.Entity("SDirectory", b =>
                {
                    b.Navigation("imageList");
                });

            modelBuilder.Entity("SImport", b =>
                {
                    b.Navigation("directories");
                });
#pragma warning restore 612, 618
        }
    }
}
