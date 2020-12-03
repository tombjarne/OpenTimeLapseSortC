using System;
using System.Collections.Generic;
using System.Text;

using OpenTimelapseSort.Models;

namespace OpenTimelapseSort.Models
{
    class Import
    {
        // only one instance per day ( for Import )
        // save Import instances with AUTOINCREMENT ID in database 

        public HashSet<ImageDirectory> directories;
        public DateTime timestamp { get; set; }
        public string importDate { get; set; }

        public string name { get; set; } // enable user to change name of import :-)
        public int length { get; set; }

        public Import(HashSet<ImageDirectory> directories, bool fetch)
        {
            if (!fetch)
            {
                timestamp = System.DateTime.Today;
                importDate = timestamp.ToString();
            }
            this.directories = directories;

        }

        // tryPush will be called during import
        public bool tryPush(ImageDirectory directory)
        {
            if(directory.getTimestamp() == timestamp)
            {
                directories.Add(directory);
                return true;
            } else
            {
                // false means a new Import Object has to be created as the dates vary
                return false;
            }
        }

    }
}
