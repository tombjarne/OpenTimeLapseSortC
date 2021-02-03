using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace OpenTimelapseSort.Views
{
    /// <summary>
    /// Interaktionslogik für StartupScreen.xaml
    /// </summary>
    public partial class StartupScreen
    {
        public StartupScreen()
        {
            InitializeComponent();
            StartupActions();
        }

        private void StartupActions()
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            Topmost = true;
            //BackgroundVideo.Play();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void InvokeTutorialScreen(object sender, RoutedEventArgs e)
        {
            var tutorialWindow = new Tutorial();
            tutorialWindow.Show();
        }
    }
}
