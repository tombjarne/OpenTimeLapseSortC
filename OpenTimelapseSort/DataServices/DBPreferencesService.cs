﻿using OpentimelapseSort.Models;
using OpenTimelapseSort.Contexts;
using System;
using System.Collections.Generic;
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
                preferences.useAutoDetectInterval = true;
                preferences.useCopy = true;
                preferences.sequenceImageCount = 10;
                preferences.sequenceInterval = 5;

                if (!SavePreferencesToDataBase(preferences))
                {
                    Console.WriteLine(e.StackTrace);
                    // let user know something weird happened
                }
            }
            return preferences;
        }

        private bool SavePreferencesToDataBase(Preferences preferences)
        {
            // save preferences to DB
            bool saveSucceeded = false;
            try
            {
                using (var context = new ImportContext())
                {

                    context.Add(preferences);
                    context.SaveChanges();
                }

                saveSucceeded = true;

            } catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }

            return saveSucceeded;
        }

        private bool SeedPreferencesDatabase()
        {
            bool seedSucceeded = false;
            try
            {
                using (var context = new PreferencesContext)
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
    }
}
