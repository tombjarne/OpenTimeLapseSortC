using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Input;
using OpenTimelapseSort.Contexts;
using OpenTimelapseSort.DataServices;
using OpenTimelapseSort.Models;
using OpenTimelapseSort.Mvvm;

namespace OpenTimelapseSort.ViewModels
{
    internal class PreferencesViewModel
    {
        /// <summary>
        ///     service
        /// </summary>
        private readonly DbPreferencesService _dbPreferencesService = new DbPreferencesService();

        /// <summary>
        ///     ActionCommands
        /// </summary>
        private readonly ActionCommand _deletePreferencesCommand;
        private readonly ActionCommand _savePreferencesCommand;

        /// <summary>
        /// Preferences
        ///     local preferences instance
        /// </summary>
        private Preferences _preferences;

        public PreferencesViewModel()
        {
            _savePreferencesCommand = new ActionCommand(SavePreferences);
            _deletePreferencesCommand = new ActionCommand(DeletePreferences);
            StartupActions();
        }

        public ICommand SavePreferencesCommand => _savePreferencesCommand;
        public ICommand DeletePreferencesCommand => _deletePreferencesCommand;

        /// <summary>
        ///     SelectedPreferences
        ///     binds current preferences to view and vice versa
        /// </summary>
        public Preferences SelectedPreferences
        {
            get => _preferences;
            set
            {
                _preferences = value;
                OnPropertyChanged("SelectedPreferences");
            }
        }

        /// <summary>
        ///     PropertyChangedEventHandler
        ///     event that is triggered on property change
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        ///     StartupActions()
        ///     fetches preferences from the database
        ///     calls <see cref="_dbPreferencesService" /> to do so
        /// </summary>
        private void StartupActions()
        {
            SelectedPreferences = _dbPreferencesService.FetchPreferences();
        }

        /// <summary>
        ///     SavePreferences()
        ///     handles the saving of preferences set in preferences view
        ///     calls <see cref="_dbPreferencesService" /> to do so
        /// </summary>
        /// <param name="obj"></param>
        public void SavePreferences(object obj)
        {
            using var database = new PreferencesContext();
            _dbPreferencesService.SavePreferences(SelectedPreferences);
        }

        /// <summary>
        ///     DeletePreferences()
        ///     handles the deletion of the current preferences
        ///     calls <see cref="_dbPreferencesService" /> to do so
        /// </summary>
        /// <param name="obj"></param>
        public void DeletePreferences(object obj)
        {
            using var database = new PreferencesContext();
            _dbPreferencesService.DeletePreferences(SelectedPreferences);
            SelectedPreferences = _dbPreferencesService.FetchPreferences();
        }
    }
}