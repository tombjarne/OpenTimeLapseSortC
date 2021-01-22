using OpenTimelapseSort.Contexts;
using OpenTimelapseSort.DataServices;
using OpenTimelapseSort.Models;
using System;

namespace OpenTimelapseSort.ViewModels
{
    internal class PreferencesViewModel
    {
        private readonly DbPreferencesService _dbPreferencesService = new DbPreferencesService();

        public PreferencesViewModel()
        {
            InitialisePreferencesDb();
        }

        private static async void EnsureDatabaseIsCreatedAsync()
        {
            await using var database = new PreferencesContext();
            await database.Database.EnsureCreatedAsync();
        }

        //TODO: refactor and optimize!

        public bool SavePreferences(bool useAutoDetectInterval,
            bool copyIsEnabled, double imageInterval, int generosity, int imageCount)
        {

            var success = false;
            using var database = new PreferencesContext();

            try
            {
                EnsureDatabaseIsCreatedAsync();

                var preferences = new Preferences(
                    useAutoDetectInterval,
                    copyIsEnabled,
                    imageInterval,
                    generosity,
                    imageCount
                );

                success = _dbPreferencesService.SavePreferencesToDataBase(preferences);
            }
            catch (Exception)
            {
                // handle exception
            }

            return success;
        }

        private static void InitialisePreferencesDb()
        {
            using var database = new PreferencesContext();
            try
            {
                EnsureDatabaseIsCreatedAsync();
                //service.SeedPreferencesDatabase();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
        }

        public Preferences FetchFromDatabase()
        {
            return _dbPreferencesService.FetchPreferences();
        }
    }
}
