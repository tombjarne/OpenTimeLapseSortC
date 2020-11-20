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

namespace WpfApp1
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
        //////                   CONSTRUCTOR                //////
        //////////////////////////////////////////////////////////

        public MainWindow()
        {
            InitializeComponent();
            this.renderDirectories();
        }


        //////////////////////////////////////////////////////////
        //////                  XAMLFUNCTIONS               //////
        //////////////////////////////////////////////////////////

        /**
        * menu1_Click
        *
        * fetches currently set Preferences from database and updates UI
        */

        private void menu1_Click(object sender, RoutedEventArgs e)
        {

        }


        //////////////////////////////////////////////////////////
        //////                    FUNCTIONS                 //////
        //////////////////////////////////////////////////////////

        /**
        * renderDirectories
        *
        * fetches currently set Preferences from database and updates UI
        */

        void renderDirectories()
        {
            //this could be any large object, imagine a diagram...though for this example im just using loads
            //of Rectangles
            itemsControl.Items.Add(CreateStackPanel(Brushes.Salmon));

        }

        // create elements
        // this function needs to be called from service to pass directory length and directory
        // to render the directory
        private StackPanel CreateStackPanel(SolidColorBrush color)
        {

            StackPanel sp = new StackPanel();
            sp.Orientation = Orientation.Vertical;

            for (int i = 0; i < 5; i++)
            {
                Rectangle rect = new Rectangle();
                rect.Width = 100;
                rect.Height = 100;
                rect.Margin = new Thickness(5);
                rect.Fill = i % 2 == 0 ? Brushes.Black : color;
                sp.Children.Add(rect);
            }
            return sp;
        }

        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            if (ScrollViewer.IsMouseOver)
            {
                // Save starting point, used later when determining
                //how much to scroll.
                start = e.GetPosition(this);
                startOffset.X = ScrollViewer.HorizontalOffset;
                startOffset.Y = ScrollViewer.VerticalOffset;

                // Update the cursor if can scroll or not.
                this.Cursor = (ScrollViewer.ExtentWidth >
                    ScrollViewer.ViewportWidth) ||
                    (ScrollViewer.ExtentHeight >
                    ScrollViewer.ViewportHeight) ?
                    Cursors.ScrollAll : Cursors.Arrow;

                this.CaptureMouse();
            }
            base.OnPreviewMouseDown(e);
        }


        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {
            if (this.IsMouseCaptured)
            {
                // Get the new scroll position.
                Point point = e.GetPosition(this);

                // Determine the new amount to scroll.

                double x = (point.X > this.start.X) ?
                        -(point.X - this.start.X) :
                        (this.start.X - point.X);

                double y = (point.Y > this.start.Y) ?
                        -(point.Y - this.start.Y) :
                        (this.start.Y - point.Y);

                Point delta = new Point(x, y);

                // Scroll to the new position.
                ScrollViewer.ScrollToHorizontalOffset(
                    this.startOffset.X + delta.X);
                ScrollViewer.ScrollToVerticalOffset(
                    this.startOffset.Y + delta.Y);
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
