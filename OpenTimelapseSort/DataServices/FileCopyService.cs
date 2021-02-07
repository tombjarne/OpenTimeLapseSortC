using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using OpenTimelapseSort.Models;

namespace OpenTimelapseSort.DataServices
{
    internal class FileCopyService
    {
        private readonly DbService _dbService = new DbService();

        /// <summary>
        ///     ErrorMessage
        ///     holds provided error message and delegates to <see cref="ViewModels.MainViewModel.HandleError" />
        /// </summary>
        /// <param name="errorMessage"></param>
        public delegate void ErrorMessage(string errorMessage);

        /// <summary>
        ///     CopyFiles()
        ///     copies the matched directories to their destination
        ///     creates a new directory if mainDirectoryPath does not point to valid file location
        ///     sets <see cref="SDirectory.Target" /> attribute to the actual new location
        ///     calls <see cref="_dbService" /> to save the just copied files into the database
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
                    // assemble path from target path and the directories name
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
            catch (UnauthorizedAccessException) // thrown when e.g. sys files are copied
            {
                setErrorMessage("Cannot write to this folder.");
                foreach (var directory in currentDirectories)
                    await _dbService.UpdateImportAfterRemovalAsync(directory.Id);
            }

            return currentDirectories;
        }
    }
}