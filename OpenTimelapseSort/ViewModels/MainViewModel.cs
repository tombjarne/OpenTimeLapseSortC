using OpenTimelapseSort.Contexts;
using OpenTimelapseSort.DataServices;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using Microsoft.WindowsAPICodePack.Dialogs;
using OpenTimelapseSort.Models;
using OpenTimelapseSort.Mvvm;

namespace OpenTimelapseSort.ViewModels
{
    internal class MainViewModel : INotifyPropertyChanged
    {
        private string _errorMessage;
        private string _directoryName;
        private string _importTargetPath;
        private string _importOriginPath;
        private string _directoryPath;
        private string _foundImportImagesCount;
        private Visibility _errorMessageIsVisible;
        private bool _importPopupIsOpen;
        private bool _importConfirmationButtonIsEnabled;
        private bool _importConfirmationPopupIsVisible;
        private bool _importBtnIsEnabled;
        private Visibility _loaderIsShowing;
        private DateTime? _selectedSortingDate;
        private string _currentImportCountDownSeconds;
        private DispatcherTimer _timer = new DispatcherTimer();
        private readonly CommonOpenFileDialog _fileDialog;
        private SDirectory _selectedDirectory;

        private readonly DbService _dbService = new DbService();
        private readonly MatchingService _matching = new MatchingService();

        private List<SDirectory> _directories = new List<SDirectory>();
        List<SDirectory> _currentDirectories = new List<SDirectory>();
        private readonly List<SImage> _images = new List<SImage>();

        private ObservableCollection<SDirectory> _sortedDirectories = new ObservableCollection<SDirectory>();
        private ObservableCollection<SDirectory> _helperList = new ObservableCollection<SDirectory>();
        private readonly ObservableCollection<SImport> _imports = new ObservableCollection<SImport>();
        private ObservableCollection<SImage> _selectedImages = new ObservableCollection<SImage>();

        private delegate void IntervalAction(TimeSpan currentSecond);
        private delegate void EndAction();

        private readonly ActionCommand _invokeImportCmd;
        private readonly ActionCommand _invokeFileChooserCommand;
        private readonly ActionCommand _beginImportCommand;
        private readonly ActionCommand _resetSortingCommand;
        private readonly ActionCommand _showImagesCommand;
        private readonly ActionCommand _updateDirectoryNameCommand;
        private readonly ActionCommand _deleteDirectoryCommand;

        public ICommand InvokeImportCommand { get { return _invokeImportCmd; } }
        public ICommand InvokeFileChooserCommand { get { return _invokeFileChooserCommand; } }
        public ICommand BeginImportCommand { get { return _beginImportCommand; } }
        public ICommand ResetSortingCommand { get { return _resetSortingCommand; } }
        public ICommand ShowImagesCommand { get { return _showImagesCommand; } }
        public ICommand UpdateDirectoryNameCommand { get { return _updateDirectoryNameCommand; } }
        public ICommand DeleteDirectoryCommand { get { return _deleteDirectoryCommand; } }

        public MainViewModel()
        {
            _invokeImportCmd = new ActionCommand(InvokeImportPopupAction);
            _invokeFileChooserCommand = new ActionCommand(InvokeTargetChooser);
            _beginImportCommand = new ActionCommand(ConfirmImportSettings);
            _resetSortingCommand = new ActionCommand(CancelSorting);
            _showImagesCommand = new ActionCommand(ShowImages);
            _updateDirectoryNameCommand = new ActionCommand(SetDirectoryName);
            _deleteDirectoryCommand = new ActionCommand(DeleteImageDirectoryHandler);

            _fileDialog = new CommonOpenFileDialog
            {
                InitialDirectory = @"C:\users",
                Title = "Choose Import Target",
                IsFolderPicker = true,
                Multiselect = false
            };

            _ = StartupActionsAsync();
        }

        public void ShowImages(object obj)
        {
            var directory = GetDirectoryFromId(obj.ToString());

            SelectedImages.Clear();
            SelectedDirectory = directory;
            DirectoryName = directory.Name;

            foreach (var image in directory.ImageList)
            {
                SelectedImages.Insert(0, image);
            }
        }

        public SDirectory GetDirectoryFromId(string id)
        {
            return SortedDirectories.First(d => d.Id == id);
        }

        public SDirectory SelectedDirectory
        {
            get => _selectedDirectory;
            set
            {
                _selectedDirectory = value;
                OnPropertyChanged("SelectedDirectory");
            }
        }
        public SImage SelectedImage { get; set; }
        public ObservableCollection<SDirectory> SortedDirectories { get => _sortedDirectories;
            set
            {
                _sortedDirectories = value;
                OnPropertyChanged("SortedDirectories");
            }
        }
        public ObservableCollection<SImage> SelectedImages
        {
            get => _selectedImages;
            set
            {
                _selectedImages = value;
                OnPropertyChanged("SelectedImages");
            }
        }
        public string ErrorMessage { get => _errorMessage; set { _errorMessage = value; OnPropertyChanged("ErrorMessage"); } }
        public Visibility ErrorMessageVisibility { get => _errorMessageIsVisible; set { _errorMessageIsVisible = value; OnPropertyChanged("ErrorMessageVisibility"); } }
        public Visibility LoaderVisibility { get => _loaderIsShowing; set { _loaderIsShowing = value; OnPropertyChanged("LoaderVisibility"); } }

        public DateTime? SelectedSortingDate
        {
            get => _selectedSortingDate;
            set
            {
                _selectedSortingDate = value; 
                UpdateSorting(); 
                OnPropertyChanged("SelectedSortingDate");
            }
        }
        public string DirectoryName { get => _directoryName; set { _directoryName = value; OnPropertyChanged("DirectoryName"); } }
        public bool ImportPopupVisibility { get => _importPopupIsOpen; set { _importPopupIsOpen = value; OnPropertyChanged("ImportPopupVisibility"); } }
        public bool ImportConfirmationPopupVisibility { get => _importConfirmationPopupIsVisible; set { _importConfirmationPopupIsVisible = value; OnPropertyChanged("ImportConfirmationPopupVisibility"); } }
        public string CurrentImportCountDownTimeSpan { get => _currentImportCountDownSeconds; set { _currentImportCountDownSeconds = value; OnPropertyChanged("CurrentImportCountDownTimeSpan"); } }
        public string ImportTargetPath { get => _importTargetPath; set { _importTargetPath = value; OnPropertyChanged("ImportTargetPath"); } }
        public bool ImportConfirmationButtonIsEnabled { get => _importConfirmationButtonIsEnabled; set { _importConfirmationButtonIsEnabled = value; OnPropertyChanged("ImportConfirmationButtonIsEnabled"); } }
        public string FoundImportImagesCount { get => _foundImportImagesCount; set { _foundImportImagesCount = value; OnPropertyChanged("FoundImportImagesCount"); } }
        public bool ImportBtnIsEnabled { get => _importBtnIsEnabled; set { _importBtnIsEnabled = value; OnPropertyChanged("ImportBtnIsEnabled"); } }
        public string ImportOriginPath { get => _importOriginPath; set { _importOriginPath = value; OnPropertyChanged("ImportOriginPath"); } }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private async Task StartupActionsAsync()
        {
            await Task.Run(async () =>
            {
                var fetchedDirectories = await _dbService.GetDirectoriesAsync();
                _directories = fetchedDirectories;
                PushToDirectories(fetchedDirectories);
            });
            ErrorMessageVisibility = Visibility.Hidden;
            LoaderVisibility = Visibility.Hidden;
        }

        private void PushToDirectories(List<SDirectory> directories)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                foreach (var directory in directories)
                {
                    SortedDirectories.Insert(0, directory);
                    AddImportIfNotExists(directory.ParentImport);
                }
                AddImportIfNotExists(directories[0].ParentImport);
                LoaderVisibility = Visibility.Hidden;
            });
        }

        public void InvokeImportPopupAction(object obj)
        {
            ImportPopupVisibility = true;
        }

        private void InvokeTargetChooser(object obj)
        {
            ImportPopupVisibility = false;
            if (_fileDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                ImportPopupVisibility = true;
                if (SelectionMatchesRequirements())
                {
                    ImportOriginPath = _fileDialog.FileName;
                    ImportConfirmationButtonIsEnabled = true;
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
            SetTimer(15, 1, false, () => ErrorMessageVisibility = Visibility.Hidden);
        }

        private void HandleListingProgress()
        {
            ImportPopupVisibility = false;
            SetTimer(9, 1, true, InvokeImportAction);
        }

        private void UpdateImportCountDownNumber(TimeSpan currentSecond)
        {
            CurrentImportCountDownTimeSpan = currentSecond.ToString(@"\ s");
        }

        private void SetTimer(int timeSpanSeconds, int updateInterval, bool IsCountDown, EndAction endAction)
        {
            var timeSpan = TimeSpan.FromSeconds(timeSpanSeconds);
            _timer = new DispatcherTimer(new TimeSpan(0, 0, updateInterval),
                DispatcherPriority.Normal, delegate
                {
                    if(IsCountDown)
                        UpdateImportCountDownNumber(timeSpan);

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
            ImportConfirmationPopupVisibility = false;
            var sortingTask = Task.Run(() =>
            {
                _currentDirectories.Clear();
                var currentList = _matching.MatchImages(_images); // make it async
                foreach(var directory in currentList)
                {
                    _currentDirectories.Insert(0, directory);
                    _directories.Insert(0, directory); // add newly imported images to those already fetched on startup
                }
            });
            sortingTask.ContinueWith(task =>
            {
                ImportTargetPath = _directories[0].Origin;
                CopyFiles();
                PushToDirectories(_currentDirectories);
            });
        }

        private async void CopyFiles()
        {
            //TODO: maybe add this as configurable parameter
            var mainDirectory =
                Environment.GetFolderPath(Environment.SpecialFolder.MyPictures) + @"\OTS_IMG";

            if (!Directory.Exists(mainDirectory))
                Directory.CreateDirectory(mainDirectory);

            foreach (var directory in _currentDirectories)
            {
                var destination = mainDirectory + @"\" + directory.Name;
                directory.Target = mainDirectory;
                Directory.CreateDirectory(destination);

                foreach (var image in directory.ImageList)
                {
                    var source = Path.Combine(image.Origin);
                    image.Target = destination;
                    File.Copy(source, destination + @"\" + image.Name, true);
                }
                await _dbService.UpdateDirectoryAsync(directory);
            }
            LoaderVisibility = Visibility.Hidden;
        }

        private void ConfirmImportSettings(object sender)
        {
            if (Directory.Exists(ImportOriginPath))
            {
                ImportBtnIsEnabled = true;
                ImportPopupVisibility = false;

                ImportConfirmationPopupVisibility = true;
                LoaderVisibility = Visibility.Visible;
                Import(ImportOriginPath);
                HandleListingProgress();
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
                FoundImportImagesCount = "Found " + _images.Count() + " images";
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

        private void AddImportIfNotExists(SImport import)
        {
            if (!_imports.Contains(import))
            {
                _imports.Insert(0, import);
            }
        }

        public async void SetDirectoryName(object obj)
        {
            SelectedDirectory.ChangeDirectoryName(DirectoryName);
            SelectedDirectory.Name = DirectoryName;

            await _dbService.UpdateDirectoryAsync(SelectedDirectory);
            RefreshDirectoryListView(); // is this valid MVVM? Did not work without explicit call
        }

        private void RefreshDirectoryListView()
        {
            CollectionViewSource.GetDefaultView(SortedDirectories).Refresh();
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

        public async void DeleteImageDirectoryHandler(object obj)
        {
            var directory = GetDirectoryFromId(obj.ToString());
            await DeleteImageDirectory(directory);
        }

        public async Task DeleteImageDirectory(SDirectory directory)
        {
            if (directory.Delete())
            {
                var import = directory.ParentImport;
                import.Directories.Remove(directory);
                SortedDirectories.Remove(directory);

                if (import.Directories.Count == 0)
                    await _dbService.DeleteImportAsync(import.Id);
                else
                    await _dbService.UpdateImportAsync(import);

                await _dbService.DeleteDirectoryAsync(directory.Id);

                HandleError("Directory was deleted successfully.");
            } else
            {
                HandleError("Directory could not be deleted.");
            }
            RefreshDirectoryListView();
        }

        public async Task UpdateImageDirectory(SDirectory directory)
        {
            await _dbService.UpdateDirectoryAsync(directory);
        }

        public void UpdateSorting()
        {
            var tempList = new List<SDirectory>();
            var targetDate = SelectedSortingDate;
            SortedDirectories.Clear();

            foreach (var import in _imports)
                if (import.Timestamp == targetDate)
                    foreach (var directory in import.Directories)
                    {
                        tempList.Insert(0, directory);
                    }

            PushToDirectories(tempList);
        }

        public void CancelSorting(object obj)
        {
            PushToDirectories(_directories);
        }

        // called from XAML

        private void CancelSortAfterDate(object sender, MouseButtonEventArgs e)
        {
            SelectedSortingDate = null;
        }
    }
}