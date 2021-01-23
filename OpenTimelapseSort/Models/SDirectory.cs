using System;
using System.Collections.Generic;
using System.IO;

namespace OpenTimelapseSort.Models
{
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
            Timestamp = DateTime.Today;
            Target = target;
            Name = name;
            ImageList = new List<SImage>();
        }

        public bool Delete()
        {
            try
            {
                File.Delete(Path.GetFullPath(Target));
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}