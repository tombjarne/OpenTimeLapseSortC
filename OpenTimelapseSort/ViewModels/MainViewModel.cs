using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
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

        private readonly ActionCommand _beginImportCommand;
        private readonly ActionCommand _closeImportConfirmationPopupCommand;
        private readonly List<SDirectory> _currentDirectories = new List<SDirectory>();
        private readonly DbService _dbService = new DbService();

        private readonly ActionCommand _deleteDirectoryCommand;
        private readonly DirectoryDetailService _directoryDetailService = new DirectoryDetailService();
        private readonly CommonOpenFileDialog _fileDialog;
        private readonly List<SImage> _images = new List<SImage>();
        private readonly ObservableCollection<SImport> _imports = new ObservableCollection<SImport>();
        private readonly ActionCommand _invokeFileChooserCommand;
        private readonly ActionCommand _invokeImportCmd;

        private readonly MatchingService _matching = new MatchingService();
        private readonly ActionCommand _resetSortingCommand;
        private readonly ActionCommand _showDirectoryLocationCommand;
        private readonly ActionCommand _showImagesCommand;
        private readonly ActionCommand _updateDirectoryNameCommand;
        private string _currentImportCountDownSeconds;

        private List<SDirectory> _directories = new List<SDirectory>();
        private string _directoryName;
        private string _directoryPath;

        private string _errorMessage;
        private Visibility _errorMessageIsVisible;
        private string _foundImportImagesCount;
        private bool _importBtnIsEnabled;
        private bool _importConfirmationButtonIsEnabled;
        private bool _importConfirmationPopupIsVisible;
        private string _importOriginPath;
        private bool _importPopupIsOpen;
        private string _importTargetPath;
        private Visibility _loaderIsShowing;
        private SDirectory _selectedDirectory;
        private ObservableCollection<SImage> _selectedImages = new ObservableCollection<SImage>();
        private DateTime? _selectedSortingDate;

        private ObservableCollection<SDirectory> _sortedDirectories = new ObservableCollection<SDirectory>();
        private DispatcherTimer _timer = new DispatcherTimer();

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

        public ICommand InvokeImportCommand => _invokeImportCmd;
        public ICommand InvokeFileChooserCommand => _invokeFileChooserCommand;
        public ICommand BeginImportCommand => _beginImportCommand;
        public ICommand ResetSortingCommand => _resetSortingCommand;
        public ICommand ShowImagesCommand => _showImagesCommand;
        public ICommand UpdateDirectoryNameCommand => _updateDirectoryNameCommand;
        public ICommand DeleteDirectoryCommand => _deleteDirectoryCommand;
        public ICommand ShowDirectoryLocationCommand => _showDirectoryLocationCommand;
        public ICommand CloseImportConfirmationPopupCommand => _closeImportConfirmationPopupCommand;

        public SDirectory SelectedDirectory
        {
            get => _selectedDirectory;
            set
            {
                _selectedDirectory = value;
                OnPropertyChanged("SelectedDirectory");
            }
        }

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

        /// <summary>
        ///     CloseImportConfirmationPopup()
        ///     Closes import confirmation popup by binding value to <see cref="ImportConfirmationPopupVisibility" />
        /// </summary>
        /// <param name="sender"></param>
        public void CloseImportConfirmationPopup(object sender)
        {
            ImportConfirmationPopupVisibility = false;
        }

        /// <summary>
        ///     ShowDirectoryLocation()
        ///     Invokes file explorer for passed directory id in <see cref="obj" />
        /// </summary>
        /// <param name="obj"></param>
        public void ShowDirectoryLocation(object obj)
        {
            var location = MainDirectoryPath + @"\" + obj;
            var argument = "/select, \"" + location + "\"";

            if (Directory.Exists(location)) Process.Start("explorer.exe", argument);
        }

        /// <summary>
        ///     ShowImages()
        ///     Sets observed property <see cref="SelectedImages" /> with images from selected directory
        /// </summary>
        /// <param name="obj"></param>
        public void ShowImages(object obj)
        {
            var directory = GetDirectoryFromId(obj.ToString());

            SelectedImages.Clear();
            SelectedDirectory = directory;
            DirectoryName = directory.Name;
            DirectoryPath = directory.Target;

            foreach (var image in directory.ImageList) SelectedImages.Insert(0, image);
        }

        /// <summary>
        ///     GetDirectoryFromId()
        ///     Returns directory instance from <see cref="SortedDirectories" /> that matches passed <see cref="id" />
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public SDirectory GetDirectoryFromId(string id)
        {
            return SortedDirectories.First(d => d.Id == id);
        }

        /// <summary>
        ///     OnPropertyChanged()
        ///     Invokes handling of new <see cref="PropertyChangedEventArgs" /> instance from passed <see cref="propertyName" />
        /// </summary>
        /// <param name="propertyName"></param>
        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        ///     StartupActionsAsync
        ///     Fetches directories from database
        ///     Calls <see cref="PushToDirectories" /> to update local observed list
        /// </summary>
        /// <returns></returns>
        private async Task StartupActionsAsync()
        {
            HandleError("Fetching database entries...");

            var fetchedDirectories = await _dbService.GetDirectoriesAsync();
            _directories = fetchedDirectories;
            PushToDirectories(fetchedDirectories);

            ErrorMessageVisibility = Visibility.Hidden;
            LoaderVisibility = Visibility.Hidden;
        }


        /// <summary>
        ///     PushToDirectories()
        ///     Pushes a passed list to the observed Property <see cref="SortedDirectories" />
        /// </summary>
        /// <param name="directories"></param>
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

        /// <summary>
        ///     InvokeImportPopupAction()
        ///     Sets <see cref="ImportPopupVisibility" /> to true
        ///     Bound to <see cref="_invokeImportCmd" />
        /// </summary>
        /// <param name="obj"></param>
        public void InvokeImportPopupAction(object obj)
        {
            ImportPopupVisibility = true;
        }


        /// <summary>
        ///     InvokeTargetChooser()
        ///     Invokes a generic file chooser window
        ///     Sets other popup visibilities
        ///     Calls <see cref="HandleError" /> in case of an invalid option
        ///     Bound to <see cref="_invokeFileChooserCommand" />
        /// </summary>
        /// <param name="obj"></param>
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

        /// <summary>
        ///     HandleError()
        ///     Sets an error message from passed parameter
        ///     Hides the error message after 10 seconds
        /// </summary>
        /// <param name="errorMessage"></param>
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

        /// <summary>
        ///     HandleListingProgress()
        ///     Sets the import timer interval and passes <see cref="InvokeImportAction" /> to execute after timer has stopped
        /// </summary>
        private void HandleListingProgress()
        {
            ImportPopupVisibility = false;
            SetTimer(9, 1, true, InvokeImportAction);
        }

        /// <summary>
        ///     UpdateImportCountDownNumber()
        ///     Sets remaining timespan before timer stops
        /// </summary>
        /// <param name="currentSecond"></param>
        private void UpdateImportCountDownNumber(TimeSpan currentSecond)
        {
            CurrentImportCountDownTimeSpan = currentSecond.ToString(@"\ s");
        }

        /// <summary>
        ///     SetTimer()
        ///     Generic function to set a timer with passed interval
        ///     Performs update action on <see cref="UpdateImportCountDownNumber" />
        ///     Performs action as soon as the timer has ended
        /// </summary>
        /// <param name="timeSpanSeconds"></param>
        /// <param name="updateInterval"></param>
        /// <param name="isCountDown"></param>
        /// <param name="endAction"></param>
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

        /// <summary>
        ///     InvokeImportAction()
        ///     Starts the matching process
        ///     Saves retrieved list into the local Property <see cref="_currentDirectories" />
        /// </summary>
        private void InvokeImportAction()
        {
            ImportConfirmationPopupVisibility = false;

            _currentDirectories.Clear();
            var currentList = _matching.MatchImages(_images); // make it async
            foreach (var directory in currentList)
            {
                _currentDirectories.Insert(0, directory);
                _directories.Insert(0, directory); // add newly imported images to those already fetched on startup
            }

            ImportTargetPath = _directories[0].Origin;
            CopyFiles();
            PushToDirectories(_currentDirectories);
        }

        /// <summary>
        ///     CopyFiles()
        ///     Actually copies the matched directories to their destination
        ///     Creates a new directory if <see cref="MainDirectoryPath" /> does not point to valid file location
        ///     Sets <see cref="SDirectory.Target" /> attribute to the actual new location
        ///     Calls <see cref="_dbService" /> to save the just copied files into the database
        /// </summary>
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

        /// <summary>
        ///     ConfirmImportSettings()
        ///     Is called on button click
        ///     Calls <see cref="HandleError" /> if the selected location is not reachable
        ///     Sets the visibility of import popups and shows the loader icon
        /// </summary>
        /// <param name="sender"></param>
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

        /// <summary>
        ///     Import()
        ///     Collects all files of a specified directory from a maximum depth of 2
        ///     Updates <see cref="FoundImportImagesCount" /> with number of images found
        /// </summary>
        /// <param name="name"></param>
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

        /// <summary>
        ///     CreateImage()
        ///     Creates an instance of <see cref="SImage" /> with the passed attributes
        /// </summary>
        /// <param name="file"></param>
        /// <param name="info"></param>
        /// <returns></returns>
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

        /// <summary>
        ///     SelectionMatchesRequirements()
        ///     Determines whether the local file chooser wants to import from an allowed location
        ///     Forbids the import from any directory that contains "Windows" or "Default"
        ///     The selected directory must match an actual existing file path
        /// </summary>
        /// <returns></returns>
        private bool SelectionMatchesRequirements()
        {
            // TODO: fix button issue!
            return _fileDialog.FileName != "Default" &&
                   !_fileDialog.FileName.Contains("Windows") &&
                   Directory.Exists(_fileDialog.FileName);
        }

        /// <summary>
        ///     AddImportIfNotExists()
        ///     Adds an import to <see cref="_imports" /> if it has not been added yet
        /// </summary>
        /// <param name="import"></param>
        private void AddImportIfNotExists(SImport import)
        {
            if (!_imports.Contains(import)) _imports.Insert(0, import);
        }

        /// <summary>
        ///     SetDirectoryName()
        ///     Calls <see cref="_dbService" /> to update directory
        ///     Updates observable property <see cref="DirectoryName" />
        ///     Calls <see cref="HandleError" /> if the name could not be updated in <see cref="SelectedDirectory" /> instance
        /// </summary>
        /// <param name="obj"></param>
        public async void SetDirectoryName(object obj)
        {
            var indexToReplace = SortedDirectories.IndexOf(SelectedDirectory);
            var directory = _directoryDetailService.ChangeDirectoryName(SelectedDirectory, DirectoryName, HandleError);
            SelectedDirectory = directory;

            await _dbService.UpdateDirectoryAsync(SelectedDirectory);
            DirectoryName = SelectedDirectory.Name;
            SortedDirectories.RemoveAt(indexToReplace);
            SortedDirectories.Insert(indexToReplace, SelectedDirectory);
        }

        /// <summary>
        ///     DeleteImageDirectoryHandler()
        ///     Calls <see cref="DeleteImageDirectory" />
        /// </summary>
        /// <param name="obj"></param>
        public async void DeleteImageDirectoryHandler(object obj)
        {
            var directory = GetDirectoryFromId(obj.ToString());
            await DeleteImageDirectory(directory);
        }

        /// <summary>
        ///     DeleteImageDirectory()
        ///     Handles deletion of passed <see cref="SDirectory" />
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        public async Task DeleteImageDirectory(SDirectory directory)
        {
            if (_directoryDetailService.Delete(directory))
            {
                await _dbService.UpdateImportAfterRemovalAsync(directory.Id);

                EmptyCurrentSession(directory);
                HandleError("Directory was deleted successfully.");
            }
            else
            {
                HandleError("Directory could not be deleted.");
            }
        }

        /// <summary>
        ///     EmptyCurrentSession()
        ///     Resets values after directory deletion
        /// </summary>
        /// <param name="directory"></param>
        private void EmptyCurrentSession(SDirectory directory)
        {
            SortedDirectories.Remove(directory);
            _imports.Single(i => i.Id == directory.ImportId).Directories.Remove(directory);
            _directories.Remove(directory);

            foreach (var image in directory.ImageList) SelectedImages.Remove(image);

            DirectoryName = "";
            DirectoryPath = "";
        }

        /// <summary>
        ///     UpdateSorting()
        ///     Iterates through imports and adds them to a temporary list
        ///     The sorted directories will be pushed to <see cref="SortedDirectories" /> via <see cref="PushToDirectories" />
        /// </summary>
        public void UpdateSorting()
        {
            var tempList = new List<SDirectory>();
            var targetDate = SelectedSortingDate;
            SortedDirectories.Clear();

            foreach (var import in _imports)
                if (import.Timestamp == targetDate)
                    foreach (var directory in import.Directories)
                        tempList.Add(directory);

            PushToDirectories(tempList);
        }

        /// <summary>
        ///     CancelSorting()
        ///     Cancels the current sorting options and sets <see cref="SortedDirectories" /> via <see cref="PushToDirectories" />
        /// </summary>
        /// <param name="obj"></param>
        public void CancelSorting(object obj)
        {
            if (_directories.Count > 0)
            {
                SortedDirectories.Clear();
                PushToDirectories(_directories);
            }
        }

        private delegate void EndAction();
    }
}