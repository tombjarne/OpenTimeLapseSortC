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

        private DBService service = new DBService();
        private MatchingService matching = new MatchingService();
        private List<Import> imports;

        public delegate void ImageListingProgress(int count, List<Image> imageList);
        public delegate void SortProgress(StackPanel panel);
        public delegate void InitFetch(StackPanel panel);
        public delegate void ViewUpdate(List<ImageDirectory> directories);

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
            service = new DBService();
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

        public List<Image> GetImages(int id)
        {
            return new List<Image>();
        }


        //
        // is called during creation of View
        // fetches all saved elements from database
        // renders elements accordingly through call to Render()
        public StackPanel InitialiseView()
        {
            imports = service.ReturnImports();

            if (imports.Capacity > 0)
            {
                StackPanel allImports = new StackPanel();
                StackPanel directoryPanel = new StackPanel();

                foreach (Import import in imports)
                {
                    try
                    {
                        StackPanel importPanel = new StackPanel();
                        foreach (ImageDirectory directory in import.directories)
                        {
                            // add on click event
                            // add directoryPanel to importPanel
                            //importPanel.Children.Add(directoryPanel);

                            Rectangle rect2 = new Rectangle();
                            rect2.Width = 100;
                            rect2.Height = 100;
                            rect2.Margin = new Thickness(5);
                            rect2.Fill = Brushes.Blue;
                            directoryPanel.Children.Add(rect2);

                        }
                        allImports.Children.Add(importPanel);
                    }
                    catch (Exception e)
                    {
                        // Test purposes
                        Console.WriteLine(e.StackTrace);
                    }

                    allImports.Width = 300;
                    allImports.Height = 100;
                    allImports.Margin = new Thickness(5);

                    TextBox importName = new TextBox();
                    importName.Text = "Test Import";
                    //allImports.Children.Add(directoryPanel);
                    //allImports.Children.Add(importName);

                }
                return directoryPanel;

            }
            else
            {
                StackPanel errorStackPanel = new StackPanel();
                // add attributes and text to warning
                return errorStackPanel;
            }

        }

        public void Import(string name, ImageListingProgress listProgress)
        //public StackPanel Import(string name)
        {
            List<Image> imageList = new List<Image>();
            var files = Directory.EnumerateFileSystemEntries(name).ToList();
            int length = files.Count();

            if (length > 0)
            {
                for (int i = 0; i < length; i++)
                {
                    FileInfo info = new FileInfo(files[i]);

                    if (Directory.Exists(files[i]))
                    {
                        string[] subDirImages = Directory.GetFiles(files[i]);
                        FileInfo subDirInfo = new FileInfo(subDirImages[i]);
                        var subDirLength = Directory.EnumerateFiles(files[i]).ToList().Count();

                        for (int p = 0; p < subDirLength; p++)
                        {
                            Image image = new Image(Path.GetFileName(subDirImages[i]), subDirInfo.FullName, subDirInfo.DirectoryName);
                            imageList.Add(image);
                        }
                    }
                    else
                    {
                        Image image = new Image(Path.GetFileName(files[i]), info.FullName, info.DirectoryName);
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

        public async void SortImages(List<Image> imageList, ViewUpdate update)
        {
            Task sortingTask = Task
                .Run(() => {
                    matching.SortImages(imageList, async (List<ImageDirectory> directories) =>
                    {
                        // TODO: parameterize path to images
                        string destination = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
                        string source;
                        string mainDirectory = destination + @"\OTS_IMG";

                        if (!Directory.Exists(mainDirectory))
                            Directory.CreateDirectory(mainDirectory);

                        try
                        {
                            Import import = new Import(false)
                            {
                                id = DateTime.Now.Millisecond + DateTime.Today.Day,
                                name = "Nightsky_Timelapse",
                                importDate = DateTime.Today.ToShortDateString(),
                                length = 10,
                                target = "killmenow"
                            };
                            await service.SaveImportAsync(import);
                            // TODO: add missing import instance
                            foreach (var directory in directories)
                            {
                                destination = mainDirectory + @"\" + directory.name;
                                Directory.CreateDirectory(destination);

                                directory.id += DateTime.Today.Millisecond;
                                directory.importId = import.id;
                                await service.SaveImageDirectoryAsync(directory);

                                foreach (var image in directory.imageList)
                                {
                                    source = Path.Combine(image.target);
                                    File.Copy(source, destination + @"\" + image.name, true);
                                    image.id = DateTime.Now.Millisecond;
                                    image.directoryId = directory.id;
                                    Debug.WriteLine(image);
                                    await service.SaveImageAsync(image);
                                }
                                destination = @"C:\Users\bjarn\Bilder";
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
