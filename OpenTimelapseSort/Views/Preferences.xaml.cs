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

        private void ClosePreferencesWindow(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void MoveWindow(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
