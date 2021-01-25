using System.ComponentModel;
using System.Windows.Input;
using OpenTimelapseSort.Contexts;
using OpenTimelapseSort.DataServices;
using OpenTimelapseSort.Models;
using OpenTimelapseSort.Mvvm;

namespace OpenTimelapseSort.ViewModels
{
    internal class PreferencesViewModel
    {
        private readonly DbPreferencesService _dbPreferencesService = new DbPreferencesService();

        private readonly ActionCommand _savePreferencesCommand;
        private readonly ActionCommand _deletePreferencesCommand;

        private Preferences _preferences;

        public PreferencesViewModel()
        {
            _savePreferencesCommand = new ActionCommand(SavePreferences);
            _deletePreferencesCommand = new ActionCommand(DeletePreferences);
            StartupActions();
        }

        private void StartupActions()
        {
            SelectedPreferences = _dbPreferencesService.FetchPreferences();
        }

        public ICommand SavePreferencesCommand => _savePreferencesCommand;
        public ICommand DeletePreferencesCommand => _deletePreferencesCommand;

        public Preferences SelectedPreferences
        {
            get => _preferences;
            set
            {
                _preferences = value;
                OnPropertyChanged("SelectedPreferences");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void SavePreferences(object obj)
        {
            using var database = new PreferencesContext();
            _dbPreferencesService.SavePreferencesToDataBase(SelectedPreferences);
        }

        public void DeletePreferences(object obj)
        {
            using var database = new PreferencesContext();
            _dbPreferencesService.DeletePreferences(SelectedPreferences);
        }
    }
}