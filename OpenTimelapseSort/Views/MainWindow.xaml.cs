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
using OpenTimelapseSort.Models;
using OpenTimelapseSort.ViewModels;

namespace OpenTimelapseSort.Views
{
    public partial class MainWindow
    {
        //////////////////////////////////////////////////////////
        //////                    VARIABLES                 //////
        //////////////////////////////////////////////////////////

        private readonly ObservableCollection<SDirectory> _directories = new ObservableCollection<SDirectory>();
        private readonly ObservableCollection<SDirectory> _sortedDirectories = new ObservableCollection<SDirectory>();
        private readonly ObservableCollection<SImport> _imports = new ObservableCollection<SImport>();
        private readonly ObservableCollection<SImage> _images = new ObservableCollection<SImage>();

        private readonly MainViewModel _mainViewModel = new MainViewModel();

        private delegate void IntervalAction(TimeSpan currentSecond);

        private delegate void EndAction();

        public string Error = "";

        private DispatcherTimer _timer = new DispatcherTimer();
        private readonly CommonOpenFileDialog _fileDialog;

        //////////////////////////////////////////////////////////
        //////                   CONSTRUCTOR                //////
        //////////////////////////////////////////////////////////

        public MainWindow()
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            InvokeStartupScreen();
            InitializeComponent();
            _ = FetchOnStartupAsync();

            _fileDialog = new CommonOpenFileDialog
            {
                InitialDirectory = @"C:\users",
                Title = "Choose Import Target",
                IsFolderPicker = true,
                Multiselect = false
            };
        }

        //////////////////////////////////////////////////////////
        //////                  XAMLFUNCTIONS               //////
        //////////////////////////////////////////////////////////

        private void InvokeStartupScreen()
        {
            var startupScreen = new StartupScreen();
            startupScreen.Show();
        }

        // handles fetching of directories on load

        private async Task FetchOnStartupAsync()
        {
            Render(await _mainViewModel.GetDirectoriesAsync());
        }

        // handles invocation of preferences window

        private void InvokePreferences(object sender, RoutedEventArgs e)
        {
            var preferencesWindow = new PreferencesView();
            preferencesWindow.Show();
        }

        // handles closing of application

        private void CloseApplication(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        // handles deletion of an image

        public async void DeleteImage(object sender, RoutedEventArgs e)
        {
            var image = _images[ImageViewer1.SelectedIndex];
            await _mainViewModel.DeleteImage(image, InvokeWarningPopup);
        }

        // handles deletion of a directory

        public async void DeleteDirectory(object sender, RoutedEventArgs e)
        {
            /*
            var directory = _directories[DirectoryViewer1.SelectedIndex];
            await _mainViewModel.DeleteImageDirectory(directory, InvokeWarningPopup);
            */
        }

        // handles invocation of error messages

        private void InvokeWarningPopup(string errorDetails)
        {
            ChangeErrorMessageVisibility();
            ErrorMessage.Text = errorDetails;
            SetTimer(15, 1, UpdateRemainingErrorTimeNumber, ChangeErrorMessageVisibility);
        }

        private void ChangeErrorMessageVisibility()
        {
            Warning.Visibility = Warning.Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
        }

        // invokes a new popup and counts down to import

        private void HandleListingProgress(int count)
        {
            ImportProgressCount.Text = "Found " + count + " images";
            SetTimer(9, 1, UpdateImportCountDownNumber, InvokeImportAction);
        }

        // TODO: make it async!

        private void InvokeImportAction()
        {
            ImportProgressPopup.IsOpen = false;
            _mainViewModel.SortImages(Render);
        }

        private void UpdateImportCountDownNumber(TimeSpan currentSecond)
        {
            SortingCountdown.Text = currentSecond.ToString(@"\ s");
        }

        private void UpdateRemainingErrorTimeNumber(TimeSpan currentSecond)
        {
            //SortingCountdown.Text = currentSecond.ToString(@"\ s");
        }

        // handles generic approach to using dispatcher

        private void SetTimer(int timeSpanSeconds, int updateInterval, IntervalAction intervalAction,
            EndAction endAction)
        {
            var timeSpan = TimeSpan.FromSeconds(timeSpanSeconds);
            _timer = new DispatcherTimer(new TimeSpan(0, 0, updateInterval),
                DispatcherPriority.Normal, delegate
                {
                    intervalAction(timeSpan);
                    if (timeSpan == TimeSpan.Zero)
                    {
                        _timer.Stop();
                        endAction();
                    }

                    timeSpan = timeSpan.Add(TimeSpan.FromSeconds(-1));
                }, Application.Current.Dispatcher);
            _timer.Start();
        }

        // handles minimization of window

        private void MinimizeApplication(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        // handles drag of window

        private void MoveWindow(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        // handles invocation of target chooser

        private void InvokeTargetChooser(object sender, RoutedEventArgs e)
        {
            ImportPopup.IsOpen = false;

            if (_fileDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                ImportPopup.IsOpen = true;
                if (SelectionMatchesRequirements())
                {
                    ImportTarget.Text = _fileDialog.FileName;
                    ImportConfirmBtn.IsEnabled = true;
                }
                else
                {
                    const string errorMessage = "Invalid location. Choose a different directory.";
                    InvokeWarningPopup(errorMessage);
                }
            }
        }

        private bool SelectionMatchesRequirements()
        {
            return _fileDialog.FileName != "Default" &&
                   !_fileDialog.FileName.Contains("Windows") &&
                   Directory.Exists(_fileDialog.FileName);
        }

        // handles visibility of import popup

        private void ChangeImportPopupVisibility(object sender, RoutedEventArgs e)
        {
            ImportPopup.IsOpen = !ImportPopup.IsOpen;
        }

        // handles import

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

        // renders list of images after selected directory

        private void RenderImages(List<SImage> imageList)
        {
            Dispatcher.Invoke(() =>
            {
                DirectoryName.Text = imageList[0].ParentDirectory.Name;
                DirectoryName.IsReadOnly = false;
                DirectoryPath.Content = "Path " + imageList[0].ParentDirectory.Target;

                _images.Clear();
                lock (_images)
                {
                    foreach (var image in imageList) _images.Add(image);

                    ImageViewer1.DataContext = _images;
                }
            });
        }

        // handles adding of imports to _imports list

        private void AddImportIfNotExists(SImport import)
        {
            if (!_imports.Contains(import))
            {
                _imports.Insert(0, import);
            }
        }

        // handles setting of directory name

        public async void SetDirectoryName(object sender, RoutedEventArgs e)
        {
            var directory = GetAndUpdateDirectory(DirectoryName.Text);

            await _mainViewModel.UpdateImageDirectory(directory);
        }

        // handles update of list after name change of directory

        private SDirectory GetAndUpdateDirectory(string name)
        {
            var directory = _directories[DirectoryViewer1.SelectedIndex];
            directory.Name = DirectoryName.Text;

            DirectoryViewer1.Items.Refresh();

            return directory;
        }

        // renders a list of stack panels of SDirectories 

        private void Render(List<SDirectory> dirList)
        {
            Dispatcher.InvokeAsync(() =>
            {
                lock (_directories)
                {
                    foreach (var directory in dirList)
                    {
                        _directories.Insert(0, directory);
                        AddImportIfNotExists(directory.ParentImport);
                    }

                    DirectoryViewer1.DataContext = _directories;
                    AddImportIfNotExists(dirList[0].ParentImport);

                    HideLoader();
                }
            });
        }

        // shows spinning loading icon

        private void ShowLoader()
        {
            Loader.Visibility = Visibility.Visible;
        }

        // hides spinning loading icon

        private void HideLoader()
        {
            Loader.Visibility = Visibility.Hidden;
        }

        // handles invocation of imagelist

        private void DirectoryViewer1_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var imageList = SortingCalendar.SelectedDate == null
                ? _directories[DirectoryViewer1.SelectedIndex].ImageList
                : _sortedDirectories[DirectoryViewer1.SelectedIndex].ImageList;
            RenderImages(imageList);
        }

        // handles scrolling

        private void ImageViewer_OnPreviewMouseDown(object sender, MouseWheelEventArgs e)
        {
            var scv = (ScrollViewer) sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
        }

        // handles scrolling

        private void DirectoryViewer_OnPreviewMouseDown(object sender, MouseWheelEventArgs e)
        {
            var scv = (ScrollViewer) sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
        }

        // handles abortion of new import

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            _timer.Stop();
            ImportProgressPopup.IsOpen = false;
            HideLoader();
        }

        // handles sort after selected calendar date

        private void SortDirectoriesAfterSelectedDate(object sender, RoutedEventArgs e)
        {
            var calendar = (Calendar) sender;
            var targetDate = calendar.SelectedDate;

            _sortedDirectories.Clear();
            foreach (var import in _imports)
                if (import.Timestamp == targetDate)
                    foreach (var directory in import.Directories)
                        _sortedDirectories.Insert(0, directory);

            DirectoryViewer1.DataContext = _sortedDirectories;
        }

        // handles abortion of sort after selected calendar date
        private void CancelSortAfterDate(object sender, MouseButtonEventArgs e)
        {
            SortingCalendar.SelectedDate = null;
            DirectoryViewer1.DataContext = _directories;
        }
    }
}