using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public class Import
{
    [Key]
    public int id { get; set; }
    public string target { get; set; }
    public string name { get; set; } // enable user to change name of import :-)
    public virtual List<ImageDirectory> directories { get; set; }
    public DateTime timestamp { get; set; }
    public string importDate { get; set; }
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

    public bool tryPush(ImageDirectory directory)
    {
        if(directory.getTimestamp() == timestamp)
        {
            if(this.directories != null)
            {
                directories.Add(directory);
            }
            else
            {
                directories = new List<ImageDirectory>();
                if (this.tryPush(directory))
                {
                    return true;
                }
            }
            return true;
        } else
        {
            return false;
        }
    }

}
