using System;
using System.Linq;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using OpenTimelapseSort.Contexts;
using OpenTimelapseSort.Models;

namespace OpenTimelapseSort.DataServices
{
    internal class DbPreferencesService
    {
        /// <summary>
        ///     SavePreferences()
        ///     saves a provided <see cref="preferences" /> to the database
        ///     if saving fails it ensures the database is created or resets the database
        /// </summary>
        /// <param name="preferences"></param>
        public void SavePreferences(Preferences preferences)
        {
            try
            {
                using var database = new PreferencesContext();
                var entity = database.Preferences.FirstOrDefault(p => p.Id == 1);

                if (entity != null)
                    database.Entry(entity).CurrentValues.SetValues(preferences);
                else
                    database.Add(preferences);

                database.SaveChanges();
            }
            catch (Exception)
            {
                CreateAndMigrate();
                SavePreferences(preferences);
            }
        }

        /// <summary>
        ///     CreateAndMigrate()
        ///     runs the corresponding migrations to reset the database
        /// </summary>
        private static async void CreateAndMigrate()
        {
            await using var database = new PreferencesContext();
            await database.Database.MigrateAsync();
            SeedDatabase();
        }

        /// <summary>
        ///     SeedDatabase()
        ///     sets default values as preferences after deletion or migration
        /// </summary>
        private static void SeedDatabase()
        {
            using var database = new PreferencesContext();

            var preferences = new Preferences(true, 50, 10, 20);
            database.Add(preferences);
            database.SaveChanges();
        }

        /// <summary>
        ///     DeletePreferences()
        ///     deletes passed <see cref="preferences" /> from the database
        /// </summary>
        /// <param name="preferences"></param>
        public void DeletePreferences(Preferences preferences)
        {
            using var database = new PreferencesContext();
            database.Preferences.Remove(preferences);
            database.SaveChanges();
        }

        /// <summary>
        ///     FetchPreferences()
        ///     returns all preferences from the database
        /// </summary>
        /// <returns></returns>
        public Preferences FetchPreferences()
        {
            using var database = new PreferencesContext();

            try
            {
                var preferences = database.Preferences
                    .Single(predicate => predicate.Id == 1);
                return preferences;
            }
            catch (SqliteException)
            {
                // create and migrate if the database was removed / is empty
                CreateAndMigrate();
                return FetchPreferences();
            }
        }
    }
}