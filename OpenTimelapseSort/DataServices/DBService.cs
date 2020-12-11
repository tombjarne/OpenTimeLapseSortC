using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;

using OpenTimelapseSort.Models;
using OpenTimelapseSort.Contexts;
using OpentimelapseSort.Models;
using Microsoft.EntityFrameworkCore;

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

            return new Preferences(true, true, 2.0, 1); // TODO: remove, test purpose only
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

        // fetches all images that belong to a directory that already exists and is fetched from db

         private List<Image> GetImages(ImageDirectory directory) // gets all images related to a passed directory
        {
            using (var context = new ImportContext())
            {
                List<Image> images = new List<Image>();
                int directoryId = directory.id;

                //TODO: add LINQ statement to select only those images that belong to the submitted directory ( id ) 

                foreach (Image image in context.Images)
                {
                    Image newImage = new Image(
                        image.name,
                        image.target
                    );
                    images.Add(image);
                }

                return images;
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
