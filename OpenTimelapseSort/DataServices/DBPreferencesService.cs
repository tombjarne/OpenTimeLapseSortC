using OpentimelapseSort.Models;
using OpenTimelapseSort.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenTimelapseSort.DataServices
{
    class DBPreferencesService
    {
        public DBPreferencesService()
        {

        }

        private Preferences ReturnPreferences()
        {
            //create Preferences objects
            Preferences preferences = new Preferences();

            try
            {
                using (var context = new PreferencesContext())
                {
                    //TODO: remove foreach
                    //TODO: rework get setting of values - attributes should not be public
                    foreach (Preferences preference in context.Preferences)
                    {
                        preferences = new Preferences(
                            preference.useAutoDetectInterval,
                            preference.useCopy,
                            preference.sequenceInterval,
                            preference.sequenceImageCount
                        );
                    }
                }
            } catch (Exception e)
            {
                // fill with dummy data when database was deleted or something unexpected happened

                //TODO maybe move this whole section to SeedDatabase()
                /*
                preferences.useAutoDetectInterval = true;
                preferences.useCopy = true;
                preferences.sequenceImageCount = 10;
                preferences.sequenceInterval = 5;

                if (!SavePreferencesToDataBase(preferences))
                {
                    Console.WriteLine(e.StackTrace);
                    // let user know something weird happened
                }
                */
            }
            return preferences;
        }

        public bool SavePreferencesToDataBase(Preferences preferences)
        {
            // save preferences to DB
            bool saveSucceeded = false;
            try
            {
                using (var context = new PreferencesContext())
                {
                    var entity = context.Preferences.FirstOrDefault(item => item.id == 1);
                    
                    if (true) // TODO: update statement
                    {
                        // TODO: update values
                        context.Update(preferences);
                    } 
                    else
                    {
                        context.Add(preferences);
                    }
                    context.SaveChanges();
                }

                saveSucceeded = true;

            } catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }

            return saveSucceeded;
        }

        public bool SeedPreferencesDatabase()
        {
            bool seedSucceeded = false;
            try
            {
                using (var context = new PreferencesContext())
                {
                    Preferences preferences = new Preferences(true, true, 10, 20);
                    context.Add(preferences);
                    context.SaveChanges();
                }

                seedSucceeded = true;

            } catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }

            return seedSucceeded;
        }

        public Preferences FetchPreferences()
        {
            Preferences pre = new Preferences();

            using (var context = new PreferencesContext())
            {

                foreach (Preferences preferences in context.Preferences)
                {
                    pre = new Preferences(
                        preferences.useAutoDetectInterval,
                        preferences.useCopy,
                        preferences.sequenceInterval,
                        preferences.sequenceImageCount
                    );
                }
            }
            return pre;
        }
    }
}
