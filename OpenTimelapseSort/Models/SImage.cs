using OpenTimelapseSort.Constants;
using System;
using System.Collections.Generic;
using System.IO;

public class SImage
{
    public string Id { get; set; }
    public string DirectoryId { get; set; }

    private readonly Dictionary<META_ATTRIBUTE, string> Meta = new Dictionary<META_ATTRIBUTE, string>();
    public string Name { get; set; }
    public string Target { get; set; }
    public DateTime FileTime { get; set; }
    public long FileSize { get; set; }
    public string ParentInstance { get; set; }
    public SDirectory ParentDirectory { get; set; }
    public double Lumen { get; set; }
    public long Colors { get; set; }

    public SImage(string name, string target, string parentInstance)
    {
        this.Name = name;
        this.Target = target;
        this.ParentInstance = parentInstance;
        FileTime = File.GetCreationTime(target);
    }

    public void SetTimestamp(string value)
    {
        Meta.Add(META_ATTRIBUTE.Timestamp, value);
    }

    public void SetSize(string value)
    {
        Meta.Add(META_ATTRIBUTE.Filesize, value);
    }
}