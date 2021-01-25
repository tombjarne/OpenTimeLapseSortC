using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using Microsoft.WindowsAPICodePack.Dialogs;
using OpenTimelapseSort.DataServices;
using OpenTimelapseSort.Models;
using OpenTimelapseSort.Mvvm;

namespace OpenTimelapseSort.ViewModels
{
    internal class MainViewModel : INotifyPropertyChanged
    {
        private static readonly string MainDirectoryPath =
            Environment.GetFolderPath(Environment.SpecialFolder.MyPictures) + @"\OTS_IMG";

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
        private readonly List<SDirectory> _currentDirectories = new List<SDirectory>();
        private readonly List<SImage> _images = new List<SImage>();

        private ObservableCollection<SDirectory> _sortedDirectories = new ObservableCollection<SDirectory>();
        private readonly ObservableCollection<SImport> _imports = new ObservableCollection<SImport>();
        private ObservableCollection<SImage> _selectedImages = new ObservableCollection<SImage>();

        private delegate void EndAction();

        private readonly ActionCommand _invokeImportCmd;
        private readonly ActionCommand _invokeFileChooserCommand;
        private readonly ActionCommand _beginImportCommand;
        private readonly ActionCommand _resetSortingCommand;
        private readonly ActionCommand _showImagesCommand;
        private readonly ActionCommand _updateDirectoryNameCommand;
        private readonly ActionCommand _deleteDirectoryCommand;
        private readonly ActionCommand _showDirectoryLocationCommand;
        private readonly ActionCommand _closeImportConfirmationPopupCommand;

        public ICommand InvokeImportCommand => _invokeImportCmd;
        public ICommand InvokeFileChooserCommand => _invokeFileChooserCommand;
        public ICommand BeginImportCommand => _beginImportCommand;
        public ICommand ResetSortingCommand => _resetSortingCommand;
        public ICommand ShowImagesCommand => _showImagesCommand;
        public ICommand UpdateDirectoryNameCommand => _updateDirectoryNameCommand;
        public ICommand DeleteDirectoryCommand => _deleteDirectoryCommand;
        public ICommand ShowDirectoryLocationCommand => _showDirectoryLocationCommand;
        public ICommand CloseImportConfirmationPopupCommand => _closeImportConfirmationPopupCommand;

        public MainViewModel()
        {
            _invokeImportCmd = new ActionCommand(InvokeImportPopupAction);
            _invokeFileChooserCommand = new ActionCommand(InvokeTargetChooser);
            _beginImportCommand = new ActionCommand(ConfirmImportSettings);
            _resetSortingCommand = new ActionCommand(CancelSorting);
            _showImagesCommand = new ActionCommand(ShowImages);
            _updateDirectoryNameCommand = new ActionCommand(SetDirectoryName);
            _deleteDirectoryCommand = new ActionCommand(DeleteImageDirectoryHandler);
            _showDirectoryLocationCommand = new ActionCommand(ShowDirectoryLocation);
            _closeImportConfirmationPopupCommand = new ActionCommand(CloseImportConfirmationPopup);

            _fileDialog = new CommonOpenFileDialog
            {
                InitialDirectory = @"C:\users",
                Title = "Choose Import Target",
                IsFolderPicker = true,
                Multiselect = false
            };

            _ = StartupActionsAsync();
        }

        public void CloseImportConfirmationPopup(object sender)
        {
            ImportConfirmationPopupVisibility = false;
        }

        public void ShowDirectoryLocation(object obj)
        {
            var location = MainDirectoryPath + @"\" + obj;
            var argument = "/select, \"" + location + "\"";

            if (Directory.Exists(location)) Process.Start("explorer.exe", argument);
        }

        public void ShowImages(object obj)
        {
            var directory = GetDirectoryFromId(obj.ToString());

            SelectedImages.Clear();
            SelectedDirectory = directory;
            DirectoryName = directory.Name;
            DirectoryPath = directory.Target;

            foreach (var image in directory.ImageList) SelectedImages.Insert(0, image);
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

        public ObservableCollection<SDirectory> SortedDirectories
        {
            get => _sortedDirectories;
            set
            {
                _sortedDirectories = value;
                OnPropertyChanged("SortedDirectories");
            }
        }

        public string DirectoryPath
        {
            get => _directoryPath;
            set
            {
                _directoryPath = value;
                OnPropertyChanged("DirectoryPath");
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
                UpdateSorting();
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

        public bool ImportPopupVisibility
        {
            get => _importPopupIsOpen;
            set
            {
                _importPopupIsOpen = value;
                OnPropertyChanged("ImportPopupVisibility");
            }
        }

        public bool ImportConfirmationPopupVisibility
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

        private async Task StartupActionsAsync()
        {
            var startupTask = Task.Run(async () =>
            {
                HandleError("Fetching database entries...");

                var fetchedDirectories = await _dbService.GetDirectoriesAsync();
                _directories = fetchedDirectories;
                PushToDirectories(fetchedDirectories);
            });

            await startupTask.ContinueWith(task =>
            {
                ErrorMessageVisibility = Visibility.Hidden;
                LoaderVisibility = Visibility.Hidden;
            });
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
            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(10000);
                ErrorMessageVisibility = Visibility.Hidden;
                ErrorMessage = "";
            });
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

        private void SetTimer(int timeSpanSeconds, int updateInterval, bool isCountDown, EndAction endAction)
        {
            var timeSpan = TimeSpan.FromSeconds(timeSpanSeconds);
            _timer = new DispatcherTimer(new TimeSpan(0, 0, updateInterval),
                DispatcherPriority.Normal, delegate
                {
                    if (isCountDown)
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
                foreach (var directory in currentList)
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
            if (!Directory.Exists(MainDirectoryPath))
                Directory.CreateDirectory(MainDirectoryPath);

            foreach (var directory in _currentDirectories)
            {
                var destination = MainDirectoryPath + @"\" + directory.Name;
                directory.Target = MainDirectoryPath;
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
            if (!_imports.Contains(import)) _imports.Insert(0, import);
        }

        public async void SetDirectoryName(object obj)
        {
            if (!SelectedDirectory.ChangeDirectoryName(DirectoryName))
            {
                HandleError("Please provide a proper name.");
            }

            await _dbService.UpdateDirectoryAsync(SelectedDirectory);
            DirectoryName = SelectedDirectory.Name;
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

                await _dbService.UpdateImportAfterRemovalAsync(import, directory);

                EmptyCurrentSession(directory);

                HandleError("Directory was deleted successfully.");
            }
            else
            {
                HandleError("Directory could not be deleted.");
            }
        }

        private void EmptyCurrentSession(SDirectory directory)
        {
            SortedDirectories.Remove(directory);
            DirectoryName = "";
            foreach (var image in directory.ImageList) SelectedImages.Remove(image);
            RefreshDirectoryListView();
        }

        public void UpdateSorting()
        {
            var tempList = new List<SDirectory>();
            var targetDate = SelectedSortingDate;
            SortedDirectories.Clear();

            foreach (var import in _imports)
                if (import.Timestamp == targetDate)
                    foreach (var directory in import.Directories)
                        tempList.Insert(0, directory);

            PushToDirectories(tempList);
        }

        public void CancelSorting(object obj)
        {
            PushToDirectories(_directories);
        }
    }
}