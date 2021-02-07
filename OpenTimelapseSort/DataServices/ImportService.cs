using OpenTimelapseSort.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OpenTimelapseSort.DataServices
{
    internal class ImportService
    {
        /// <summary>
        ///     ErrorMessage
        ///     holds provided error message and delegates to <see cref="ViewModels.MainViewModel.HandleError" />
        /// </summary>
        /// <param name="errorMessage"></param>
        public delegate void ErrorMessage(string errorMessage);

        /// <summary>
        ///     Import()
        ///     collects all files of a specified directory from a maximum depth of 2
        ///     returns all files found as a List
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="setErrorMessage"></param>
        public List<SImage> Import(string origin, ErrorMessage setErrorMessage)
        {
            var images = new List<SImage>();

            var files = Directory.EnumerateFileSystemEntries(origin).ToList();
            var length = files.Count();

            try
            {
                for (var i = 0; i < length; i++)
                {
                    var file = files[i];

                    if (Directory.Exists(file))
                    {
                        // if current file is a directory, iterate through it

                        var subDirImages = Directory.GetFiles(file);
                        var subDirInfo = new FileInfo(subDirImages[i]);
                        var subDirLength = Directory.EnumerateFiles(file).ToList().Count();

                        for (var p = 0; p < subDirLength; p++)
                        {
                            var subDirFile = subDirImages[p];
                            images.Add(CreateImage(subDirFile, subDirInfo));
                        }
                    }
                    else
                    {
                        var info = new FileInfo(file);
                        images.Add(CreateImage(file, info));
                    }
                }
            }
            catch (Exception)
            {
                setErrorMessage("The directory did not contain any files.");
            }
            return images;
        }

        /// <summary>
        ///     CreateImage()
        ///     Creates an instance of <see cref="SImage" /> with the passed attributes
        /// </summary>
        /// <param name="file"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        private static SImage CreateImage(string file, FileInfo info)
        {
            var image = new SImage
            (
                Path.GetFileName(file),
                info.FullName,
                info.DirectoryName
            )
            {
                Id = Guid.NewGuid().ToString(),
                FileSize = info.Length / 1000
            };

            return image;
        }
    }
}
