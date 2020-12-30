using System;
using System.Collections.Generic;
using OpenTimelapseSort.Constants;
using System.ComponentModel.DataAnnotations;

public class Image
{
    [Key]
    public int id { get; set; }

    private Dictionary<META_ATTRIBUTE, string> meta = new Dictionary<META_ATTRIBUTE, string>();
    public string name { get; set; }
    public string target { get; set; }
    public DateTime fileTime { get; set; }
    public long fileSize { get; set; }

    public string parentInstance { get; set; }

    public Image(string name, string target, string parentInstance)
    {
        this.name = name;
        this.target = target;
        this.parentInstance = parentInstance;
    }

    public void SetTimestamp(string value)
    {
        meta.Add(META_ATTRIBUTE.TIMESTAMP, value);
    }

    public void SetSize(string value)
    {
        meta.Add(META_ATTRIBUTE.FILESIZE, value);
    }
} 