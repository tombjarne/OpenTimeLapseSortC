using OpenTimelapseSort.Contexts;
using OpenTimelapseSort.DataServices;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using OpenTimelapseSort.Models;

namespace OpenTimelapseSort.ViewModels
{
    internal class MainViewModel
    {
        private DbService _dbService = new DbService();
        private readonly MatchingService _matching = new MatchingService();
        private List<SDirectory> _directories = new List<SDirectory>();
        private readonly List<SImage> _images = new List<SImage>();

        public delegate void ImageListingProgress(int count);
        public delegate void DeletionErrorCallback(string message);
        public delegate void ViewUpdate(List<SDirectory> directories);

        private void InitialiseDbService()
        {
            _dbService = new DbService();
            using var database = new ImportContext();
            database.Database.EnsureCreated();
            //service.SeedDatabase();
        }

        public async Task<List<SDirectory>> GetDirectoriesAsync()
        {
            return await _dbService.GetDirectoriesAsync();
        }

        public async Task DeleteImageDirectory(SDirectory directory, DeletionErrorCallback callback)
        {
            if (directory.Delete())
            {
                var import = directory.ParentImport;
                import.Directories.Remove(directory);

                if (import.Directories.Count == 0)
                    await _dbService.DeleteImportAsync(import.Id);
                else
                    await _dbService.UpdateImportAsync(import);

                callback("Directory was deleted successfully.");
            }
            callback("Directory could not be deleted.");
        }

        public async Task UpdateImageDirectory(SDirectory directory)
        {
            await _dbService.UpdateDirectoryAsync(directory);
        }

        public async Task DeleteImage(SImage image, DeletionErrorCallback callback)
        {
            if (image.Delete())
            {
                var directory = image.ParentDirectory;
                directory.ImageList.Remove(image);

                if (directory.ImageList.Count == 0)
                    await DeleteImageDirectory(directory, callback);

                callback("Image was deleted successfully.");
            }
            callback("Image could not be deleted.");
        }

        public void Import(string name, ImageListingProgress listProgress)
        {
            _images.Clear();

            var files = Directory.EnumerateFileSystemEntries(name).ToList();
            var length = files.Count();

            if (length > 0)
            {
                for (var i = 0; i < length; i++)
                {
                    var file = files[i];
                    Debug.WriteLine(i);

                    if (Directory.Exists(file))
                    {
                        var subDirImages = Directory.GetFiles(file);
                        var subDirInfo = new FileInfo(subDirImages[i]);
                        var subDirLength = Directory.EnumerateFiles(file).ToList().Count();

                        for (var p = 0; p < subDirLength; p++)
                        {
                            Debug.WriteLine(p);
                            var subDirFile = subDirImages[p];
                            _images.Add(CreateImage(subDirFile, subDirInfo));
                        }
                    }
                    else
                    {
                        var info = new FileInfo(file);
                        _images.Add(CreateImage(file, info));
                    }
                }

                listProgress(_images.Count);
            }
        }

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

        /**
         * deleteAccount
         *
         * Deletes local database and all persistently saved information
         *
         * @param event         trigger for delete button being clicked
         */
        public void SortImages(ViewUpdate update)
        {
            var sortingTask = Task.Run(() => { _directories = _matching.MatchImages(_images); });
            Debug.WriteLine(_directories);
            sortingTask.ContinueWith(task => { Callback(update); });
        }

        private void Callback(ViewUpdate update)
        {
            var mainDirectory =
                Environment.GetFolderPath(Environment.SpecialFolder.MyPictures) + @"\OTS_IMG";

            if (!Directory.Exists(mainDirectory))
                Directory.CreateDirectory(mainDirectory);

            foreach (var directory in _directories)
            {
                var destination = mainDirectory + @"\" + directory.Name;
                Directory.CreateDirectory(destination);

                foreach (var image in directory.ImageList)
                {
                    var source = Path.Combine(image.Target);
                    File.Copy(source, destination + @"\" + image.Name, true);
                }
            }

            update(_directories);
        }
    }
}