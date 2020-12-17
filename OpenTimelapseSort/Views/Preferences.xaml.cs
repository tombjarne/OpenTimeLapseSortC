using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using OpenTimelapseSort.ViewModels;

namespace OpenTimelapseSort.Views
{
    /// <summary>
    /// Interaktionslogik für Preferences.xaml
    /// </summary>
    public partial class Preferences : Window
    {
        public Preferences()
        {
            InitializeComponent();
            this.DataContext = new PreferencesViewModel();
            FetchOnStartup();
        }

        private void FetchOnStartup()
        {
            PreferencesViewModel pvm = new PreferencesViewModel();
            // TODO: finish and make pretty
            SetImageSequence(pvm.FetchFromDatabase().sequenceInterval);
            SetImageSequenceCount(pvm.FetchFromDatabase().sequenceImageCount);
            SetCopyEnabled(pvm.FetchFromDatabase().useCopy);
        }

        void SetImageSequence(double value)
        {
            Interval.Content = value;
            IntervalSlider.Value = value;
        } 
        
        void SetImageSequenceCount(double value)
        {
            IntervalCount.Content = value;
            IntervalCountSlider.Value = value;
        }

        void SetCopyEnabled(bool CopyIsEnabled)
        {
            Copy.IsChecked = CopyIsEnabled;
        }

        private void minimizeApplication(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void moveWindow(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {

        }
    }
}
