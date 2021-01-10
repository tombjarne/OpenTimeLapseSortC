using OpenTimelapseSort.Contexts;
using OpenTimelapseSort.DataServices;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Path = System.IO.Path;

namespace OpenTimelapseSort
{
    class MainViewModel
    {

        private DbService _dbService = new DbService();
        private readonly MatchingService matching = new MatchingService();

        private List<SImport> imports;

        public delegate void ImageListingProgress(int count, List<SImage> imageList);
        public delegate void ViewUpdate(List<SDirectory> directories);

        public MainViewModel()
        {
            //init db service 
            //service = new DBService();
            //service.

            //fetch db entries
            //initialize local values
            //initialiseDBService();
            //InitialiseView();
        }

        private void initialiseDBService()
        {
            _dbService = new DbService();
            using (var database = new ImportContext())
            {
                try
                {
                    database.Database.EnsureCreated();
                    
                    //service.SeedDatabase();
                    //InitialiseView();
                } catch(Exception e)
                {
                    Console.WriteLine(e.StackTrace);
                }
            }
        }

        public bool PerformAutoSave()
        {
            // collect currently active instances and try to save them into database
            // empty memory before closing
            GC.Collect();
            GC.WaitForPendingFinalizers();
            return true;
        }

        public async Task<List<SDirectory>> GetDirectoriesAsync()
        {
            return await _dbService.GetDirectoriesAsync();
        }

        public async Task<List<SImage>> GetImagesAsync(string id)
        {
            return await _dbService.GetImagesAsync(id);
        }

        public void Import(string name, ImageListingProgress listProgress)
        //public StackPanel Import(string name)
        {
            var imageList = new List<SImage>();
            var files = Directory.EnumerateFileSystemEntries(name).ToList();
            var length = files.Count();

            if (length > 0)
            {
                for (int i = 0; i < length; i++)
                {
                    var file = files[i];
                    var info = new FileInfo(file);

                    if (Directory.Exists(file))
                    {
                        var subDirImages = Directory.GetFiles(file);
                        var subDirInfo = new FileInfo(subDirImages[i]);
                        var subDirLength = Directory.EnumerateFiles(file).ToList().Count();

                        for (int p = 0; p < subDirLength; p++)
                        {
                            var image = new SImage
                            (
                                Path.GetFileName(subDirImages[i]),
                                subDirInfo.FullName,
                                subDirInfo.DirectoryName
                            )
                            {
                                id = System.Guid.NewGuid().ToString()
                            };
                            imageList.Add(image);
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
                            id = System.Guid.NewGuid().ToString()
                        };
                        imageList.Add(image);
                    }
                }
                listProgress(imageList.Count, imageList);
            }
        }

        /**
         * deleteAccount
         *
         * Deletes local database and all persistently saved information
         *
         * @param event         trigger for delete button being clicked
         */

        public void SortImages(List<SImage> imageList, ViewUpdate update)
        {
            var sortingTask = Task
            .Run(() =>
            {
                matching.SortImages(imageList, async (List<SDirectory> directories) =>
                {
                    var destination = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
                    var mainDirectory = destination + @"\OTS_IMG";

                    if (!Directory.Exists(mainDirectory))
                        Directory.CreateDirectory(mainDirectory);

                    try
                    {
                        foreach (var directory in directories)
                        {
                            destination = mainDirectory + @"\" + directory.name;
                            Directory.CreateDirectory(destination);

                            foreach (var image in directory.imageList)
                            {
                                var source = Path.Combine(image.target);
                                File.Copy(source, destination + @"\" + image.name, true);
                            }
                            destination = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.StackTrace);
                    }
                    update(directories);
                });
            });
        }
    }
}
