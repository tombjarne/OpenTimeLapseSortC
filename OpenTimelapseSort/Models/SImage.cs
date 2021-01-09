using System;
using System.Collections.Generic;
using OpenTimelapseSort.Constants;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;

public class SImage
{
    [Key][Required]
    public int id { get; set; }
    [ForeignKey("directoryId")] [Required]
    public int directoryId { get; set; }

    private Dictionary<META_ATTRIBUTE, string> meta = new Dictionary<META_ATTRIBUTE, string>();
    [Required]
    public string name { get; set; }
    [Required]
    public string target { get; set; }
    public DateTime fileTime { get; set; }
    public long fileSize { get; set; }
    [Required]
    public string parentInstance { get; set; }

    public SDirectory parentDirectory { get; set; }

    public SImage(string name, string target, string parentInstance)
    {
        this.name = name;
        this.target = target;
        this.parentInstance = parentInstance;
        fileTime = File.GetCreationTime(target);
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