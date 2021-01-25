using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Microsoft.VisualBasic;

namespace OpenTimelapseSort.Models
{
    public class SDirectory
    {
        public string Id { get; set; }
        public string ImportId { get; set; }
        public List<SImage> ImageList { get; set; }
        public DateTime Timestamp { get; set; }
        public string Origin { get; set; }
        public string Target { get; set; }
        public string Name { get; set; }
        public SImport ParentImport { get; set; }

        public SDirectory(string origin, string name)
        {
            Timestamp = DateTime.Today;
            Origin = origin;
            Name = name;
            ImageList = new List<SImage>();
        }

        public bool Delete()
        {
            try
            {
                var currentPosition = Target + @"\" + Name;

                if (ImageList.Count > 0)
                {
                    var directory = new DirectoryInfo(currentPosition);

                    foreach (var file in directory.GetFiles())
                    {
                        file.Delete();
                    }

                    foreach (var dir in directory.GetDirectories())
                    {
                        dir.Delete(true);
                    }
                }

                Directory.Delete(Path.GetFullPath(Target + @"\" + Name));
                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.InnerException);
                return false;
            }
        }

        public void ChangeDirectoryName(string newName)
        {
            var sanitizedName = (Target + @"\" + newName).Trim();
            FileSystem.Rename(Target + @"\" + Name, sanitizedName);
        }
    }
}