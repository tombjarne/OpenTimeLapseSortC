using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.IO;
using System.ComponentModel;
using System.Windows.Input;
using OpenTimelapseSort.Models;
using OpenTimelapseSort.DataServices;
using OpenTimelapseSort.Contexts;

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

        public static StackPanel Import()
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
                DirectoryReference references = new DirectoryReference(RenderDirectories);

                // instead of files, filename fetch from object that need to be created beginning of this function

                return references(files, filename);
            } else
            {
                return new StackPanel(); //TODO: remove, test only
            }
        }

        private static StackPanel RenderDirectories(IEnumerable<String> FileNames, String DirName)
        {
            Debug.WriteLine("FILENAMES: "+FileNames.ToHashSet());
            StackPanel sp = new StackPanel();
            sp.Orientation = Orientation.Vertical;

            // only provisional
            string [] images = Directory.GetFiles(DirName);

            for (int i = 0; i < images.Length; i++)
            {
                Rectangle rect = new Rectangle();
                rect.Width = 100;
                rect.Height = 100;
                rect.Margin = new Thickness(5);
                rect.Fill = i % 2 == 0 ? Brushes.Black : Brushes.Salmon;
                sp.Children.Add(rect);
            }
            return sp;
        }

    }
}
