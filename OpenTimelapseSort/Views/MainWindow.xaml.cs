using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.IO;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Threading;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using FontAwesome.WPF;

namespace OpenTimelapseSort.Views
{
    public partial class MainWindow : Window
    {

        //////////////////////////////////////////////////////////
        //////                    VARIABLES                 //////
        //////////////////////////////////////////////////////////

        //private delegate void WarningReference(string errorHeadline, string errorDetails);

        ObservableCollection<StackPanel> panels = new ObservableCollection<StackPanel>();
        ObservableCollection<SDirectory> directories = new ObservableCollection<SDirectory>();
        ObservableCollection<SImport> imports = new ObservableCollection<SImport>();
        ObservableCollection<SImage> images = new ObservableCollection<SImage>();

        MainViewModel mainViewModel = new MainViewModel();


        //////////////////////////////////////////////////////////
        //////                   CONSTRUCTOR                //////
        //////////////////////////////////////////////////////////

        public MainWindow()
        {
            InitializeComponent();
            SetScreenSize();
            FetchOnStartup();
        }

        //////////////////////////////////////////////////////////
        //////                  XAMLFUNCTIONS               //////
        //////////////////////////////////////////////////////////

        private async void FetchOnStartup()
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
            var preferencesWindow = new PreferencesView();
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

                // TODO: fix below statement
                //InvokeWarningPopup("Could not perform autosave", "Could not save your latest changes", ForceClose);
            }
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

                ShowLoader();
                mainViewModel.Import(Import_Target.Text, HandleListingProgress);
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

        private void HandleListingProgress(int count, List<SImage> imageList)
        {

            Import_Progress_Count.Text = "Found " + count + " images";

            var timer = new DispatcherTimer();
            TimeSpan timeSpan;

            timeSpan = TimeSpan.FromSeconds(9);
            timer = new DispatcherTimer(new TimeSpan(0, 0, 1), DispatcherPriority.Normal, delegate
            {
                Sorting_Countdown.Text = timeSpan.ToString(@"\ s");
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
                        //
                    });
                }
                timeSpan = timeSpan.Add(TimeSpan.FromSeconds(-1));
            }, Application.Current.Dispatcher);
            timer.Start();
        }


        // alternative to this is an observable list and getting the id from the selectionmodel 
        // but this alt only works properly if elements dont have to be generated manually 

        private async Task GetImagesAsync(int id)
        {
            try
            {
                var imageList = await mainViewModel.GetImagesAsync(id);

                Style headlineStyle = this.FindResource("HeadlineTemplate") as Style;
                Style subHeadlineStyle = this.FindResource("SubHeadlineTemplate") as Style;
                Style panelStyle = this.FindResource("PanelTemplate") as Style;

                int width = GetRelativeSize() * 402;
                directory_headline1.Content = imageList[0].parentInstance;
                directory_name1_Copy.Content = imageList[0].target;

                foreach (var image in imageList)
                {
                    images.Add(image);

                    Label imageName = new Label();
                    imageName.Content = image.name;
                    imageName.Style = headlineStyle;

                    Label imageSize = new Label();
                    imageSize.Content = image.fileSize;
                    imageName.Style = subHeadlineStyle;

                    Grid detailGrid = new Grid();
                    detailGrid.Children.Add(imageName);
                    detailGrid.Children.Add(imageSize);

                    //Image previewImage = new Image();
                    //previewImage.Source = new Uri(image.target);

                    DockPanel dockWrapper = new DockPanel();
                    dockWrapper.Width = width;
                    dockWrapper.Children.Add(detailGrid);
                    DockPanel.SetDock(detailGrid, Dock.Top);
                    //DockPanel.SetDock(previewImage, Dock.Bottom);

                    StackPanel directoryPanel = new StackPanel();
                    directoryPanel.Name = "E" + image.id.ToString();
                    directoryPanel.Style = panelStyle;
                    directoryPanel.Width = width;
                    directoryPanel.Height = width * 0.2;
                    directoryPanel.Children.Add(dockWrapper);

                    ImageViewer1.Items.Add(directoryPanel);
                }
            }
            catch
            {

            }
        }

        /**
         * Render
         * 
         * Renders the imported directories into the view component
         * @param dirList, type List<ImageDirectory>
         */

        private void Render(List<SDirectory> dirList)
        {
            this.Dispatcher.Invoke(() =>
            {
                lock (directories)
                {
                    Style headlineStyle = this.FindResource("HeadlineTemplate") as Style;
                    Style subHeadlineStyle = this.FindResource("SubHeadlineTemplate") as Style;
                    Style panelStyle = this.FindResource("PanelTemplate") as Style;

                    int width = GetRelativeSize() * 402;

                    foreach (var directory in dirList)
                    {
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
                        directoryPanel.Name = "E" + directory.id.ToString();
                        directoryPanel.Style = panelStyle;
                        directoryPanel.Width = width;
                        directoryPanel.Height = width * 0.2;
                        directoryPanel.Children.Add(dockWrapper);

                        DirectoryViewer1.Items.Add(directoryPanel);
                        panels.Add(directoryPanel);
                        HideLoader();
                    }
                }
            });       
        }

        private void ShowLoader()
        {
            Style panelStyle = this.FindResource("PanelTemplate") as Style;

            int width = GetRelativeSize() * 402;

            ImageAwesome loadingIcon = new ImageAwesome()
            {
                Icon = FontAwesomeIcon.CircleOutlineNotch,
                Spin = true,
                SpinDuration = 2,
                Width = 50.0,
                Height = 50.0,
                Margin = new Thickness(20),
                Foreground = Brushes.White,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            StackPanel directoryPanel = new StackPanel()
            {
                Name = "DirectoryLoader",
                Style = panelStyle,
                Width = width,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };

            directoryPanel.Children.Add(loadingIcon);

            DirectoryViewer1.Items.Add(directoryPanel);
            panels.Add(directoryPanel);
        }

        private void HideLoader()
        {
            DirectoryViewer1.Items.Remove(panels.Single(p => p.Name == "DirectoryLoader"));
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

        private void DirectoryViewer1_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            int directoryId = directories[DirectoryViewer1.SelectedIndex].id;
            var fetchImagesTask = GetImagesAsync(directoryId);
        }
    }
}
