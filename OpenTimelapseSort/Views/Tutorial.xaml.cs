using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Xps.Packaging;

namespace OpenTimelapseSort.Views
{
    public partial class Tutorial
    {
        public Tutorial()
        {
            InitializeComponent();
            StartupActions();
        }

        /// <summary>
        /// StartupActions()
        /// sets the proper document for <see cref="DocumentViewer"/>
        /// </summary>

        private void StartupActions()
        {
            var path = 
                Path.GetFullPath("..\\OpenTimelapseSort\\Resources\\xps\\opentimelapsesort-documentation.xps");

            var dlg = new XpsDocument(path, FileAccess.Read);
            DocumentViewer.Document = dlg.GetFixedDocumentSequence();
        }

        /// <summary>
        ///     CloseWindow()
        ///     closes the tutorial window on button click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            Close();
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
    }
}
