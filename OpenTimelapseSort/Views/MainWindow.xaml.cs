using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.WindowsAPICodePack.Dialogs;

using OpenTimelapseSort.ViewModels;
using OpenTimelapseSort.Mvvm;
using System;

namespace OpenTimelapseSort.Views
{
    public partial class MainWindow : Window 
    {

        //////////////////////////////////////////////////////////
        //////                    VARIABLES                 //////
        //////////////////////////////////////////////////////////

        //public ICommand PrintCommand => new DelegateCommand(HandlePopup);

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
            this.Close();
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
            // make invokeable only once
            CommonOpenFileDialog targetChooser = new CommonOpenFileDialog();
            targetChooser.InitialDirectory = @"C:\";
            targetChooser.Title = "Choose Import Target";
            targetChooser.IsFolderPicker = true;
            targetChooser.Multiselect = false;

            //TODO: find proper way of returning a stackpanel on else path

            if (targetChooser.ShowDialog() == CommonFileDialogResult.Ok)
            {
                Import_Target.Text = targetChooser.FileName;
            }
        }

        // sets popup to visible or invisible
        private void ChooseImportTarget(object sender, RoutedEventArgs e)
        { 
            //Import_Popup.IsOpen = false;
        }

        private void ConfirmImportSettings(object sender, RoutedEventArgs e)
        {
            InvokeFileChooser(Import_Target.Text);
        }

        //////////////////////////////////////////////////////////
        //////                    FUNCTIONS                 //////
        //////////////////////////////////////////////////////////

        public bool HandlePopup(object obj)
        {
            return !Import_Popup.IsOpen;
        }

        private void InvokeFileChooser(string target)
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
