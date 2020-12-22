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

        public void InitImport(string name)
        {
            Import(name);
        }

        public static StackPanel Import(string name)
        {
            CommonOpenFileDialog fileChooser = new CommonOpenFileDialog();
            fileChooser.InitialDirectory = @"C:\";
            fileChooser.Title = "Choose Directory";
            fileChooser.IsFolderPicker = true;
            fileChooser.Multiselect = true;

            //TODO: find proper way of returning a stackpanel on else path

            if (fileChooser.ShowDialog() == CommonFileDialogResult.Ok)
            {
                //TODO: fetch needed attributes -> might need to change IEnumerable to something else
                string filename = fileChooser.FileName;

                IEnumerable<String> files = fileChooser.FileNames;
                DirectoryReference references = new DirectoryReference(HandleImport);

                // instead of files, filename fetch from object that need to be created beginning of this function

                if (files.Count() > 0)
                {
                    return references(files, filename);
                } else
                {
                    return new StackPanel();
                }
            } else
            {
                return new StackPanel(); //TODO: remove, test only
            }
        }

        private static StackPanel HandleImport(IEnumerable<String> FileNames, String DirName)
        {
            Debug.WriteLine("FILENAMES: "+FileNames.ToHashSet());

            

            StackPanel import = new StackPanel();

            // TODO: add changable text field to import element

            string [] images = Directory.GetFiles(DirName);
            int length = images.Length;

            for (int i = 0; i < length; i++)
            {
                FileInfo info = new FileInfo(images[i]);
                ImageDirectory directory = new ImageDirectory(info.FullName, images[i]);

                RenderDirectory();

                // TODO: add directory to import element
            }
            return import;
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
