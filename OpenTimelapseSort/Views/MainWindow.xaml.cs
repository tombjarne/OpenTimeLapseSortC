using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.WindowsAPICodePack.Dialogs;

using OpenTimelapseSort.ViewModels;

namespace OpenTimelapseSort.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        //////////////////////////////////////////////////////////
        //////                    VARIABLES                 //////
        //////////////////////////////////////////////////////////

        private Point start;
        private Point startOffset;

        //////////////////////////////////////////////////////////
        //////                    VARIABLES                 //////
        //////////////////////////////////////////////////////////

        /// Bindings


        //////////////////////////////////////////////////////////
        //////                   CONSTRUCTOR                //////
        //////////////////////////////////////////////////////////

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new MainViewModel();
        }


        //////////////////////////////////////////////////////////
        //////                  XAMLFUNCTIONS               //////
        //////////////////////////////////////////////////////////

        private void invokePreferences(object sender, RoutedEventArgs e)
        {
            var preferencesWindow = new Preferences();
            preferencesWindow.Show();
        }

        private void import(object sender, RoutedEventArgs e)
        {
            // TODO: start new task to display progress
            // Task ImportVisualizer = new Task();
            RenderComponent(MainViewModel.Import());
            RenderImports();
        }

        private void RenderImports()
        {
            MainViewModel vm = new MainViewModel();
            StackPanel directoryPanel = vm.InitialiseView();
            //StackPanel directoryPanel = new StackPanel();
            /*
            for (int i = 0; i < 5; i++)
            {
                Rectangle rect2 = new Rectangle();
                rect2.Width = 400;
                rect2.Height = 100;
                rect2.Margin = new Thickness(5);
                rect2.Fill = Brushes.HotPink;
                directoryPanel.Children.Add(rect2);
            }
            */

            RenderComponent(directoryPanel);
        }


        //////////////////////////////////////////////////////////
        //////                    FUNCTIONS                 //////
        //////////////////////////////////////////////////////////


        private void closeApplication(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void minimizeApplication(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void moveWindow(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }


        /**
        * renderDirectories
        *
        * fetches currently set Preferences from database and updates UI
        */

        void RenderComponent(StackPanel sp)
        {
            directoryControl.Items.Add(sp);
        }

        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            if (DirectoryViewer.IsMouseOver)
            {
                bool extent = DirectoryViewer.ExtentWidth > DirectoryViewer.ViewportWidth || DirectoryViewer.ExtentHeight > DirectoryViewer.ViewportHeight;

                start = e.GetPosition(this);
                startOffset.X = DirectoryViewer.HorizontalOffset;
                startOffset.Y = DirectoryViewer.VerticalOffset;

                this.Cursor = extent ? Cursors.ScrollAll : Cursors.Arrow;
                this.CaptureMouse();
            }
            base.OnPreviewMouseDown(e);
        }


        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {
            if (this.IsMouseCaptured)
            {
                Point point = e.GetPosition(this);
                Point delta;

                double x = (point.X > this.start.X) ? -(point.X - this.start.X) : (this.start.X - point.X);
                double y = (point.Y > this.start.Y) ? -(point.Y - this.start.Y) : (this.start.Y - point.Y);

                delta = new Point(x, y);

                DirectoryViewer.ScrollToHorizontalOffset(this.startOffset.X + delta.X);
                DirectoryViewer.ScrollToVerticalOffset(this.startOffset.Y + delta.Y);
            }

            base.OnPreviewMouseMove(e);
        }


        protected override void OnPreviewMouseUp(
            MouseButtonEventArgs e)
        {
            if (this.IsMouseCaptured)
            {
                this.Cursor = Cursors.Arrow;
                this.ReleaseMouseCapture();
            }

            base.OnPreviewMouseUp(e);
        }
    }
}
