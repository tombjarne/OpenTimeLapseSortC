using System;
using System.Collections.Generic;

public class SImport
{
    public string id { get; set; }
    public string target { get; set; }
    public string name { get; set; } // enable user to change Name of import :-)
    public virtual List<SDirectory> directories { get; set; }
    public DateTime timestamp { get; set; }
    public string importDate { get; set; }
    public int length { get; set; }
    public SImport()
    {
        timestamp = DateTime.Today;
        importDate = timestamp.ToShortDateString();
    }

    public void initImportList(List<SDirectory> directories)
    {
        this.directories = directories;
    }

    public bool tryPush(SDirectory directory)
    {
        if (directory.Timestamp == timestamp)
        {
            if (this.directories != null)
            {
                directories.Add(directory);
            }
            else
            {
                directories = new List<SDirectory>();
                if (this.tryPush(directory))
                {
                    return true;
                }
            }
            return true;
        }
        else
        {
            return false;
        }
    }

}
