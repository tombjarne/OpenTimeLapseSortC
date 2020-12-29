using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.WindowsAPICodePack.Dialogs;

using OpenTimelapseSort.ViewModels;
using OpenTimelapseSort.Mvvm;
using System;
using System.IO;
using System.ComponentModel;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace OpenTimelapseSort.Views
{
    public partial class MainWindow : Window
    {

        //////////////////////////////////////////////////////////
        //////                    VARIABLES                 //////
        //////////////////////////////////////////////////////////

        //private delegate void WarningReference(string errorHeadline, string errorDetails);


        ObservableCollection<ImageDirectory> directories = new ObservableCollection<ImageDirectory>();
        MainViewModel mainViewModel = new MainViewModel();


        //////////////////////////////////////////////////////////
        //////                   CONSTRUCTOR                //////
        //////////////////////////////////////////////////////////

        public MainWindow()
        {
            InitializeComponent();
            //this.DataContext = new MainViewModel(RenderComponent);
            this.DataContext = new MainViewModel();
            DataContext = this;
            SetScreenSize();
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
                this.Close();
                //InvokeWarningPopup("Could not perform autosave", "Could not save your latest changes", ForceClose);
            }
        }

        private ICommand GetDelegateCommand(Action<object> callback)
        {

            var errorCommand = new DelegateCommand(callback);
            return errorCommand;
        }

        private void InvokeWarningPopup(string errorHeadline, string errorDetails, Action<object> callback)
        {
            Warning_Popup.IsOpen = true;
            Error_Head.Content = errorHeadline;
            Error_Desc.Text = errorDetails;

            //Error_Btn.Click += new RoutedEventHandler(callback);

            // TODO: find way to pass functions as Click events generically
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

                Import_Progress_Popup.IsOpen = true;

                //var mainViewModel = DataContext as MainViewModel;

                mainViewModel.Import(Import_Target.Text, HandleListingProgress);

                //Task listImagesTask = ListImages();
                //listImagesTask.ContinueWith(HandleListingProgress);
            }
            else
            {
                Import_Confirm_Btn.IsEnabled = false;
                Import_Target.Text = "Location unreachable, did you delete something?";
            }
        }

        /*
        private Task ListImages()
        {
            var mainViewModel = DataContext as MainViewModel;
            return Task.Run(() => 
            {
                mainViewModel?.Import(Import_Target.Text, HandleListingProgress);
                Debug.WriteLine("End reached");
            });
        }
        */

        //////////////////////////////////////////////////////////
        //////                    FUNCTIONS                 //////
        //////////////////////////////////////////////////////////

        private void HandleListingProgress(int count, List<Image> imageList)
        {
            // save returned number of found files
            // update view after all images have been found

            Debug.WriteLine("reached");
            Debug.WriteLine(imageList.Count);

            Import_Progress_Btn.IsEnabled = true;
            Import_Progress_Count.Text = "Found "+count+" images";

            mainViewModel.SortImages(imageList, RenderComponent);
        }

        void RenderComponent(StackPanel sp)
        {
            Debug.WriteLine("countofstackpanels");
            directoryControl.Items.Add(sp);
        }

        void SetScreenSize()
        {
            //set width and height according to system specs
            //maybe also adjust text size
        }
    }
}
