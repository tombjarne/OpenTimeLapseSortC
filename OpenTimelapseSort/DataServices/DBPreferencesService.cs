using System.Linq;
using OpenTimelapseSort.Contexts;
using OpenTimelapseSort.Models;

namespace OpenTimelapseSort.DataServices
{
    internal class DbPreferencesService
    {
        public void SavePreferencesToDataBase(Preferences preferences)
        {
            using var database = new PreferencesContext();
            var entity = database.Preferences.FirstOrDefault(p => p.Id == 1);

            if (entity != null)
                database.Entry(entity).CurrentValues.SetValues(preferences);
            else
                database.Add(preferences);

            database.SaveChanges();
        }

        public void SeedPreferencesDatabase()
        {
            using var database = new PreferencesContext();
            
            var preferences = new Preferences(true, true, 50, 10, 20);
            database.Add(preferences);
            database.SaveChanges();
        }

        public Preferences FetchPreferences()
        {
            using var database = new PreferencesContext();

            try
            {
                var preferences = database.Preferences
                    .Single(predicate => predicate.Id == 1);
                return preferences;
            } catch
            {
                SeedPreferencesDatabase();
                return FetchPreferences();
            }
        }
    }
}