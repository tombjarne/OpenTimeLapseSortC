using System.ComponentModel;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.VisualBasic;
using OpenTimelapseSort.Models;

namespace OpenTimelapseSort.DataServices
{
    internal class DirectoryDetailService
    {
        public delegate void ErrorMessage(string errorMessage);

        public bool Delete(SDirectory directory)
        {
            var target = directory.Target;
            var imageList = directory.ImageList;
            var name = directory.Name;

            try
            {
                var currentPosition = target + @"\" + name;

                if (imageList.Count > 0)
                {
                    var dirInfo = new DirectoryInfo(currentPosition);

                    foreach (var file in dirInfo.GetFiles()) file.Delete();

                    foreach (var dir in dirInfo.GetDirectories()) dir.Delete(true);
                }

                Directory.Delete(Path.GetFullPath(target + @"\" + name));
                return true;
            }
            catch
            {
                return false;
            }
        }

        public SDirectory ChangeDirectoryName(SDirectory dir, string newName, ErrorMessage setErrorMessage)
        {
            var changeWasClean = true;

            newName = Regex.Replace(newName, @"[^0-9a-zA-Z!$* \-_]+", "");
            var sanitizedPath = (dir.Target + @"\" + newName).Trim();

            if (newName == "")
            {
                sanitizedPath += "D";
                changeWasClean = false;
                setErrorMessage("Please provide a proper name!");
            }

            if (File.Exists(sanitizedPath))
            {
                sanitizedPath += dir.Id;
                changeWasClean = false;
                setErrorMessage("Please provide a proper name!");
            }

            if (changeWasClean)
            {
                try
                {
                    FileSystem.Rename(dir.Target + @"\" + dir.Name, sanitizedPath);
                    dir.Name = newName;
                    setErrorMessage("Name was changed successfully");
                }
                catch
                {
                    setErrorMessage("Could not rename. File already exists.");
                }
            }

            return dir;
        }
    }
}
