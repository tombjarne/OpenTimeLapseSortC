using System;
using System.Collections.Generic;
using System.Text;

using OpenTimelapseSort.Models;

class ImageDirectory
{

    private List<Image> imageList = new List<Image>();
    private DateTime timestamp { get; set; }
    public String target { get; set; }
    public String name { get; set; }

    public ImageDirectory(String target, String name, List<Image> imageList) //aggregation - cannot exist without images, therefore supply imageList
    {
        timestamp = DateTime.Today;
        this.target = target;
        this.name = name;
        this.imageList = imageList;
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

}