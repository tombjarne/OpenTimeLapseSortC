using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.VisualBasic;

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

        public string Id { get; set; }
        public string ImportId { get; set; }
        public List<SImage> ImageList { get; set; }
        public DateTime Timestamp { get; set; }
        public string Origin { get; set; }
        public string Target { get; set; }
        public string Name { get; set; }
        public SImport ParentImport { get; set; }

        public bool Delete()
        {
            try
            {
                var currentPosition = Target + @"\" + Name;

                if (ImageList.Count > 0)
                {
                    var directory = new DirectoryInfo(currentPosition);

                    foreach (var file in directory.GetFiles()) file.Delete();

                    foreach (var dir in directory.GetDirectories()) dir.Delete(true);
                }

                Directory.Delete(Path.GetFullPath(Target + @"\" + Name));
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public bool ChangeDirectoryName(string newName)
        {
            var changeWasClean = true;

            newName = Regex.Replace(newName, @"[^0-9a-zA-Z!$* \-_]+", "");
            var sanitizedName = (Target + @"\" + newName).Trim();

            if (newName == "")
            {
                sanitizedName += "D";
                changeWasClean = false;
            }

            if (File.Exists(sanitizedName))
            {
                sanitizedName += Id;
                changeWasClean = false;
            }

            if (changeWasClean)
            {
                FileSystem.Rename(Target + @"\" + Name, sanitizedName);
                Name = newName;
            }

            return changeWasClean;
        }
    }
}