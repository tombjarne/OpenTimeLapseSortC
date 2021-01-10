using System;
using System.Collections.Generic;
using System.Text;

using OpenTimelapseSort.DataServices;
using OpenTimelapseSort.Contexts;
using OpentimelapseSort.Models;
using System.Diagnostics;

namespace OpenTimelapseSort.ViewModels
{
    class PreferencesViewModel
    {
        private readonly DBPreferencesService _dbPreferencesService = new DBPreferencesService();

        public PreferencesViewModel()
        {
            InitialisePreferencesDB();
        }

        private async void EnsureDatabaseIsCreatedAsync()
        {
            await using var database = new PreferencesContext();
            await database.Database.EnsureCreatedAsync();
        }

        //TODO: refactor and optimize!

        public bool SavePreferences(bool useAutoDetectInterval, bool copyIsEnabled, double imageInterval, int generosity, int imageCount)
        {

            bool success = false;
            using var database = new PreferencesContext();

            try
            {
                EnsureDatabaseIsCreatedAsync();

                Preferences preferences = new Preferences(
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

        private void InitialisePreferencesDB()
        {
            using (var database = new PreferencesContext())
            {
                try
                {
                    EnsureDatabaseIsCreatedAsync();

                    //service.SeedPreferencesDatabase();
                    //InitialiseView();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.StackTrace);
                }
            }
        }

        public Preferences FetchFromDatabase()
        {
            return _dbPreferencesService.FetchPreferences();
        }
    }
}
