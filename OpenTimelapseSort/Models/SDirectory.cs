using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OpenTimelapseSort.Models
{
    public class SDirectory
    {
        public SDirectory(string origin, string name)
        {
            Timestamp = DateTime.Today;
            Origin = origin;
            Name = name;
            ImageList = new List<SImage>();
        }
        [Key]
        public string Id { get; set; }
        public string ImportId { get; set; }
        public List<SImage> ImageList { get; set; }
        public DateTime Timestamp { get; set; }
        public string Origin { get; set; }
        public string Target { get; set; }
        public string Name { get; set; }
        public SImport ParentImport { get; set; }
    }
}