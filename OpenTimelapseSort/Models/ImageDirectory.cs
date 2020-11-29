using System;
using System.Collections.Generic;
using System.Text;

using OpenTimelapseSort.Models;

class ImageDirectory
{
    public ImageDirectory(String target, String name)
    {
        timestamp = DateTime.Today;
        this.target = target;
        this.name = name;
    }

    private List<Image> imageList = new List<Image>();

    private DateTime timestamp { get; set; }
    private String target { get; set; }
    private String name { get; set; }

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

}