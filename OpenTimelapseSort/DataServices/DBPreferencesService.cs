using OpenTimelapseSort.Contexts;
using OpenTimelapseSort.Models;
using System;
using System.Diagnostics;
using System.Linq;

namespace OpenTimelapseSort.DataServices
{
    internal class DbPreferencesService
    {
        public bool SavePreferencesToDataBase(Preferences preferences)
        {
            bool saveSucceeded;
            using var database = new PreferencesContext();
            var entity = database.Preferences.FirstOrDefault(p => p.Id == 1);

            if (entity != null)
                database.Entry(entity).CurrentValues.SetValues(preferences);
            else
                database.Add(preferences);

            try
            {
                database.SaveChanges();
                saveSucceeded = true;
            }
            catch
            {
                saveSucceeded = false;
            }

            return saveSucceeded;
        }

        public bool SeedPreferencesDatabase()
        {
            var seedSucceeded = false;
            try
            {
                using (var database = new PreferencesContext())
                {
                    var preferences = new Preferences(true, true, 50, 10, 20);
                    database.Add(preferences);
                    database.SaveChanges();
                }

                seedSucceeded = true;

            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }

            return seedSucceeded;
        }

        // TODO: make seeding generic!

        public Preferences FetchPreferences()
        {
            using var database = new PreferencesContext();
            try
            {
                var preferences = database.Preferences
                    .Single(predicate => predicate.Id == 1);
                return preferences;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.StackTrace);
                if (SeedPreferencesDatabase())
                {
                    var preferences = database.Preferences
                        .Single(predicate => predicate.Id == 1);
                    return preferences;
                }
                return FetchPreferences();
            }
        }
    }
}
