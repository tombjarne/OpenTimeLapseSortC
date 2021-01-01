﻿using System;
using System.Collections.Generic;
using OpenTimelapseSort.Contexts;
using OpentimelapseSort.Models;
using System.Linq;

namespace OpenTimelapseSort.DataServices
{

    class DBService
    {

        public DBService()
        {
            // init a delegate that contains values ( HashSet ) of Directory table
            // init a delegate that contains values ( HashSet ) of Import table
            // return delegate to MainViewModel ( so that it can render HashSet of Imports ( which includes HashSet of Directories ) ) 
        }

        public void InitializeDBService()
        {
            /*
            using (var preferencesContext = new PreferencesContext())
            {
                if (!preferencesContext.Any())
                {

                }
            }
            */
        }

        public static Preferences ReturnPreferences()
        {
            // fetch from DB
            // save into Object
            // return Object

            return new Preferences(true, true, 2.0, 50, 1); // TODO: remove, test purpose only
        }

        // returns a list of Import Objects fetched from db

        public List<Import> ReturnImports()
        {
            // fetch from DB
            // save into Object
            // return Object

            using (var context = new ImportContext())
            {
                List<Import> imports = new List<Import>();
                List<ImageDirectory> directories = new List<ImageDirectory>(); // images are fetched on click to reduce overhead

                foreach (ImageDirectory directory in context.ImageDirectories)
                {
                    ImageDirectory newDirectory = new ImageDirectory(
                        directory.name,
                        directory.target
                    );
                    directories.Add(newDirectory);
                }

                foreach (Import import in context.Imports)
                {
                    Import newImport = new Import(true);
                    newImport.initImportList(directories);
                    newImport.importDate = import.importDate;
                    newImport.timestamp = import.timestamp; //convert string to date 
                    imports.Add(newImport);
                }

                Console.WriteLine(imports);
                return imports;
            }
        }


        /**
         * ReturnCurrentImport
         * 
         * Returns the latest Import instance including its directories
         * 
         */

        public Import ReturnCurrentImport()
        {
            using (var context = new ImportContext())
            {
                var import = context.Imports
                    .Select(import => new
                    {
                        fetch = true,
                        directories = import.directories,
                        timestamp = import.timestamp,
                        importDate = import.importDate,
                        id = import.id
                    }).Where(import => import.timestamp == System.DateTime.Today); // might need to implement fuzzy logic if it is midnight
                // TODO: fix casting issue!
                return (Import)import;
            }
        }

        public void SeedDatabase()
        {
            // init db for test purposes

            using (var context = new ImportContext())
            {

                Import import = new Import(false)
                {
                    name = "Urlaub",
                    length = 1,
                    directories = new List<ImageDirectory>(0)
                };
                context.Add(import);
                context.SaveChanges();

                ImageDirectory directory = new ImageDirectory("/", "Urlaub 1")
                {
                    imageList = new List<Image>(0),
                };
                context.Add(directory);
                context.SaveChanges();
            }
        }

        public ImageDirectory GetRandomDirInstance()
        {
            using (var context = new ImportContext())
            {
                var directory = context.ImageDirectories
                        .Single(predicate => predicate.name.Contains("Random"));
                return directory;
            }
        }

        public void UpdateImageDirectory(ImageDirectory directory)
        {
            using (var context = new ImportContext())
            {
                context.Update(directory);
            }        
        }

        public void UpdateImport(Import import)
        {
            using (var context = new ImportContext())
            {
                context.Update(import);
            }
        }

         public List<Image> GetImages(int id)
        {
            using (var context = new ImportContext())
            {
                ImageDirectory directory = (ImageDirectory)context.ImageDirectories
                    .Select(directory => new
                    {
                        id = directory.id,
                        target = directory.target,
                        name = directory.name,
                        timestamp = directory.timestamp,
                        imageList = directory.imageList
                    }).Where(directory => directory.id == id);

                List<Image> imageList = directory.imageList;


                // might still need this if LINQ and EFcore does not return a list of objects in imageList of directory
                /*
                foreach (Image image in imageList)
                {
                    Image newImage = new Image(
                        image.name,
                        image.target,
                        image.parentInstance
                    );
                }
                */

                return imageList;
            }
        }

        /*
        private HashSet<ImageDirectory> GetDirectories(HashSet<ImageDirectory> directories) // gets all directories related to a passed import
        {
            // loop over all entries and create new objects
            // save objects to HashSet
            // repeat for each directory
            
        }

        private HashSet<Import> GetImports() // gets all imports from database
        {
            // loop over all entries and create new objects
            // save objects to HashSet
            // repeat for each directory

            Imports imports = new Imports(GetDirectories);
            return imports;
        }
        */

        //use Entity Framework here to fetch and store

    }
}
