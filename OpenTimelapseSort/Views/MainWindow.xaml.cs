using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.WindowsAPICodePack.Dialogs;
using OpenTimelapseSort.Mvvm;
using System;
using System.IO;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Windows.Threading;
using System.Runtime.CompilerServices;
using System.Windows.Shapes;

namespace OpenTimelapseSort.Views
{
    public partial class MainWindow : Window
    {

        //////////////////////////////////////////////////////////
        //////                    VARIABLES                 //////
        //////////////////////////////////////////////////////////

        //private delegate void WarningReference(string errorHeadline, string errorDetails);


        ObservableCollection<ListView> listViews = new ObservableCollection<ListView>();

        ObservableCollection<ImageDirectory> directories = new ObservableCollection<ImageDirectory>();
        ObservableCollection<Import> imports = new ObservableCollection<Import>();
        ObservableCollection<Image> images = new ObservableCollection<Image>();

        MainViewModel mainViewModel = new MainViewModel();


        //////////////////////////////////////////////////////////
        //////                   CONSTRUCTOR                //////
        //////////////////////////////////////////////////////////

        public MainWindow()
        {
            InitializeComponent();
            SetScreenSize();
            FetchOnStartup();

           // DirectoryViewer1.Items.Add(new DCell { DirectoryName = "Test", ImportDate = "Test2", ImageCount = 420 });
        }

        //////////////////////////////////////////////////////////
        //////                  XAMLFUNCTIONS               //////
        //////////////////////////////////////////////////////////

        private void FetchOnStartup()
        {
            // TODO: make it async!
            // check for db
            // if not exists create new one
            // start subtask to notify user about that

            // fetch entries from db and render them / if there is anything to fetch / also notify user about that
            // if db did not exist -> start tutorial 
        }

        private void InvokePreferences(object sender, RoutedEventArgs e)
        {
            var preferencesWindow = new Preferences();
            preferencesWindow.Show();
        }

        private void closeApplication(object sender, RoutedEventArgs e)
        {
            var mainViewModel = DataContext as MainViewModel;
            if (mainViewModel?.PerformAutoSave() == true)
            {
                if (App.Current.Windows[1].IsActive)
                {
                    App.Current.Windows[1].Close();
                }

                this.Close();
            }
            else
            {
                this.Close();
                //InvokeWarningPopup("Could not perform autosave", "Could not save your latest changes", ForceClose);
            }
        }

        private ICommand GetDelegateCommand(Action<object> callback)
        {

            var errorCommand = new DelegateCommand(callback);
            return errorCommand;
        }

        private void InvokeWarningPopup(string errorHeadline, string errorDetails, Action<object> callback)
        {
            Warning_Popup.IsOpen = true;
            Error_Head.Content = errorHeadline;
            Error_Desc.Text = errorDetails;

            //Error_Btn.Click += new RoutedEventHandler(callback);

            // TODO: find way to pass functions as Click events generically
        }

        private void minimizeApplication(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void moveWindow(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void InvokeTargetChooser(object sender, RoutedEventArgs e)
        {
            Import_Popup.IsOpen = false;

            CommonOpenFileDialog targetChooser = new CommonOpenFileDialog();
            targetChooser.InitialDirectory = @"C:\users";
            targetChooser.Title = "Choose Import Target";
            targetChooser.IsFolderPicker = true;
            targetChooser.Multiselect = false;

            if (targetChooser.ShowDialog() == CommonFileDialogResult.Ok)
            {
                if(targetChooser.FileName != "Default" && 
                    !targetChooser.FileName.Contains("Windows"))
                {
                    if (Directory.Exists(targetChooser.FileName))
                    {
                        Import_Popup.IsOpen = true;
                        Import_Target.Text = targetChooser.FileName;
                        Import_Confirm_Btn.IsEnabled = true;
                    }
                    else
                    {
                        Import_Target.Text = "Invalid location.";
                    }
                } else
                {
                    Import_Target.Text = "Not able to import from this location.";
                }
            }
        }

        private void InvokeImportPopup(object sender, RoutedEventArgs e)
        { 
            Import_Popup.IsOpen = !Import_Popup.IsOpen;
        }

        private void ConfirmImportSettings(object sender, RoutedEventArgs e)
        {
            if (Directory.Exists(Import_Target.Text))
            {
                Import_Target.Text = Import_Target.Text;
                Import_Confirm_Btn.IsEnabled = true;
                Import_Popup.IsOpen = false;

                Import_Progress_Popup.IsOpen = true;

                //var mainViewModel = DataContext as MainViewModel;

                mainViewModel.Import(Import_Target.Text, HandleListingProgress);

                //Task listImagesTask = ListImages();
                //listImagesTask.ContinueWith(HandleListingProgress);
            }
            else
            {
                Import_Confirm_Btn.IsEnabled = false;
                Import_Target.Text = "Location unreachable, did you delete something?";
            }
        }

        //////////////////////////////////////////////////////////
        //////                    FUNCTIONS                 //////
        //////////////////////////////////////////////////////////

        private void HandleListingProgress(int count, List<Image> imageList)
        {

            Import_Progress_Btn.IsEnabled = true;
            Import_Progress_Count.Text = "Found " + count + " images";

            var timer = new DispatcherTimer();
            TimeSpan timeSpan;

            timeSpan = TimeSpan.FromSeconds(5);
            timer = new DispatcherTimer(new TimeSpan(0, 0, 1), DispatcherPriority.Normal, delegate
            {
                Sorting_Countdown.Text = timeSpan.ToString("c");
                if (timeSpan == TimeSpan.Zero)
                {
                    timer.Stop();
                    Import_Progress_Popup.IsOpen = false;

                    var sortingTask = Task.Run(() =>
                    {
                        mainViewModel.SortImages(imageList, Render);
                    });

                    TaskAwaiter taskAwaiter = sortingTask.GetAwaiter();
                    taskAwaiter.OnCompleted(() =>
                    {
                        // invoke new popup to display success message
                    });
                }
                timeSpan = timeSpan.Add(TimeSpan.FromSeconds(-1));
            }, Application.Current.Dispatcher);
            timer.Start();
        }

        private void RenderComponent(StackPanel sp)
        {
            DirectoryViewer1.Items.Add(sp);
        }


        // alternative to this is an observable list and getting the id from the selectionmodel 
        // but this alt only works properly if elements dont have to be generated manually 

        private void GetImages(object sender, RoutedEventArgs e)
        {
            try
            {
                Button referer = (Button)sender;
                mainViewModel.GetImages(Int16.Parse(referer.Name.Substring(3, referer.Name.Length - 1)));

                // TODO: set screen values to the images of chosen directory
            }
            catch
            {

            }
        }

        private void Render(ImageDirectory directory)
        {
            this.Dispatcher.Invoke(() =>
            {
                Style headlineStyle = this.FindResource("HeadlineTemplate") as Style;
                Style subHeadlineStyle = this.FindResource("SubHeadlineTemplate") as Style;
                Style panelStyle = this.FindResource("PanelTemplate") as Style;

                int width = GetRelativeSize()*402;

                Debug.WriteLine(directory.name);
                directories.Add(directory);

                Label directoryName = new Label();
                directoryName.Content = directory.name;
                directoryName.Style = headlineStyle;
                
                Label directoryImageCount = new Label();
                directoryImageCount.Content = directory.imageList.Count + " Images";
                directoryImageCount.Style = headlineStyle;
                
                Label importDetails = new Label();
                importDetails.Content = directory.timestamp;
                importDetails.Style = subHeadlineStyle;

                DockPanel topDockPanel = new DockPanel();
                topDockPanel.Children.Add(directoryName);
                topDockPanel.Children.Add(directoryImageCount);
                DockPanel.SetDock(directoryName, Dock.Left);
                DockPanel.SetDock(directoryImageCount, Dock.Right);

                DockPanel bottomDockPanel = new DockPanel();
                bottomDockPanel.Children.Add(importDetails);
                DockPanel.SetDock(importDetails, Dock.Left);

                Grid topWrapper = new Grid();
                topWrapper.Children.Add(topDockPanel);

                Grid bottomWrapper = new Grid();
                bottomWrapper.Children.Add(bottomDockPanel);

                DockPanel dockWrapper = new DockPanel();
                dockWrapper.Children.Add(topWrapper);
                dockWrapper.Children.Add(bottomWrapper);
                dockWrapper.Width = width;
                DockPanel.SetDock(topWrapper, Dock.Top);
                DockPanel.SetDock(bottomWrapper, Dock.Bottom);
             
                StackPanel directoryPanel = new StackPanel();
                directoryPanel.Style = panelStyle;
                directoryPanel.Width = width;
                directoryPanel.Height = width*0.2;
                directoryPanel.Children.Add(dockWrapper);

                DirectoryViewer1.Items.Add(directoryPanel);

            });       
        }

        private void GetImagesOfImportedDirectory(int id)
        {
            // TODO: add images of selected directory to observablecollection of images
            //images = mainViewModel.GetImagesOfImportedDirectory(id);
        }

        private void SetScreenSize()
        {
            //set width and height according to system specs
            //maybe also adjust text size
        }

        private int GetRelativeSize()
        {
            return 1;
        }

        public void RenderElement(object obj)
        {
            var type = obj.GetType();
            var newElement = new StackPanel();

            if (type == typeof(ImageDirectory))
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
