using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Microsoft.WindowsAPICodePack.Dialogs;
using OpenTimelapseSort.ViewModels;

namespace OpenTimelapseSort.Views
{
    public partial class MainWindow : Window
    {
        //////////////////////////////////////////////////////////
        //////                    VARIABLES                 //////
        //////////////////////////////////////////////////////////

        //private delegate void WarningReference(string errorHeadline, string errorDetails);

        private readonly ObservableCollection<SDirectory> _directories = new ObservableCollection<SDirectory>();
        private readonly ObservableCollection<SImport> _imports = new ObservableCollection<SImport>();
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

        private void CloseApplication(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void InvokeWarningPopup(string errorHeadline, string errorDetails, Action<object> callback)
        {
            Warning_Popup.IsOpen = true;
            Error_Head.Content = errorHeadline;
            Error_Desc.Text = errorDetails;

            //Error_Btn.Click += new RoutedEventHandler(callback);

            // TODO: find way to pass functions as Click events generically
        }

        private void MinimizeApplication(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void MoveWindow(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void InvokeTargetChooser(object sender, RoutedEventArgs e)
        {
            Import_Popup.IsOpen = false;

            var targetChooser = new CommonOpenFileDialog
            {
                InitialDirectory = @"C:\users",
                Title = "Choose Import Target",
                IsFolderPicker = true,
                Multiselect = false
            };

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

                    var sortingTask = Task.Run(() => { _mainViewModel.SortImages(imageList, Render); });

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

        private void RenderImages(List<SImage> imageList)
        {
            Dispatcher.Invoke(() =>
            {
                _images.Clear();
                lock (_images)
                {
                    foreach (var image in imageList) _images.Add(image);

                    AddImportIfNotExists(imageList[0].ParentDirectory.ParentImport);
                    ImageViewer1.DataContext = _images;
                }
            });
        }

        private void AddImportIfNotExists(SImport import)
        {
            if (!_imports.Contains(import)) _imports.Insert(0, import);
        }

        /**
         * Render
         * 
         * Renders the imported _directories into the view component
         * @param dirList, type List
         * <ImageDirectory>
         */
        private void Render(List<SDirectory> dirList)
        {
            Dispatcher.Invoke(() =>
            {
                lock (_directories)
                {
                    foreach (var directory in dirList) _directories.Insert(0, directory);

                    AddImportIfNotExists(dirList[0].ParentImport);
                    DirectoryViewer1.DataContext = _directories;

                    HideLoader();
                }
            });
        }

        private void ShowLoader()
        {
            Loader.Visibility = Visibility.Visible;
        }

        private void HideLoader()
        {
            Loader.Visibility = Visibility.Hidden;
        }

        private void DirectoryViewer1_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var imageList = _directories[DirectoryViewer1.SelectedIndex].ImageList;
            RenderImages(imageList);
        }

        private void ImageViewer_OnPreviewMouseDown(object sender, MouseWheelEventArgs e)
        {
            var scv = (ScrollViewer) sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
        }

        private void DirectoryViewer_OnPreviewMouseDown(object sender, MouseWheelEventArgs e)
        {
            var scv = (ScrollViewer) sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
        }
    }
}