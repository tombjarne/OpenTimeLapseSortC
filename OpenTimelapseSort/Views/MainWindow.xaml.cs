using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
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
        private readonly ObservableCollection<SDirectory> _sortedDirectories = new ObservableCollection<SDirectory>();
        private readonly ObservableCollection<SImport> _imports = new ObservableCollection<SImport>();
        private readonly ObservableCollection<SImage> _images = new ObservableCollection<SImage>();

        private readonly MainViewModel _mainViewModel = new MainViewModel();

        private DispatcherTimer _timer = new DispatcherTimer();

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

            if (targetChooser.ShowDialog() == CommonFileDialogResult.Ok &&
                targetChooser.FileName != "Default" &&
                !targetChooser.FileName.Contains("Windows") &&
                Directory.Exists(targetChooser.FileName))
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

        private void HandleListingProgress(int count)
        {
            Import_Progress_Count.Text = "Found " + count + " images";

            var timeSpan = TimeSpan.FromSeconds(9);
            _timer = new DispatcherTimer(new TimeSpan(0, 0, 1), 
                DispatcherPriority.Normal, delegate
            {
                Sorting_Countdown.Text = timeSpan.ToString(@"\ s");
                if (timeSpan == TimeSpan.Zero)
                {
                    _timer.Stop();
                    Import_Progress_Popup.IsOpen = false;

                    _mainViewModel.SortImages(Render);
                }

                timeSpan = timeSpan.Add(TimeSpan.FromSeconds(-1));
            }, Application.Current.Dispatcher);
            _timer.Start();
        }

        private void RenderImages(List<SImage> imageList)
        {
            Dispatcher.Invoke(() =>
            {
                Directory_Name.Content = imageList[0].ParentDirectory.Name;
                Directory_Path.Content = "Path " + imageList[0].ParentDirectory.Target;

                _images.Clear();
                lock (_images)
                {
                    foreach (var image in imageList)
                    {
                        _images.Add(image);
                    }

                    AddImportIfNotExists(imageList[0].ParentDirectory.ParentImport);
                    ImageViewer1.DataContext = _images;
                }
            });
            //GC.Collect();
        }

        private void AddImportIfNotExists(SImport import)
        {
            if (!_imports.Contains(import)) {
                _imports.Insert(0, import);
                Debug.WriteLine(_imports);
            }
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
            Dispatcher.InvokeAsync(() =>
            {
                Debug.WriteLine("Render");
                lock (_directories)
                {
                    foreach (var directory in dirList)
                    {
                        _directories.Insert(0, directory);
                        AddImportIfNotExists(directory.ParentImport);
                    }

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
            // TODO: fix filtration issue - _sortedDirectories when sorting is enabled
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

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            _timer.Stop();
            Import_Progress_Popup.IsOpen = false;
            HideLoader();
        }

        private void SortDirectoriesAfterSelectedDate(object sender, RoutedEventArgs e)
        {
            var calendar = (Calendar) sender;
            var targetDate = calendar.SelectedDate;

            _sortedDirectories.Clear();
            foreach(var import in _imports)
            {
                if (import.Timestamp == targetDate)
                {
                    foreach (var directory in import.Directories)
                    {
                        _sortedDirectories.Insert(0, directory);
                    }
                }
            }

            DirectoryViewer1.DataContext = _sortedDirectories;
        }

        private void ImportDateSortOption(object sender, RoutedEventArgs e)
        {
            DateTakenBackground.Opacity = 100;
            ImportDateBackground.Opacity = 80;

            SortingCalendar.IsEnabled = true;
        }

        private void DateTakenSortOption(object sender, RoutedEventArgs e)
        {
            ImportDateBackground.Opacity = 100;
            DateTakenBackground.Opacity = 80;

            SortingCalendar.IsEnabled = true;
        }
    }
}