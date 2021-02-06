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
using Microsoft.Data.Sqlite;
using OpenTimelapseSort.DataServices;
using OpenTimelapseSort.Models;
using OpenTimelapseSort.Mvvm;

namespace OpenTimelapseSort.ViewModels
{
    internal class MainViewModel : INotifyPropertyChanged
    {
        private readonly ActionCommand _beginImportCommand;
        private readonly ActionCommand _closeImportConfirmationPopupCommand;
        private readonly List<SDirectory> _currentDirectories = new List<SDirectory>();
        private readonly DbService _dbService = new DbService();

        private readonly ActionCommand _deleteDirectoryCommand;
        private readonly DirectoryDetailService _directoryDetailService = new DirectoryDetailService();
        private readonly FileCopyService _fileCopyService = new FileCopyService();
        private readonly ObservableCollection<SImport> _imports = new ObservableCollection<SImport>();
        private readonly ImportService _importService = new ImportService();
        private readonly ActionCommand _invokeImportCmd;

        private readonly MatchingService _matching = new MatchingService();
        private readonly ActionCommand _resetSortingCommand;
        private readonly ActionCommand _showDirectoryLocationCommand;
        private readonly ActionCommand _showImagesCommand;
        private readonly ActionCommand _updateDirectoryNameCommand;

        private List<SDirectory> _directories = new List<SDirectory>();
        private string _directoryName;
        private string _directoryPath;

        private string _errorMessage;
        private Visibility _errorMessageIsVisible;
        private string _foundImportImagesCount;
        private List<SImage> _images = new List<SImage>();
        private bool _importConfirmationButtonIsEnabled;
        private string _importOriginPath;
        private bool _importPopupIsOpen;
        private string _importTargetPath;
        private Visibility _loaderIsShowing;

        private string _mainDirectoryPath =
            Environment.GetFolderPath(Environment.SpecialFolder.MyPictures) + @"\OTS_IMG";

        private SDirectory _selectedDirectory;
        private ObservableCollection<SImage> _selectedImages = new ObservableCollection<SImage>();
        private DateTime? _selectedSortingDate;

        private ObservableCollection<SDirectory> _sortedDirectories = new ObservableCollection<SDirectory>();

        public MainViewModel()
        {
            _invokeImportCmd = new ActionCommand(InvokeImportPopupAction);
            _beginImportCommand = new ActionCommand(StartImportAction);
            _resetSortingCommand = new ActionCommand(CancelSorting);
            _showImagesCommand = new ActionCommand(ShowImages);
            _updateDirectoryNameCommand = new ActionCommand(SetDirectoryName);
            _deleteDirectoryCommand = new ActionCommand(DeleteImageDirectoryHandler);
            _showDirectoryLocationCommand = new ActionCommand(ShowDirectoryLocation);
            _closeImportConfirmationPopupCommand = new ActionCommand(CloseImportConfirmationPopup);

            StartupActionsAsync();
        }

        public ICommand InvokeImportCommand => _invokeImportCmd;
        public ICommand BeginImportCommand => _beginImportCommand;
        public ICommand ResetSortingCommand => _resetSortingCommand;
        public ICommand ShowImagesCommand => _showImagesCommand;
        public ICommand UpdateDirectoryNameCommand => _updateDirectoryNameCommand;
        public ICommand DeleteDirectoryCommand => _deleteDirectoryCommand;
        public ICommand ShowDirectoryLocationCommand => _showDirectoryLocationCommand;
        public ICommand CloseImportConfirmationPopupCommand => _closeImportConfirmationPopupCommand;

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
        /// </summary>
        /// <param name="location"></param>
        public void SetImportTarget(string location)
        {
            if (location != "")
            {
                _mainDirectoryPath = location;
                ImportTargetPath = location;
            }
            else
            {
                HandleError("The selected directory location was invalid.");
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="location"></param>
        public void SetImportOrigin(string location)
        {
            if (location != "")
            {
                _images = _importService.Import(location, HandleError);
                FoundImportImagesCount = "Found " + _images.Count + " Images";
                ImportOriginPath = Directory.GetParent(location).FullName;
                ImportConfirmationButtonIsEnabled = true;
            }
            else
            {
                HandleError("The selected directory location was invalid.");
            }
        }

        /// <summary>
        ///     CloseImportConfirmationPopup()
        ///     Closes import confirmation popup by binding value to <see cref="ImportPopupVisibility" />
        /// </summary>
        /// <param name="sender"></param>
        public void CloseImportConfirmationPopup(object sender)
        {
            ImportPopupVisibility = false;
        }

        /// <summary>
        ///     ShowDirectoryLocation()
        ///     Invokes file explorer for passed directory id in <see cref="obj" />
        /// </summary>
        /// <param name="obj"></param>
        public void ShowDirectoryLocation(object obj)
        {
            var id = obj.ToString();
            var directory = SortedDirectories.Single(d => d.Id == id);
            var location = directory.Target + @"\" + directory.Name;
            var argument = "/select, \"" + location + "\"";

            if (Directory.Exists(location))
                Process.Start("explorer.exe", argument);
            else
                HandleError("Could not open in explorer. Invalid path.");
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
            _selectedDirectory = directory;
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
        private async void StartupActionsAsync()
        {
            LoaderVisibility = Visibility.Visible;
            HandleError("Fetching database entries...");

            var fetchedDirectories = await _dbService.GetDirectoriesAsync();
            if (fetchedDirectories.Count != 0)
            {
                _directories = fetchedDirectories;
                PushToDirectories(fetchedDirectories);
            }

            ErrorMessageVisibility = Visibility.Hidden;
            LoaderVisibility = Visibility.Hidden;
        }

        /// <summary>
        ///     PushToDirectories()
        ///     Pushes a passed list to the observed Property <see cref="SortedDirectories" />
        /// </summary>
        /// <param name="directories"></param>
        private async void PushToDirectories(List<SDirectory> directories)
        {
            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                foreach (var directory in directories)
                {
                    SortedDirectories.Insert(0, directory);
                    AddImportIfNotExists(directory.ParentImport);
                }

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
        ///     StartImportAction()
        ///     Is called on button click
        ///     Calls <see cref="HandleError" /> if the selected location is not reachable
        ///     Sets the visibility of import popups and shows the loader icon
        /// </summary>
        /// <param name="sender"></param>
        private void StartImportAction(object sender)
        {
            var importTask = Task.Run(async () =>
            {
                if (Directory.Exists(ImportOriginPath))
                {
                    if (ImportTargetPath != _mainDirectoryPath)
                        ImportTargetPath = _mainDirectoryPath;

                    ImportPopupVisibility = false;
                    LoaderVisibility = Visibility.Visible;

                    _currentDirectories.Clear();

                    try
                    {
                        var importExists = _imports.Any();
                        var currentList = _ = _matching.MatchImages(_images);
                        foreach (var directory in currentList)
                        {
                            _currentDirectories.Add(directory);
                            _directories.Add(directory);

                            if (!importExists)
                            {
                                var import = _imports.Single(i => i.Id == directory.ImportId);
                                import.Directories.Add(directory);
                            }
                        }

                        await _fileCopyService.CopyFiles(_currentDirectories,
                            _mainDirectoryPath, HandleError);
                    }
                    catch (FileNotFoundException)
                    {
                        HandleError("Location unreachable, did you delete something?");
                    }
                    catch (OutOfMemoryException)
                    {
                        HandleError("Memory exhausted!");
                    }

                    LoaderVisibility = Visibility.Hidden;
                }
                else
                {
                    HandleError("The target location is unreachable.");
                }
            });
            importTask.ContinueWith(task =>
            {
                PushToDirectories(_currentDirectories);
                EmptyImportSession();
            });
        }

        /// <summary>
        ///     AddImportIfNotExists()
        ///     Adds an import to <see cref="_imports" /> if it has not been added yet
        /// </summary>
        /// <param name="import"></param>
        private void AddImportIfNotExists(SImport import)
        {
            var match = _imports.Any(i => i.Id == import.Id);
            if (!match) _imports.Insert(0, import);
        }

        /// <summary>
        ///     SetDirectoryName()
        ///     Calls <see cref="_dbService" /> to update directory
        ///     Updates observable property <see cref="DirectoryName" />
        ///     Calls <see cref="HandleError" /> if the name could not be updated in <see cref="_selectedDirectory" /> instance
        /// </summary>
        /// <param name="obj"></param>
        public async void SetDirectoryName(object obj)
        {
            var indexToReplace = SortedDirectories.IndexOf(_selectedDirectory);
            var directory = _directoryDetailService
                .ChangeDirectoryName(_selectedDirectory, DirectoryName, HandleError);

            _selectedDirectory = directory;
            await _dbService.UpdateDirectoryAsync(_selectedDirectory);
            DirectoryName = _selectedDirectory.Name;
            SortedDirectories.RemoveAt(indexToReplace);
            SortedDirectories.Insert(indexToReplace, _selectedDirectory);
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
            try
            {
                await _dbService.UpdateImportAfterRemovalAsync(directory.Id);
                if (_directoryDetailService.Delete(directory))
                {
                    EmptyCurrentSession(directory);
                    HandleError("Directory was deleted successfully.");
                }
                else
                {
                    HandleError("Directory could not be deleted.");
                }
            }
            catch (OutOfMemoryException)
            {
                HandleError("Memory exhausted!");
            }
            catch (SqliteException)
            {
                HandleError("Could not delete entry from database");
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
        /// EmptyImportSession()
        /// resets the most crucial values after an import has finished
        /// </summary>
        private void EmptyImportSession()
        {
            ImportConfirmationButtonIsEnabled = false;
            ImportPopupVisibility = false;
            FoundImportImagesCount = null;
            ImportOriginPath = null;
            ImportTargetPath = null;
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
    }
}