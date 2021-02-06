using System;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.VisualBasic;
using OpenTimelapseSort.Models;

namespace OpenTimelapseSort.DataServices
{
    internal class DirectoryDetailService
    {
        /// <summary>
        ///     ErrorMessage
        ///     holds provided error message and delegates to <see cref="ViewModels.MainViewModel.HandleError" />
        /// </summary>
        /// <param name="errorMessage"></param>
        public delegate void ErrorMessage(string errorMessage);

        /// <summary>
        ///     Delete()
        ///     handles deletion of passed <see cref="directory" />
        ///     returns true if successful, otherwise false
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
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
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        ///     ChangeDirectoryName()
        ///     handles name change of provided <see cref="dir" />
        ///     sanitizes new name in <see cref="newName" />
        ///     invokes error message via <see cref="setErrorMessage" /> on fail
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="newName"></param>
        /// <param name="setErrorMessage"></param>
        /// <returns></returns>
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

            return dir;
        }
    }
}