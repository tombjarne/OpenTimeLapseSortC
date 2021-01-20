using Microsoft.EntityFrameworkCore;
using OpenTimelapseSort.Models;

namespace OpenTimelapseSort.Contexts
{
    internal class PreferencesContext : DbContext
    {
        public DbSet<Preferences> Preferences { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(@"Data Source=OpenTimelapseSortPreferences.db;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Preferences>().ToTable("Preferences");
        }
    }
}
