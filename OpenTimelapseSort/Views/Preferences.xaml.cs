using OpentimelapseSort.Models;
using OpenTimelapseSort.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace OpenTimelapseSort.Views
{
    public partial class PreferencesView : Window
    {
        private readonly PreferencesViewModel _preferencesViewModel = new PreferencesViewModel();

        public PreferencesView()
        {
            InitializeComponent();
            SetPreferences(_preferencesViewModel.FetchFromDatabase());
        }

        // TODO: simplify

        private void UpdateSliderCountValue(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var slider = (Slider)sender;
            var value = (int)Math.Round(slider.Value, 0);
            IntervalCount.Content = value;
        }

        private void UpdateSliderIntervalValue(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var slider = (Slider)sender;
            var value = Math.Round(slider.Value, 2);
            Interval.Content = value;
        }

        private void UpdateGenerosityValue(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var slider = (Slider)sender;
            var value = Math.Round(slider.Value, 2);
            Generosity.Content = value + " %";
        }

        private void SetPreferences(Preferences preferences)
        {
            IntervalCount.Content = preferences.SequenceImageCount;
            IntervalCountSlider.Value = preferences.SequenceImageCount;

            Interval.Content = preferences.SequenceInterval;
            IntervalSlider.Value = preferences.SequenceInterval;

            Generosity.Content = preferences.SequenceIntervalGenerosity;
            GenerositySlider.Value = preferences.SequenceIntervalGenerosity;

            Copy.IsChecked = preferences.UseCopy;
            Auto_Detection_btn.IsChecked = preferences.UseAutoDetectInterval;
        }

        public void SavePreferences(object sender, RoutedEventArgs e)
        {
            _preferencesViewModel.SavePreferences((bool)Auto_Detection_btn.IsChecked, (bool)Copy.IsChecked,
                IntervalSlider.Value, (int)GenerositySlider.Value, (int)IntervalCountSlider.Value);
        }

        private void closePreferencesWindow(object sender, RoutedEventArgs e)
        {
            App.Current.Windows[1].Close();
        }

        private void MoveWindow(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
