using OpenTimelapseSort.Contexts;
using OpenTimelapseSort.DataServices;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

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
                    
                    service.SeedDatabase();
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
                            Image image = new Image(subDirImages[i], subDirInfo.FullName);
                            imageList.Add(image);
                        }
                    }
                    else
                    {
                        Image image = new Image(files[i], info.FullName);
                        imageList.Add(image);
                    }
                }

                listProgress(imageList.Count, imageList);
            }
        }

        public void SortImages(List<Image> imageList, SortProgress callback)
        {
            // RenderDelegate = callback; how to get this into RenderElement?

            Debug.WriteLine(imageList.Count);
            matching.SortImages(imageList, RenderElement);

            // call matching service with counter callback for progressbar update
            // on file handled call the delegate function in matching service and therefore update its screen value

            // create another delegate that notifies when a directory in matching has been finished
            // delegate function is passed to matching service
            // delegate function holds function call to callback from partial class -> renders element


            //matching.SortImages(imageList);

            // TODO: sort the created images in the matchingservice!
            // TODO: update the images paths in matchingservice according to their sorting and target!

            // TODO: return new directory and import instances
            // TODO: render accordingly to sorted information!

            // TODO: create directory element
            // TODO: add "reimport" button to directory
            // TODO: add directory to import element
        }

        //
        // function that renders directories, images or imports
        // is explicitly called from view and returns a visual element on success in matching service
        // 
        public void RenderElement(object obj)
        {
            var type = obj.GetType();
            var newElement = new StackPanel();

            if(type == typeof(ImageDirectory))
            {
                newElement = RenderDirectory((ImageDirectory)obj);
            }
            else if (type == typeof(Image))
            {
                newElement = RenderImage((Image)obj);
            }
            else if (type == typeof(Import))
            {
                newElement = RenderImport((Import)obj);
            }

            //callback(newElement);
        }


        //
        // following functions are called from the delegated method that is called in matching service
        // the passed function RenderElement then decides which function to call to pass the newly generated
        // element back to the view
        private StackPanel RenderImport(Import import)
        {
            return new StackPanel();
        }

        private StackPanel RenderDirectory(ImageDirectory directory)
        {
            return new StackPanel();
        }

        private StackPanel RenderImage(Image image)
        {
            return new StackPanel();
        }
    }
}
