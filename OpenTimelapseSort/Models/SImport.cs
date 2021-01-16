using System;
using System.Collections.Generic;

public class SImport
{
    public string Id { get; set; }
    public string Target { get; set; }
    public string Name { get; set; } // enable user to change Name of import :-)
    public virtual List<SDirectory> Directories { get; set; }
    public DateTime Timestamp { get; set; }
    public string ImportDate { get; set; }
    public int Length { get; set; }
    public SImport()
    {
        Timestamp = DateTime.Today;
        ImportDate = Timestamp.ToShortDateString();
    }
}
