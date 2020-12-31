using System.Windows.Controls;

public class DCell : GridViewColumn
{
    public string DirectoryName { get; set; }
    public string ImportDate { get; set; }
    public string Target { get; set; }
    public int ImageCount { get; set; }
    int id { get; set; }

    public DCell()
    {

    }
}