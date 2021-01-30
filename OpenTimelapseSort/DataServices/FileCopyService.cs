using OpenTimelapseSort.Models;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace OpenTimelapseSort.DataServices
{
    internal class FileCopyService
    {

        private readonly DbService _dbService = new DbService();

        /// <summary>
        ///     CopyFiles()
        ///     Actually copies the matched directories to their destination
        ///     Creates a new directory if mainDirectoryPath does not point to valid file location
        ///     Sets <see cref="SDirectory.Target" /> attribute to the actual new location
        ///     Calls <see cref="_dbService" /> to save the just copied files into the database
        /// </summary>
        public async Task<List<SDirectory>> CopyFiles(List<SDirectory> currentDirectories, string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

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

            return currentDirectories;
            //TODO: in mainviewmodel LoaderVisibility = Visibility.Hidden;
        }
    }
}