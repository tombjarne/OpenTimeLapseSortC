using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Xps.Packaging;

namespace OpenTimelapseSort.Views
{
    /// <summary>
    /// Interaktionslogik für Tutorial.xaml
    /// </summary>
    public partial class Tutorial
    {
        public Tutorial()
        {
            InitializeComponent();
            StartupActions();
        }

        private void StartupActions()
        {
            var path = Path.GetFullPath("..\\OpenTimelapseSort\\Resources\\pdf\\documentation.xps");

            var dlg = new XpsDocument(path, FileAccess.Read);
            DocumentViewer.Document = dlg.GetFixedDocumentSequence();
        }

        private void CloseApplication(object sender, RoutedEventArgs e)
        {
            Close();
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

        private void MoveWindow(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
