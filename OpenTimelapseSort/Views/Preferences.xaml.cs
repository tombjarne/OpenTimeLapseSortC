using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using OpentimelapseSort.Models;
using OpenTimelapseSort.ViewModels;

namespace OpenTimelapseSort.Views
{
    public partial class PreferencesView : Window
    {

        public delegate double SliderCountValueCommand(double value);
        PreferencesViewModel preferencesViewModel = new PreferencesViewModel();

        public PreferencesView()
        {
            InitializeComponent();
            SetPreferences(preferencesViewModel.FetchFromDatabase());
        }

        // TODO: simplify

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

        void UpdateGenerosityValue(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider slider = (Slider)sender;
            double value = Math.Round(slider.Value, 2);
            this.Generosity.Content = value + " %";
        }

        private void SetPreferences(Preferences preferences)
        {
            IntervalCount.Content = preferences.sequenceImageCount.ToString();
            IntervalCountSlider.Value = preferences.sequenceImageCount;

            Interval.Content = preferences.sequenceInterval.ToString();
            IntervalSlider.Value = preferences.sequenceInterval;

            Generosity.Content = preferences.sequenceIntervalGenerosity.ToString();
            GenerositySlider.Value = preferences.sequenceIntervalGenerosity;

            Copy.IsChecked = preferences.useCopy;
        }

        public void SavePreferences(object sender, RoutedEventArgs e)
        {
            preferencesViewModel.SavePreferences(true, (bool)Copy.IsChecked, 
                (double)IntervalSlider.Value, (int)GenerositySlider.Value, (int)IntervalCountSlider.Value);
        }

        private void closePreferencesWindow(object sender, RoutedEventArgs e)
        {
            App.Current.Windows[1].Close();
        }

        private void moveWindow(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void Copy_Checked(object sender, RoutedEventArgs e)
        {

        }
    }
}
