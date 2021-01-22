using Microsoft.WindowsAPICodePack.Dialogs;
using OpenTimelapseSort.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace OpenTimelapseSort.Views
{
    public partial class MainWindow : Window
    {
        //////////////////////////////////////////////////////////
        //////                    VARIABLES                 //////
        //////////////////////////////////////////////////////////

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

        private void InvokeWarningPopup(string errorDetails)
        {
            Warning.Visibility = Visibility.Visible;
            ErrorMessage.Text = errorDetails;

            var timeSpan = TimeSpan.FromSeconds(15);
            _timer = new DispatcherTimer(new TimeSpan(0, 0, 1),
                DispatcherPriority.Normal, delegate
                {
                    if (timeSpan == TimeSpan.Zero)
                    {
                        _timer.Stop();
                        Warning.Visibility = Visibility.Hidden;
                    }
                    timeSpan = timeSpan.Add(TimeSpan.FromSeconds(-1));
                }, Application.Current.Dispatcher);
            _timer.Start();
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
            ImportPopup.IsOpen = false;

            var targetChooser = new CommonOpenFileDialog
            {
                InitialDirectory = @"C:\users",
                Title = "Choose Import Target",
                IsFolderPicker = true,
                Multiselect = false
            };

            if (targetChooser.ShowDialog() == CommonFileDialogResult.Ok)
            {
                ImportPopup.IsOpen = true;
                if (
                    targetChooser.FileName != "Default" &&
                    !targetChooser.FileName.Contains("Windows") &&
                    Directory.Exists(targetChooser.FileName))
                {
                    ImportTarget.Text = targetChooser.FileName;
                    ImportConfirmBtn.IsEnabled = true;
                }
                else
                {
                    const string errorMessage = "Invalid location. Choose a different directory.";
                    ImportTarget.Text = errorMessage;
                    InvokeWarningPopup(errorMessage);
                }
            }
        }

        private void InvokeImportPopup(object sender, RoutedEventArgs e)
        {
            ImportPopup.IsOpen = !ImportPopup.IsOpen;
        }

        private void ConfirmImportSettings(object sender, RoutedEventArgs e)
        {
            if (Directory.Exists(ImportTarget.Text))
            {
                ImportTarget.Text = ImportTarget.Text;
                ImportConfirmBtn.IsEnabled = true;
                ImportPopup.IsOpen = false;

                ImportProgressPopup.IsOpen = true;

                ShowLoader();
                _mainViewModel.Import(ImportTarget.Text, HandleListingProgress);
            }
            else
            {
                const string errorMessage = "Location unreachable, did you delete something?";
                ImportConfirmBtn.IsEnabled = false;
                ImportTarget.Text = errorMessage;

                InvokeWarningPopup(errorMessage);
            }
        }

        //////////////////////////////////////////////////////////
        //////                    FUNCTIONS                 //////
        //////////////////////////////////////////////////////////

        private void HandleListingProgress(int count)
        {
            ImportProgressCount.Text = "Found " + count + " images";

            var timeSpan = TimeSpan.FromSeconds(9);
            _timer = new DispatcherTimer(new TimeSpan(0, 0, 1),
                DispatcherPriority.Normal, delegate
            {
                SortingCountdown.Text = timeSpan.ToString(@"\ s");
                if (timeSpan == TimeSpan.Zero)
                {
                    _timer.Stop();
                    ImportProgressPopup.IsOpen = false;

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
                DirectoryName.Content = imageList[0].ParentDirectory.Name;
                DirectoryPath.Content = "Path " + imageList[0].ParentDirectory.Target;

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
        }

        private void AddImportIfNotExists(SImport import)
        {
            if (!_imports.Contains(import))
            {
                _imports.Insert(0, import);
                Debug.WriteLine(_imports);
            }
        }

        /**
         * Render
         * 
         * Renders the imported _directories into the view component
         * @param dirList, type List
         * <ImageDirectory/>
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
            var imageList = SortingCalendar.SelectedDate == null ?
            _directories[DirectoryViewer1.SelectedIndex].ImageList :
            _sortedDirectories[DirectoryViewer1.SelectedIndex].ImageList;
            RenderImages(imageList);
        }

        private void ImageViewer_OnPreviewMouseDown(object sender, MouseWheelEventArgs e)
        {
            var scv = (ScrollViewer)sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
        }

        private void DirectoryViewer_OnPreviewMouseDown(object sender, MouseWheelEventArgs e)
        {
            var scv = (ScrollViewer)sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            _timer.Stop();
            ImportProgressPopup.IsOpen = false;
            HideLoader();
        }

        private void SortDirectoriesAfterSelectedDate(object sender, RoutedEventArgs e)
        {
            var calendar = (Calendar)sender;
            var targetDate = calendar.SelectedDate;

            _sortedDirectories.Clear();
            foreach (var import in _imports)
                if (import.Timestamp == targetDate)
                    foreach (var directory in import.Directories)
                        _sortedDirectories.Insert(0, directory);

            DirectoryViewer1.DataContext = _sortedDirectories;
        }

        private void CancelSortAfterDate(object sender, MouseButtonEventArgs e)
        {
            SortingCalendar.SelectedDate = null;
            DirectoryViewer1.DataContext = _directories;
        }
    }
}