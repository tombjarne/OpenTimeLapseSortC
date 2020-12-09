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
        private delegate StackPanel ImportReference(ArrayList imports, String ImportName);

        private readonly BackgroundWorker worker;
        private readonly ICommand progressBarInvocation;
        private int importProgress;

        DBService service;
        List<Import> imports;

        public MainViewModel()
        {
            //init db service 
            //fetch db entries
            //initialize local values

            initialiseDBService();
            InitialiseView();
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
                    InitialiseView();
                } catch(Exception e)
                {
                    Console.WriteLine(e.StackTrace);
                }
            }
        }

        public StackPanel InitialiseView()
        {
            imports = service.ReturnImports();
            Console.WriteLine(imports);

            if(imports.Count > 0)
            {
                StackPanel allImports = new StackPanel();
                foreach (Import import in imports)
                {
                    try
                    {
                        StackPanel importPanel = new StackPanel();
                        foreach (ImageDirectory directory in import.directories)
                        {
                            StackPanel directoryPanel = new StackPanel();
                            // add on click event
                            // add directoryPanel to importPanel
                            importPanel.Children.Add(directoryPanel);
                        }
                        allImports.Children.Add(importPanel);
                    } catch (Exception e)
                    {
                        // Test purposes
                        Console.WriteLine(e.StackTrace);
                    }
                }
                return allImports;

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
