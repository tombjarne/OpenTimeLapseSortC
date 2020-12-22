using Microsoft.WindowsAPICodePack.Dialogs;
using OpenTimelapseSort.Contexts;
using OpenTimelapseSort.DataServices;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace OpenTimelapseSort
{
    class MainViewModel
    {

        private delegate StackPanel DirectoryReference(IEnumerable<String> FileNames, String DirName);
        private delegate StackPanel ImportReference(List<Import> imports, String ImportName);

        private readonly BackgroundWorker worker;
        private readonly ICommand progressBarInvocation;
        private int importProgress;

        private DBService service;
        private MatchingService matching;
        private List<Import> imports;

        public MainViewModel()
        {
            //init db service 
            //fetch db entries
            //initialize local values

            initialiseDBService();
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

        public static StackPanel TestView()
        {
            List<Import> ti = new List<Import>();
            DBService sv = new DBService();
            ti = sv.ReturnImports();

            // TEST PURPOSES ONLY

            StackPanel directoryPanel = new StackPanel();
            for (int i = 0; i < 5; i++)
            {
                Rectangle rect2 = new Rectangle();
                rect2.Width = 100;
                rect2.Height = 100;
                rect2.Margin = new Thickness(5);
                directoryPanel.Children.Add(rect2);
            }

            return directoryPanel;

        }

        public StackPanel InitialiseView()
        {
            imports = service.ReturnImports();

            // TEST PURPOSES ONLY
            /*
            StackPanel directoryPanel = new StackPanel();
            for (int i = 0; i < 5; i++)
            {
                Rectangle rect2 = new Rectangle();
                rect2.Width = 100;
                rect2.Height = 100;
                rect2.Margin = new Thickness(5);
                rect2.Fill = Brushes.Blue;
                directoryPanel.Children.Add(rect2);
            }

            return directoryPanel;
            */
            
            // TODO: need to fix structure and associations
            // ensure that directories and imports have right amound of count

            if(imports.Capacity > 0)
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
                    } catch (Exception e)
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

            } else
            {
                StackPanel errorStackPanel = new StackPanel();
                // add attributes and text to warning
                return errorStackPanel;
            }
            
        } 

        private StackPanel RenderImports()
        {
            //render all directories for all imports in double for loop ( foreach )
            //for(){
            //for () { }}
            return new StackPanel();
        }

        public int ImportProgress
        {
            get { return this.importProgress; }
            private set
            {
                if (this.importProgress != value)
                {
                    this.importProgress = value;
                    //this.OnPropertyChanged(() => this.ImportProgress);
                }
            }
        }

        public static StackPanel Import(string name)
        {

            List<Image> imageList = new List<Image>();

            CommonOpenFileDialog fileChooser = new CommonOpenFileDialog();
            fileChooser.InitialDirectory = @"C:\";
            fileChooser.Title = "Choose Directory";
            fileChooser.IsFolderPicker = true;
            fileChooser.Multiselect = true;

            //TODO: find proper way of returning a stackpanel on else path

            if (fileChooser.ShowDialog() == CommonFileDialogResult.Ok)
            {
                string chosenDirectory = fileChooser.FileName;
                IEnumerable<string> files = fileChooser.FileNames;

                // instead of files, filename fetch from object that need to be created beginning of this function

                if (files.Count() > 0)
                {
                    StackPanel import = new StackPanel();

                    // TODO: add changable text field to import element

                    string[] images = Directory.GetFiles(fileChooser.FileName);
                    int length = images.Length;

                    for (int i = 0; i < length; i++)
                    {
                        FileInfo info = new FileInfo(images[i]);

                        // Attention: does not support more depth than two! Intended to use with camera folders only!
                        // TODO: check for file endings!
                        if((File.Exists(images[i]) && info.Directory != null) && !info.FullName.Contains("")) //file is directory TODO: check for invalid endings
                        {
                            string[] subDirImages = Directory.GetFiles(images[i]);
                            FileInfo subDirInfo = new FileInfo(subDirImages[i]);

                            for(int p = 0; p < length; p++)
                            {
                                Image image = new Image(subDirImages[i], subDirInfo.FullName);
                                imageList.Add(image);
                            }
                        } 
                        else
                        {
                            Image image = new Image(images[i], info.FullName);
                            imageList.Add(image);
                        }
                    }

                    //matching.SortImages(imageList);

                    // TODO: sort the created images in the matchingservice!
                    // TODO: update the images paths in matchingservice according to their sorting and target!

                    // TODO: return new directory and import instances
                    // TODO: render accordingly to sorted information!

                    RenderDirectory();

                    // TODO: create directory element
                    // TODO: add "reimport" button to directory
                    // TODO: add directory to import element

                    return import;
                } else
                {
                    return new StackPanel();
                    // TODO: add "could not import any files" to StackPanel, add listener to autodestruct after 15 seconds
                }
            } else
            {
                return new StackPanel();
                // TODO: add "could not import any files" to StackPanel, add listener to autodestruct after 15 seconds
            }
        }

        private static StackPanel RenderDirectory()
        {
            // TODO: replace with proper elements 

            StackPanel sp = new StackPanel();
            Rectangle rect = new Rectangle();
            rect.Width = 100;
            rect.Height = 100;
            rect.Margin = new Thickness(5);
            rect.Fill = Brushes.Salmon;
            sp.Children.Add(rect);

            return sp;
        }

    }
}
