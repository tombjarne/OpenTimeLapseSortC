using System;
using System.Collections.Generic;
using System.Text;

using OpenTimelapseSort.Models;
using OpenTimelapseSort.Contexts;
using OpentimelapseSort.Models;

/*
using (var context = new ImportContext())
{
    var import = new Import
    {
        // fields
    };

    context.Import.Add(import);
    context.SaveChanges();
}
*/

namespace OpenTimelapseSort.DataServices
{

    class DBService
    {

        //maybe also this?
        //public delegate HashSet<Import<Directory>> ?

        //public delegate HashSet<Import> Directories(HashSet<ImageDirectory> directories); // name is obvious and can be fetched from indexes
        public delegate HashSet<Import> Imports(HashSet<ImageDirectory> directories); // name is obvious and can be fetched from indexes
        public delegate HashSet<Image> Images(HashSet<Image> images); // name is obvious and can be fetched from indexes

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

        public HashSet<Import> ReturnImports()
        {
            // fetch from DB
            // save into Object
            // return Object

            using (var context = new ImportContext())
            {
                HashSet<Import> imports = new HashSet<Import>();
                HashSet<ImageDirectory> directories = new HashSet<ImageDirectory>(); // images are fetched on click to reduce overhead

                foreach (ImageDirectory directory in context.ImageDirectory)
                {
                    ImageDirectory newDirectory = new ImageDirectory(
                        directory.name,
                        directory.target,
                        new List<Image>()
                    );
                    directories.Add(newDirectory);
                }

                foreach (Import import in context.Import) //rename to plural -> Imports?
                {
                    Import newImport = new Import(directories, true); //construct newImport
                    newImport.importDate = import.importDate;
                    newImport.timestamp = import.timestamp; //convert string to date 
                }

                return imports;
            }
        }

        /*
         private HashSet<Image> GetImages(HashSet<Image> images) // gets all images related to a passed directory
        {

        }

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
