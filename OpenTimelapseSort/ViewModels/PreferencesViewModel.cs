using System;
using System.Collections.Generic;
using System.Text;

using OpenTimelapseSort.DataServices;
using OpenTimelapseSort.Contexts;
using OpentimelapseSort.Models;

namespace OpenTimelapseSort.ViewModels
{
    class PreferencesViewModel
    {

        private DBPreferencesService service = new DBPreferencesService();
        Preferences preferences; //holds global object so that preferences only ever refer to a single object

        public PreferencesViewModel()
        {
            InitialisePreferencesDB();
        }

        public Preferences PreferencesInstance()
        {
            return preferences;
        }

        public bool SavePreferences(bool useAutoDetectInterval, bool copyIsEnabled, bool useAutoNaming, double imageInterval, int imageCount)
        {

            bool success = false;

            using (var database = new PreferencesContext())
            {
                try
                {
                    database.Database.EnsureCreated();
                    preferences = new Preferences(
                        useAutoDetectInterval,
                        copyIsEnabled,
                        useAutoNaming,
                        imageInterval,
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
            using (var database = new PreferencesContext())
            {
                database.Database.EnsureCreated();

                return service.FetchPreferences();
                //InitialiseView();
            }
        }
    }
}
