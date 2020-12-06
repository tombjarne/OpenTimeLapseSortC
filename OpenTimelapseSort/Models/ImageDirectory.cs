using System;
using System.Collections.Generic;
using System.IO;
using System.ComponentModel.DataAnnotations;
using OpenTimelapseSort.Models;

public class ImageDirectory
{
    [Key]
    public int id { get; set; }

    public List<Image> imageList { get; set; }
    public DateTime timestamp { get; set; }
    public String target { get; set; }
    public String name { get; set; }

    public ImageDirectory(String target, String name) //aggregation - cannot exist without images, therefore supply imageList
    {
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