using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using OpenTimelapseSort.Contexts;
using OpenTimelapseSort.DataServices;

namespace OpenTimelapseSort.ViewModels
{
    internal class MainViewModel
    {
        private DbService _dbService = new DbService();
        private readonly MatchingService _matching = new MatchingService();
        private List<SDirectory> _directories = new List<SDirectory>();
        private List<SImage> _images = new List<SImage>();

        public delegate void ImageListingProgress(int count);
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
                    var info = new FileInfo(file);

                    if (Directory.Exists(file))
                    {
                        var subDirImages = Directory.GetFiles(file);
                        var subDirInfo = new FileInfo(subDirImages[i]);
                        var subDirLength = Directory.EnumerateFiles(file).ToList().Count();

                        for (var p = 0; p < subDirLength; p++)
                        {
                            var image = new SImage
                            (
                                Path.GetFileName(subDirImages[i]),
                                subDirInfo.FullName,
                                subDirInfo.DirectoryName
                            )
                            {
                                Id = Guid.NewGuid().ToString(),
                                FileSize = subDirInfo.Length / 1000
                            };
                            _images.Add(image);
                        }
                    }
                    else
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
                        _images.Add(image);
                    }
                }
                listProgress(_images.Count);
            }
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
            var sortingTask = Task.Run( () =>
            {
                _directories = _matching.MatchImages(_images);
            });

            sortingTask.ContinueWith( task =>
            {
                Callback(update);
            });
        }

        private void Callback(ViewUpdate update)
        {
            var destination = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            var mainDirectory = destination + @"\OTS_IMG";

            if (!Directory.Exists(mainDirectory))
                Directory.CreateDirectory(mainDirectory);

            try
            {
                foreach (var directory in _directories)
                {
                    destination = mainDirectory + @"\" + directory.Name;
                    Directory.CreateDirectory(destination);

                    foreach (var image in directory.ImageList)
                    {
                        var source = Path.Combine(image.Target);
                        File.Copy(source, destination + @"\" + image.Name, true);
                    }

                    destination = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.StackTrace);
            }

            update(_directories);
        }
    }
}