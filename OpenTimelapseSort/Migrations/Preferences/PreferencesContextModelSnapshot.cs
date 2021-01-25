﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using OpenTimelapseSort.Contexts;

namespace OpenTimelapseSort.Migrations.Preferences
{
    [DbContext(typeof(PreferencesContext))]
    internal partial class PreferencesContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "5.0.0");

            modelBuilder.Entity("OpenTimelapseSort.Models.Preferences", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("SequenceImageCount")
                        .HasColumnType("INTEGER");

                    b.Property<double>("SequenceInterval")
                        .HasColumnType("REAL");

                    b.Property<int>("SequenceIntervalGenerosity")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("UseAutoDetectInterval")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("UseCopy")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("Preferences");
                });
#pragma warning restore 612, 618
        }
    }
}
