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
        private DBPreferencesService service = new DBPreferencesService();

        public PreferencesViewModel()
        {
            InitialisePreferencesDB();
        }

        //TODO: refactor and optimize!

        public bool SavePreferences(bool useAutoDetectInterval, bool copyIsEnabled, double imageInterval, int generosity, int imageCount)
        {

            bool success = false;

            using (var database = new PreferencesContext())
            {
                try
                {
                    database.Database.EnsureCreated();
                    Preferences preferences = new Preferences(
                        useAutoDetectInterval,
                        copyIsEnabled,
                        imageInterval,
                        generosity,
                        imageCount
                    );

                    success = service.SavePreferencesToDataBase(preferences);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.StackTrace);
                }
            }
            return success;
        }

        private void InitialisePreferencesDB()
        {
            using (var database = new PreferencesContext())
            {
                try
                {
                    database.Database.EnsureCreated();

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
            try
            {
                using (var database = new PreferencesContext())
                {
                    database.Database.EnsureCreated();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.StackTrace);
            }

            Debug.WriteLine(service.FetchPreferences().id);
            return service.FetchPreferences();
        }
    }
}
