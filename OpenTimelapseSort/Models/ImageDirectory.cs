using System;
using System.Collections.Generic;
using System.IO;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class ImageDirectory
{
    [Key][Required]
    public int id { get; set; }

    [ForeignKey("importId")] [Required]
    public int importId { get; set; }
    public virtual List<Image> imageList { get; set; }
    public DateTime timestamp { get; set; }
    [Required]
    public string target { get; set; }
    [Required]
    public string name { get; set; }

    public ImageDirectory(String target, String name)
    {
        // TODO: enable only on first creation!
        timestamp = DateTime.Today;

        this.target = target;
        this.name = name;
        imageList = new List<Image>();
    }

    public DateTime getTimestamp()
    {
        return timestamp;
    }

    public void pushImage(Image image)
    {
        imageList.Add(image);
    }

    public Image getImage(int index)
    {
        return imageList[index];
    }

    public void Delete(Image image)
    {
        try
        {
            File.Delete(Path.GetFullPath(image.target));
        }
        catch (FileNotFoundException e)
        {
            Console.WriteLine(e);
        }
    }
}