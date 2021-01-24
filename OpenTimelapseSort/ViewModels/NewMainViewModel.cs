using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Microsoft.WindowsAPICodePack.Dialogs;
using OpenTimelapseSort.DataServices;
using OpenTimelapseSort.Models;

namespace OpenTimelapseSort.ViewModels
{
    class NewMainViewModel
    {
        private string _errorMessage;
        private string _directoryName;
        private string _importTargetPath;
        private string _importOriginPath;
        private string _directoryPath;
        private string _foundImportImagesCount;
        private Visibility _errorMessageIsVisible;
        private Visibility _importPopupIsVisible;
        private bool _importConfirmationButtonIsEnabled;
        private Visibility _importConfirmationPopupIsVisible;
        private bool _importBtnIsEnabled;
        private Visibility _loaderIsShowing;
        private DateTime? _selectedSortingDate;
        private string _currentImportCountDownSeconds;
        private DispatcherTimer _timer = new DispatcherTimer();
        private readonly CommonOpenFileDialog _fileDialog;

        private readonly DbService _dbService = new DbService();
        private readonly MatchingService _matching = new MatchingService();

        private List<SDirectory> _directories = new List<SDirectory>();
        private readonly List<SImage> _images = new List<SImage>();

        private readonly ObservableCollection<SDirectory> _sortedDirectories = new ObservableCollection<SDirectory>();
        private readonly ObservableCollection<SImport> _imports = new ObservableCollection<SImport>();
        private readonly ObservableCollection<SImage> _selectedImages = new ObservableCollection<SImage>();

        public delegate void ImageListingProgress(int count);
        public delegate void DeletionErrorCallback(string message);
        public delegate void ViewUpdate(List<SDirectory> directories);

        private delegate void IntervalAction(TimeSpan currentSecond);
        private delegate void EndAction();

        public NewMainViewModel()
        {
            _fileDialog = new CommonOpenFileDialog
            {
                InitialDirectory = @"C:\users",
                Title = "Choose Import Target",
                IsFolderPicker = true,
                Multiselect = false
            };
        }

        public SDirectory SelectedDirectory { get; set; }
        public SImage SelectedImage { get; set; }

        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;
                OnPropertyChanged("ErrorMessage");
            }
        }

        public Visibility ErrorMessageVisibility
        {
            get => _errorMessageIsVisible;
            set
            {
                _errorMessageIsVisible = value;
                OnPropertyChanged("ErrorMessageVisibility");
            }
        }

        public Visibility LoaderVisibility
        {
            get => _loaderIsShowing;
            set
            {
                _loaderIsShowing = value;
                OnPropertyChanged("LoaderVisibility");
            }
        }

        public DateTime? SelectedSortingDate
        {
            get => _selectedSortingDate;
            set
            {
                _selectedSortingDate = value;
                OnPropertyChanged("SelectedSortingDate");
            }
        }

        public string DirectoryName
        {
            get => _directoryName;
            set
            {
                _directoryName = value;
                OnPropertyChanged("DirectoryName");
            }
        }

        public Visibility ImportPopupVisibility
        {
            get => _importPopupIsVisible;
            set
            {
                _importPopupIsVisible = value;
                OnPropertyChanged("ImportPopupVisibility");
            }
        }

        public Visibility ImportConfirmationPopupVisibility
        {
            get => _importConfirmationPopupIsVisible;
            set
            {
                _importConfirmationPopupIsVisible = value;
                OnPropertyChanged("ImportConfirmationPopupVisibility");
            }
        }

        public string CurrentImportCountDownTimeSpan
        {
            get => _currentImportCountDownSeconds;
            set
            {
                _currentImportCountDownSeconds = value;
                OnPropertyChanged("CurrentImportCountDownTimeSpan");
            }
        }

        public string ImportTargetPath
        {
            get => _importTargetPath;
            set
            {
                _importTargetPath = value;
                OnPropertyChanged("ImportTargetPath");
            }
        }

        public bool ImportConfirmationButtonIsEnabled
        {
            get => _importConfirmationButtonIsEnabled;
            set
            {
                _importConfirmationButtonIsEnabled = value;
                OnPropertyChanged("ImportConfirmationButtonIsEnabled");
            }
        }

        public string FoundImportImagesCount
        {
            get => _foundImportImagesCount;
            set
            {
                _foundImportImagesCount = value;
                OnPropertyChanged("FoundImportImagesCount");
            }
        }

        public bool ImportBtnIsEnabled
        {
            get => _importBtnIsEnabled;
            set
            {
                _importBtnIsEnabled = value;
                OnPropertyChanged("ImportBtnIsEnabled");
            }
        }

        public string ImportOriginPath
        {
            get => _importOriginPath;
            set
            {
                _importOriginPath = value;
                OnPropertyChanged("ImportOriginPath");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // called from xaml

        private void InvokeTargetChooser(object sender, RoutedEventArgs e)
        {
            ImportPopupVisibility = Visibility.Hidden;
            if (_fileDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                ImportPopupVisibility = Visibility.Visible;
                if (SelectionMatchesRequirements())
                {
                    ImportOriginPath = _fileDialog.FileName;
                    ImportConfirmationButtonIsEnabled = true;
                    HandleListingProgress(_fileDialog.FileNames.Count());
                }
                else
                {
                    HandleError("Invalid location. Choose a different directory.");
                }
            }
        }

        private void HandleError(string errorMessage)
        {
            ErrorMessage = errorMessage;
            ErrorMessageVisibility = Visibility.Visible;
            SetTimer(15, 1, UpdateRemainingErrorTimeNumber, () => ErrorMessageVisibility = Visibility.Hidden);
        }

        private void HandleListingProgress(int count)
        {
            FoundImportImagesCount = "Found " + count + " images";
            SetTimer(9, 1, UpdateImportCountDownNumber, InvokeImportAction);
        }

        private void UpdateImportCountDownNumber(TimeSpan currentSecond)
        {
            CurrentImportCountDownTimeSpan = currentSecond.ToString(@"\ s");
        }

        private void UpdateRemainingErrorTimeNumber(TimeSpan currentSecond)
        {
            //SortingCountdown.Text = currentSecond.ToString(@"\ s");
        }

        private void SetTimer(int timeSpanSeconds, int updateInterval, IntervalAction intervalAction, EndAction endAction)
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

        private void InvokeImportAction()
        {
            _importConfirmationPopupIsVisible = Visibility.Hidden;
            var sortingTask = Task.Run(() => { _directories = _matching.MatchImages(_images); });
            sortingTask.ContinueWith(task =>
            {
                ImportTargetPath = _directories[0].Origin;
                CopyFiles();
            });
        }

        private void CopyFiles()
        {
            var mainDirectory =
                Environment.GetFolderPath(Environment.SpecialFolder.MyPictures) + @"\OTS_IMG";

            if (!Directory.Exists(mainDirectory))
                Directory.CreateDirectory(mainDirectory);

            foreach (var directory in _directories)
            {
                var destination = mainDirectory + @"\" + directory.Name;
                Directory.CreateDirectory(destination);

                foreach (var image in directory.ImageList)
                {
                    var source = Path.Combine(image.Origin);
                    File.Copy(source, destination + @"\" + image.Name, true);
                }
            }

            // TODO: render directories here!
        }

        // called from xaml

        private void ConfirmImportSettings(object sender, RoutedEventArgs e)
        {
            if (Directory.Exists(_importTargetPath))
            {
                ImportBtnIsEnabled = true;
                ImportPopupVisibility = Visibility.Hidden;

                ImportConfirmationPopupVisibility = Visibility.Visible;
                LoaderVisibility = Visibility.Visible;
                Import(ImportOriginPath);
            }
            else
            {
                ImportBtnIsEnabled = false;
                HandleError("Location unreachable, did you delete something?");
            }
        }

        public void Import(string name)
        {
            _images.Clear();

            var files = Directory.EnumerateFileSystemEntries(name).ToList();
            var length = files.Count();

            if (length > 0)
            {
                for (var i = 0; i < length; i++)
                {
                    var file = files[i];

                    if (Directory.Exists(file))
                    {
                        var subDirImages = Directory.GetFiles(file);
                        var subDirInfo = new FileInfo(subDirImages[i]);
                        var subDirLength = Directory.EnumerateFiles(file).ToList().Count();

                        for (var p = 0; p < subDirLength; p++)
                        {
                            var subDirFile = subDirImages[p];
                            _images.Add(CreateImage(subDirFile, subDirInfo));
                        }
                    }
                    else
                    {
                        var info = new FileInfo(file);
                        _images.Add(CreateImage(file, info));
                    }
                }

                FoundImportImagesCount = _images.Count.ToString();
            }
        }

        private static SImage CreateImage(string file, FileInfo info)
        {
            var image = new SImage
            (
                Path.GetFileName(file),
                info.FullName,
                info.DirectoryName
            )
            {
                Id = Guid.NewGuid().ToString(),
                FileSize = info.Length / 1000
            };

            return image;
        }

        private bool SelectionMatchesRequirements()
        {
            return _fileDialog.FileName != "Default" &&
                   !_fileDialog.FileName.Contains("Windows") &&
                   Directory.Exists(_fileDialog.FileName);
        }

        // handles adding of imports to _imports list

        private void AddImportIfNotExists(SImport import)
        {
            if (!_imports.Contains(import))
            {
                _imports.Insert(0, import);
            }
        }

        // called from xaml

        public async void SetDirectoryName(object sender, RoutedEventArgs e)
        {
            // TODO: bind to SelectedItem property in XAML
            var directory = SelectedDirectory;
            await _dbService.UpdateDirectoryAsync(directory);
        }

        public async Task DeleteImage(object sender, RoutedEventArgs e)
        {
            var image = SelectedImage;

            if (image.Delete())
            {
                var directory = image.ParentDirectory;
                directory.ImageList.Remove(image);

                if (directory.ImageList.Count == 0)
                    await DeleteImageDirectory(directory);

                HandleError("Image was deleted successfully.");
            }
            HandleError("Image could not be deleted.");
        }

        public async Task<List<SDirectory>> GetDirectoriesAsync()
        {
            return await _dbService.GetDirectoriesAsync();
        }

        // called from xaml
        public async Task DeleteImageDirectoryHandler(object sender, RoutedEventArgs e)
        {
            var directory = SelectedDirectory;
            await DeleteImageDirectory(directory);
        }

        public async Task DeleteImageDirectory(SDirectory directory)
        {
            if (directory.Delete())
            {
                var import = directory.ParentImport;
                import.Directories.Remove(directory);

                if (import.Directories.Count == 0)
                    await _dbService.DeleteImportAsync(import.Id);
                else
                    await _dbService.UpdateImportAsync(import);

                HandleError("Directory was deleted successfully.");
            }
            HandleError("Directory could not be deleted.");
        }

        public async Task UpdateImageDirectory(SDirectory directory)
        {
            await _dbService.UpdateDirectoryAsync(directory);
        }

        // handles update of list after name change of directory
        /*
        private SDirectory GetAndUpdateDirectory(string name)
        {
            
            var directory = _directories[DirectoryViewer1.SelectedIndex];
            directory.Name = DirectoryName.Text;

            DirectoryViewer1.Items.Refresh();

            return directory;
            
        }
        */
        

        // renders a list of stack panels of SDirectories 

        private void Render(List<SDirectory> dirList)
        {
            Task.Run(()=>
            {
                lock (_directories)
                {
                    foreach (var directory in dirList)
                    {
                        _directories.Insert(0, directory);
                        AddImportIfNotExists(directory.ParentImport);
                    }

                    AddImportIfNotExists(dirList[0].ParentImport);
                    LoaderVisibility = Visibility.Hidden;
                }
            });
        }

        private void RenderImages(List<SImage> imageList)
        {
            Task.Run(() =>
            {
                DirectoryName = imageList[0].ParentDirectory.Name;
                //DirectoryPath = "Path " + imageList[0].ParentDirectory.Target;

                _images.Clear();
                lock (_images)
                {
                    foreach (var image in imageList) _images.Add(image);
                }
            });
        }

        // Bind to SelectedDates

        private void SortDirectoriesAfterSelectedDate(object sender, RoutedEventArgs e)
        {
            var calendar = (Calendar)sender;
            var targetDate = calendar.SelectedDate;

            _sortedDirectories.Clear();
            foreach (var import in _imports)
                if (import.Timestamp == targetDate)
                    foreach (var directory in import.Directories)
                        _sortedDirectories.Insert(0, directory);

            SelectedSortingDate = targetDate;
        }

        // called from XAML
        
        private void CancelSortAfterDate(object sender, MouseButtonEventArgs e)
        {
            SelectedSortingDate = null;
        }
    }
}
