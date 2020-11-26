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

        public Import()
        {
            timestamp = System.DateTime.Today;
            importDate = timestamp.ToString();
        }

        public HashSet<Directory> directories;
        private DateTime timestamp;
        public String importDate;
        public int length { get; set; }

        public bool tryPush(Directory directory)
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
