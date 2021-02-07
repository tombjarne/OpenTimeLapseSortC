using System.Windows;

namespace OpenTimelapseSort.Views
{
    public partial class StartupScreen
    {
        public StartupScreen()
        {
            InitializeComponent();
            StartupActions();
        }

        /// <summary>
        ///     StartupActions()
        ///     sets the window centered on the screen
        ///     sets the window as topmost
        /// </summary>
        private void StartupActions()
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            Topmost = true;
        }

        /// <summary>
        ///     ClosesWindow()
        ///     closes the startup screen on button click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        ///     InvokeTutorialScreen()
        ///     invokes a new tutorial screen on button click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InvokeTutorialScreen(object sender, RoutedEventArgs e)
        {
            var tutorialWindow = new Tutorial();
            tutorialWindow.Show();
        }
    }
}