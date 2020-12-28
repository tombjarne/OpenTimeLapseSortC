using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.WindowsAPICodePack.Dialogs;

using OpenTimelapseSort.ViewModels;
using OpenTimelapseSort.Mvvm;
using System;
using System.IO;

namespace OpenTimelapseSort.Views
{
    public partial class MainWindow : Window 
    {

        //////////////////////////////////////////////////////////
        //////                    VARIABLES                 //////
        //////////////////////////////////////////////////////////

        //public ICommand PrintCommand => new DelegateCommand(HandlePopup);
        //private delegate void WarningReference(string errorHeadline, string errorDetails);

        //////////////////////////////////////////////////////////
        //////                   CONSTRUCTOR                //////
        //////////////////////////////////////////////////////////

        public MainWindow()
        {
            DataContext = new MainViewModel(RenderComponent);

            SetScreenSize();
            InitializeComponent();
            //FetchOnStartup();
        }


        //////////////////////////////////////////////////////////
        //////                  XAMLFUNCTIONS               //////
        //////////////////////////////////////////////////////////

        private void FetchOnStartup()
        {
            // check for db
            // if not exists create new one
            // start subtask to notify user about that

            // fetch entries from db and render them / if there is anything to fetch / also notify user about that
            // if db did not exist -> start tutorial 
        }

        private void InvokePreferences(object sender, RoutedEventArgs e)
        {
            var preferencesWindow = new Preferences();
            preferencesWindow.Show();
        }

        private void closeApplication(object sender, RoutedEventArgs e)
        {
            var mainViewModel = DataContext as MainViewModel;
            if (mainViewModel?.PerformAutoSave() == true)
            {
                if (App.Current.Windows[1].IsActive)
                {
                    App.Current.Windows[1].Close();
                }

                this.Close();
            }
            else
            {
                InvokeWarningPopup("Could not perform autosave", "Could not save your latest changes", ForceClose);
            }
        }

        private void InvokeWarningPopup(string errorHeadline, string errorDetails, Action callback)
        {
            Warning_Popup.IsOpen = true;
            Error_Head.Content = errorHeadline;
            Error_Desc.Text = errorDetails;

            // TODO: find way to pass functions as Click events generically
        }

        private void ForceClose()
        {

        }

        private void minimizeApplication(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void moveWindow(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void InvokeTargetChooser(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog targetChooser = new CommonOpenFileDialog();
            targetChooser.InitialDirectory = @"C:\users";
            targetChooser.Title = "Choose Import Target";
            targetChooser.IsFolderPicker = true;
            targetChooser.Multiselect = false;

            if (targetChooser.ShowDialog() == CommonFileDialogResult.Ok)
            {
                if(targetChooser.FileName != "Default" && 
                    !targetChooser.FileName.Contains("Windows"))
                {
                    if (Directory.Exists(targetChooser.FileName))
                    {
                        Import_Target.Text = targetChooser.FileName;
                        Import_Confirm_Btn.IsEnabled = true;
                    }
                    else
                    {
                        Import_Target.Text = "Invalid location.";
                    }
                } else
                {
                    Import_Target.Text = "Not able to import from this location.";
                }
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
                PassFilesToBeSorted(Import_Target.Text);
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

        public bool HandlePopup(object obj)
        {
            return !Import_Popup.IsOpen;
        }

        private void PassFilesToBeSorted(string target)
        {
            var mainViewModel = DataContext as MainViewModel;
            RenderComponent(mainViewModel?.Import(Import_Target.Text));
        }

        void RenderComponent(StackPanel sp)
        {
            directoryControl.Items.Add(sp);
        }

        void SetScreenSize()
        {
            //set width and height according to system specs
            //maybe also adjust text size
        }
    }
}
