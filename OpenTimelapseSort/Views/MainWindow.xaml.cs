using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.WindowsAPICodePack.Dialogs;
using OpenTimelapseSort.ViewModels;

namespace OpenTimelapseSort.Views
{
    public partial class MainWindow
    {
        // TODO: fix
        private readonly CommonOpenFileDialog _fileOriginDialog;
        private readonly CommonOpenFileDialog _fileTargetDialog;

        public MainWindow()
        {
            InvokeStartupScreen();
            InitializeComponent();

            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            _fileTargetDialog = new CommonOpenFileDialog
            {
                InitialDirectory = @"C:\users",
                Title = "Choose Import Target",
                IsFolderPicker = true,
                Multiselect = false
            };

            _fileOriginDialog = new CommonOpenFileDialog
            {
                InitialDirectory = @"C:\users",
                Title = "Choose Import Origin",
                IsFolderPicker = true,
                Multiselect = false
            };
        }

        private static void InvokeStartupScreen()
        {
            var startupScreen = new StartupScreen();
                startupScreen.Show();
        }

        // handles invocation of preferences window

        private void InvokePreferences(object sender, RoutedEventArgs e)
        {
            var preferencesWindow = new PreferencesView();
            preferencesWindow.Show();
        }

        // handles closing of application

        private void CloseApplication(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        // handles minimization of window

        private void MinimizeApplication(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void MaximizeApplication(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        // handles drag of window

        private void MoveWindow(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void ImageViewer_OnPreviewMouseDown(object sender, MouseWheelEventArgs e)
        {
            var scv = (ScrollViewer) sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
        }

        private void DirectoryViewer_OnPreviewMouseDown(object sender, MouseWheelEventArgs e)
        {
            var scv = (ScrollViewer) sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
        }

        /// <summary>
        ///     InvokeTargetChooser()
        /// </summary>
        /// <param name="sender"></param>
        private void InvokeTargetChooser(object sender, RoutedEventArgs e)
        {
            var importTargetPath = "";
            ImportPopup.IsOpen = false;

            // TODO: need to replace with newer filedialog! Fails when canceled before
            if (_fileTargetDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                if(SelectionMatchesRequirements(_fileTargetDialog))
                {
                    importTargetPath = _fileTargetDialog.FileName;
                    ImportPopup.IsOpen = true;
                }
            }
            else
            {
                ImportPopup.IsOpen = true;
            }

            ((MainViewModel) DataContext).SetImportTarget(importTargetPath);
        }

        /// <summary>
        ///     InvokeTargetChooser()
        /// </summary>
        /// <param name="sender"></param>
        private void InvokeOriginChooser(object sender, RoutedEventArgs e)
        {
            var importOriginPath = "";
            ImportPopup.IsOpen = false;

            if (_fileOriginDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                if (SelectionMatchesRequirements(_fileOriginDialog))
                {
                    importOriginPath = _fileOriginDialog.FileName;
                    ImportPopup.IsOpen = true;
                }
            }
            else
            {
                ImportPopup.IsOpen = true;
            }


            ((MainViewModel) DataContext).SetImportOrigin(importOriginPath);
        }

        /// <summary>
        ///     SelectionMatchesRequirements()
        ///     Determines whether the local file chooser wants to import from an allowed location
        ///     Forbids the import from any directory that contains "Windows" or "Default"
        ///     The selected directory must match an actual existing file path
        /// </summary>
        /// <returns></returns>
        private static bool SelectionMatchesRequirements(CommonOpenFileDialog fileDialog)
        {
            return fileDialog.FileName != "Default" &&
                   !fileDialog.FileName.Contains("Windows") &&
                   Directory.Exists(fileDialog.FileName);
        }

        private void DirectoryViewer1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}