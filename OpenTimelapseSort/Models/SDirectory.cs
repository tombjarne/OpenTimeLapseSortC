using System;
using System.Collections.Generic;
using System.IO;

public class SDirectory
{
    public string Id { get; set; }
    public string ImportId { get; set; }
    public List<SImage> ImageList { get; set; }
    public DateTime Timestamp { get; set; }
    public string Target { get; set; }
    public string Name { get; set; }
    public SImport ParentImport { get; set; }

    public SDirectory(string target, string name)
    {
        // TODO: enable only on first creation!
        Timestamp = DateTime.Today;
        Target = target;
        Name = name;
        ImageList = new List<SImage>();
    }

    public void Delete(SImage image)
    {
        try
        {
            File.Delete(Path.GetFullPath(image.Target));
        }
        catch (FileNotFoundException e)
        {
            Console.WriteLine(e);
        }
    }
}