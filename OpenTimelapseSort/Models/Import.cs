using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public class Import
{
    // only one instance per day ( for Import )
    // save Import instances with AUTOINCREMENT ID in database 
    [Key]
    public int id { get; set; }

    // TODO: create own class for directorylist and map with import id
    public List<ImageDirectory> directories { get; set; }
    public DateTime timestamp { get; set; }
    public string importDate { get; set; }
    public string name { get; set; } // enable user to change name of import :-)
    public int length { get; set; }
    public bool fetch { get; set; } // change permissions?

    public Import(bool fetch)
    {
        if (!fetch)
        {
            timestamp = System.DateTime.Today;
            importDate = timestamp.ToString();
        }
        this.fetch = fetch;
    }

    public void initImportList(List<ImageDirectory> directories)
    {
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
