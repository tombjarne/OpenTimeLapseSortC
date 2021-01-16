﻿using System.Windows;
using System.Windows.Input;

namespace OpenTimelapseSort.Views
{
    /// <summary>
    /// Interaktionslogik für StartupScreen.xaml
    /// </summary>
    public partial class StartupScreen : Window
    {
        public StartupScreen()
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();
        }

        private void minimizeApplication(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void MoveWindow(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void closeApplication(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
