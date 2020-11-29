using OpenTimelapseSort.Models;
using System;
using System.Collections.Generic;
using System.Text;

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
