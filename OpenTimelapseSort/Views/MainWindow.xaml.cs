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

        /// <summary>
        ///     InvokeStartupScreen()
        ///     creates a new instance of <see cref="StartupScreen" /> and opens it
        /// </summary>
        private static void InvokeStartupScreen()
        {
            var startupScreen = new StartupScreen();
            startupScreen.Show();
        }


        /// <summary>
        ///     InvokePreferences()
        ///     creates a new instance of <see cref="PreferencesView" /> and opens it on button click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InvokePreferences(object sender, RoutedEventArgs e)
        {
            var preferencesWindow = new PreferencesView();
            preferencesWindow.Show();
        }

        /// <summary>
        ///     InvokeTutorial()
        ///     creates a new instance of <see cref="PreferencesView" /> and opens it on button click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InvokeTutorial(object sender, RoutedEventArgs e)
        {
            var tutorial = new Tutorial();
            tutorial.Show();
        }
        
        /// <summary>
        ///     CloseApplication()
        ///     closes the application on button click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseApplication(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        /// <summary>
        ///     MinimizeApplication()
        ///     minimizes the application on button click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MinimizeApplication(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        /// <summary>
        ///     MaximizeApplication
        ///     maximizes the application on button click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MaximizeApplication(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        /// <summary>
        ///     MoveWindow()
        ///     handles window movement on mouse drag on top navigation bar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MoveWindow(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        /// <summary>
        ///     ImageViewer_OnPreviewMouseDown()
        ///     handles scrolling of <see cref="ImageViewer" /> scroll view
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImageViewer_OnPreviewMouseDown(object sender, MouseWheelEventArgs e)
        {
            var scv = (ScrollViewer) sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
        }

        /// <summary>
        ///     DirectoryViewer_OnPreviewMouseDown()
        ///     handles scrolling of <see cref="DirectoryViewer" /> scroll viewer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DirectoryViewer_OnPreviewMouseDown(object sender, MouseWheelEventArgs e)
        {
            var scv = (ScrollViewer) sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
        }

        /// <summary>
        ///     InvokeChooser()
        ///     invokes a file chooser to determine the import destination
        /// </summary>
        /// <param name="sender"></param>
        private void InvokeChooser(object sender, RoutedEventArgs e)
        {
            var type = (Button) sender;
            var dialog = type.Name == "Target" ? _fileTargetDialog : _fileOriginDialog;
            var path = "";

            ImportPopup.IsOpen = false;

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                if (SelectionMatchesRequirements(dialog))
                {
                    path = dialog.FileName;
                    ImportPopup.IsOpen = true;
                }
            }
            else
            {
                ImportPopup.IsOpen = true;
            }

            if (type.Name == "Target")
                ((MainViewModel) DataContext).SetImportTarget(path);
            else
                ((MainViewModel) DataContext).SetImportOrigin(path);
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
    }
}