using System.Windows;
using System.Windows.Input;

namespace OpenTimelapseSort.Views
{
    public partial class PreferencesView
    {
        public PreferencesView()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     ClosesWindow()
        ///     closes the preferences window on button click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            Close();
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
