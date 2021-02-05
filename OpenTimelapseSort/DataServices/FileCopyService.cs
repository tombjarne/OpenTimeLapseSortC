﻿using System;
using OpenTimelapseSort.Models;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace OpenTimelapseSort.DataServices
{
    internal class FileCopyService
    {
        private readonly DbService _dbService = new DbService();
        public delegate void ErrorMessage(string errorMessage);

        /// <summary>
        ///     CopyFiles()
        ///     Actually copies the matched directories to their destination
        ///     Creates a new directory if mainDirectoryPath does not point to valid file location
        ///     Sets <see cref="SDirectory.Target" /> attribute to the actual new location
        ///     Calls <see cref="_dbService" /> to save the just copied files into the database
        /// </summary>
        public async Task<List<SDirectory>> CopyFiles(
            List<SDirectory> currentDirectories, string path, ErrorMessage setErrorMessage)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            try
            {
                foreach (var directory in currentDirectories)
                {
                    var destination = path + @"\" + directory.Name;
                    directory.Target = path;
                    Directory.CreateDirectory(destination);

                    foreach (var image in directory.ImageList)
                    {
                        var source = Path.Combine(image.Origin);
                        image.Target = destination;
                        File.Copy(source, destination + @"\" + image.Name, true);
                    }

                    await _dbService.UpdateDirectoryAsync(directory);
                }
            }
            catch (UnauthorizedAccessException)
            {
                setErrorMessage("Cannot write to this folder.");
                foreach (var directory in currentDirectories)
                {
                    await _dbService.UpdateImportAfterRemovalAsync(directory.Id);
                }
            }

            return currentDirectories;
        }
    }
}