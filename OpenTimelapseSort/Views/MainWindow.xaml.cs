using FontAwesome.WPF;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace OpenTimelapseSort.Views
{
    public partial class MainWindow : Window
    {

        //////////////////////////////////////////////////////////
        //////                    VARIABLES                 //////
        //////////////////////////////////////////////////////////

        //private delegate void WarningReference(string errorHeadline, string errorDetails);

        private readonly ObservableCollection<StackPanel> _panels = new ObservableCollection<StackPanel>();
        private readonly ObservableCollection<SDirectory> _directories = new ObservableCollection<SDirectory>();
        private readonly ObservableCollection<SImage> _images = new ObservableCollection<SImage>();

        private readonly MainViewModel _mainViewModel = new MainViewModel();

        //////////////////////////////////////////////////////////
        //////                   CONSTRUCTOR                //////
        //////////////////////////////////////////////////////////

        public MainWindow()
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            var startupScreen = new StartupScreen();
            startupScreen.Show();

            InitializeComponent();
            _ = FetchOnStartupAsync();
        }

        //////////////////////////////////////////////////////////
        //////                  XAMLFUNCTIONS               //////
        //////////////////////////////////////////////////////////

        private async Task FetchOnStartupAsync()
        {
            Render(await GetDirectoriesAsync());
            SetScreenSize();
        }

        private async Task<List<SDirectory>> GetDirectoriesAsync()
        {
            return await _mainViewModel.GetDirectoriesAsync();
        }

        private void InvokePreferences(object sender, RoutedEventArgs e)
        {
            var preferencesWindow = new PreferencesView();
            preferencesWindow.Show();
        }

        private void closeApplication(object sender, RoutedEventArgs e)
        {
            var mainViewModel = DataContext as MainViewModel;
            var windows = App.Current.Windows;
            if (mainViewModel?.PerformAutoSave() == true)
            {
                if (windows[2].IsActive)
                {
                    windows[2].Close();
                }

                windows[1].Close();
                this.Close();
            }
            else
            {
                windows[1].Close();
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

        private void MoveWindow(object sender, MouseButtonEventArgs e)
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
                if (targetChooser.FileName != "Default" &&
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
                }
                else
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
                _mainViewModel.Import(Import_Target.Text, HandleListingProgress);
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
                        _mainViewModel.SortImages(imageList, Render);
                    });

                    var taskAwaiter = sortingTask.GetAwaiter();
                    taskAwaiter.OnCompleted(() =>
                    {
                        //
                    });
                }
                timeSpan = timeSpan.Add(TimeSpan.FromSeconds(-1));
            }, Application.Current.Dispatcher);
            timer.Start();
        }


        // alternative to this is an observable list and getting the Id from the selectionmodel 
        // but this alt only works properly if elements dont have to be generated manually 

        private async Task RenderImages(List<SImage> imageList)
        {

            _images.Clear();
            ImageViewer1.Items.Clear();

            var headlineStyle = this.FindResource("HeadlineTemplate") as Style;
            var subHeadlineStyle = this.FindResource("SubHeadlineTemplate") as Style;
            var panelStyle = this.FindResource("PanelTemplate") as Style;
            var width = GetRelativeSize() * 402;

            directory_headline1.Content = imageList[0].ParentInstance;
            directory_name1_Copy.Content = imageList[0].Target;

            foreach (var image in imageList)
            {
                _images.Add(image);

                var imageName = new Label
                {
                    Content = image.Name,
                    Style = headlineStyle
                };

                var imageSize = new Label
                {
                    Content = image.FileSize
                };
                imageName.Style = subHeadlineStyle;

                var detailGrid = new Grid();
                detailGrid.Children.Add(imageName);
                detailGrid.Children.Add(imageSize);

                //Image previewImage = new Image();
                //previewImage.Source = new Uri(image.Target);

                var dockWrapper = new DockPanel
                {
                    Width = width
                };
                dockWrapper.Children.Add(detailGrid);
                DockPanel.SetDock(detailGrid, Dock.Top);
                //DockPanel.SetDock(previewImage, Dock.Bottom);

                var directoryPanel = new StackPanel
                {
                    Style = panelStyle,
                    Width = ImageViewer.Width
                };
                directoryPanel.Children.Add(dockWrapper);

                ImageViewer1.Items.Add(directoryPanel);
            }
        }

        /**
         * Render
         * 
         * Renders the imported _directories into the view component
         * @param dirList, type List<ImageDirectory>
         */

        private void Render(List<SDirectory> dirList)
        {

            this.Dispatcher.Invoke(() =>
            {
                lock (_directories)
                {
                    var headlineStyle = this.FindResource("HeadlineTemplate") as Style;
                    var subHeadlineStyle = this.FindResource("SubHeadlineTemplate") as Style;
                    var panelStyle = this.FindResource("PanelTemplate") as Style;
                    var width = GetRelativeSize() * 402;

                    foreach (var directory in dirList)
                    {
                        _directories.Add(directory);

                        var name = directory.Name.Length <= 20 ?
                            directory.Name : directory.Name.Substring(directory.Name.Length - 20);

                        var directoryName = new Label
                        {
                            Content = name,
                            Style = headlineStyle
                        };

                        var directoryImageCount = new Label
                        {
                            Content = directory.ImageList.Count + " Images",
                            Style = headlineStyle
                        };

                        var importDetails = new Label
                        {
                            Content = directory.Timestamp,
                            Style = subHeadlineStyle
                        };

                        var topDockPanel = new DockPanel();
                        topDockPanel.Children.Add(directoryName);
                        topDockPanel.Children.Add(directoryImageCount);
                        DockPanel.SetDock(directoryName, Dock.Left);
                        DockPanel.SetDock(directoryImageCount, Dock.Right);

                        var bottomDockPanel = new DockPanel();
                        bottomDockPanel.Children.Add(importDetails);
                        DockPanel.SetDock(importDetails, Dock.Left);

                        var topWrapper = new Grid();
                        topWrapper.Children.Add(topDockPanel);

                        var bottomWrapper = new Grid();
                        bottomWrapper.Children.Add(bottomDockPanel);

                        var dockWrapper = new DockPanel();
                        dockWrapper.Children.Add(topWrapper);
                        dockWrapper.Children.Add(bottomWrapper);
                        dockWrapper.Width = width;
                        DockPanel.SetDock(topWrapper, Dock.Top);
                        DockPanel.SetDock(bottomWrapper, Dock.Bottom);

                        var directoryPanel = new StackPanel
                        {
                            Style = panelStyle,
                            Width = width,
                            Height = width * 0.2
                        };
                        directoryPanel.Children.Add(dockWrapper);

                        DirectoryViewer1.Items.Add(directoryPanel);
                        _panels.Add(directoryPanel);

                    }
                    HideLoader();
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
            _panels.Add(directoryPanel);
        }

        private void HideLoader()
        {
            DirectoryViewer1.Items.Remove(_panels.Single(p => p.Name == "DirectoryLoader"));
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
            var imageList = _directories[DirectoryViewer1.SelectedIndex].ImageList;
            var fetchImagesTask = RenderImages(imageList);
        }

        private void ImageViewer_OnPreviewMouseDown(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scv = (ScrollViewer)sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
        }
    }
}
