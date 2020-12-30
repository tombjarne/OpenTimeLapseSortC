using OpentimelapseSort.Models;
using OpenTimelapseSort.Contexts;
using System;
using System.Linq;

namespace OpenTimelapseSort.DataServices
{
    class DBPreferencesService
    {
        public DBPreferencesService() {}

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
                    Preferences preferences = new Preferences(true, true, true, 10, 20);
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
                try
                {
                    foreach (Preferences preferences in context.Preferences)
                    {
                        pre = new Preferences(
                            preferences.useAutoDetectInterval,
                            preferences.useCopy,
                            preferences.useAutoNaming,
                            preferences.sequenceInterval,
                            preferences.sequenceImageCount
                        );
                    }
                }
                catch (Exception e)
                {
                    // notify on exception
                }
            }
            return pre;
        }
    }
}
