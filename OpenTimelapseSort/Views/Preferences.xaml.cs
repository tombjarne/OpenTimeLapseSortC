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
using OpenTimelapseSort.Mvvm;

namespace OpenTimelapseSort.Views
{
    /// <summary>
    /// Interaktionslogik für Preferences.xaml
    /// </summary>
    public partial class Preferences : Window
    {

        public delegate double SliderCountValueCommand(double value);
        PreferencesViewModel pvm = new PreferencesViewModel();

        public Preferences()
        {
            InitializeComponent();
            FetchOnStartup();
        }

        private void FetchOnStartup()
        {
            SetImageSequence(pvm.FetchFromDatabase().sequenceInterval);
            SetImageSequenceCount(pvm.FetchFromDatabase().sequenceImageCount);
            SetCopyEnabled(pvm.FetchFromDatabase().useCopy);
        }

        void UpdateSliderCountValue(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider slider = (Slider)sender;
            int value = (int)Math.Round(slider.Value, 0);
            this.IntervalCount.Content = value;
        }

        void UpdateSliderIntervalValue(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider slider = (Slider)sender;
            double value = Math.Round(slider.Value, 2);
            this.Interval.Content = value;
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

        public void SavePreferences(object sender, RoutedEventArgs e)
        {
            pvm.SavePreferences(true, (bool)Copy.IsChecked, (double)IntervalSlider.Value, (int)IntervalCountSlider.Value);
        }

        private void minimizeApplication(object sender, RoutedEventArgs e)
        {
            App.Current.Windows[1].Close();
        }

        private void moveWindow(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
