using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Xceed.Wpf.Toolkit;

namespace DLA
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Color myTreeColor = Colors.Green;
        private Color mySeedColor = Colors.Blue;
        private Color myBackgroundColor = Colors.Black;
        private DispatcherTimer timer;
        private Map DLAMap;

        // properties
        private Color TreeColor
        {
            get { return myTreeColor; }
            set { myTreeColor = value; }
        }
        private Color SeedColor
        {
            get { return mySeedColor; }
            set { mySeedColor = value; }
        }
        private Color BackgroundColor
        {
            get { return myBackgroundColor; }
            set { myBackgroundColor = value; }
        }
        private int Speed { get; set; }

        // constructor
        public MainWindow()
        {
            InitializeComponent();
        }

        // draws map
        private void InitializeMap(SeedKind seedKind)
        {
            // stop the timer
            if (timer != null)
                timer.Stop();

            // change button content
            startBtn.Content = "Start";

            // get drawing speed
            Speed = (int)Math.Round(speedSlider.Value, 0);

            // default colors
            drawingSurface.SeedColor = SeedColor;
            drawingSurface.TreeColor = TreeColor;
            drawingSurface.BackgroundColor = BackgroundColor;
            drawingSurface.Background = new SolidColorBrush(BackgroundColor);

            // new map
            DLAMap = new Map((int)mapSizeSlider.Value, (int)mapSizeSlider.Value);
            DLAMap.Density = (short)particleDensitySlider.Value;
            Map.Step = 0;

            // set tree seed
            DLAMap.SetTreeSeed(seedKind);

            // set step value
            stepLabel.Content = Map.Step;

            // place map on the canvas
            drawingSurface.SetMap(ref DLAMap);

            // set seed particles number
            seedParticlesNumber.Content = DLAMap.SeedParticlesNumber;
        }

        // timer initial settings
        private void timerInitialize()
        {
            timer = new DispatcherTimer();
            timer.Tick += new EventHandler(Step);
            timer.Interval = new TimeSpan(0, 0, 0, 0, 1);
            timer.Start();
        }

        private void SingleStep()
        {
            DLAMap.MoveParticles();

            // update displayed step information
            ++Map.Step;
            stepLabel.Content = Map.Step.ToString();

            // gets seed particle numbers and display it
            // stops the programm, when there is no more seed left
            int seedLeft = DLAMap.SeedParticlesNumber;
            seedParticlesNumber.Content = seedLeft.ToString();
        }

        // redraws screen
        internal void Redraw()
        {
            drawingSurface.InvalidateVisual();
        }

        // start animation
        internal void Animate()
        {
            if (timer == null || timer.IsEnabled != true)
                timerInitialize();
        }

        // pause animation
        internal void Pause()
        {
            if (timer != null)
                timer.Stop();
        }

        // one game step
        private void Step(object sender, EventArgs e)
        {
            // actualization of speed in case it has been changed after pressing start button
            Speed = (int)Math.Round(speedSlider.Value, 0);

            // calculate how often draw a map
            int step = 0;
            while (step < Speed)
            {
                if (DLAMap.SeedParticlesNumber == 0)
                {
                    Pause();
                    Redraw();
                    return;
                }
                SingleStep();
                ++step;
            }
            Redraw();
        }

        // on initialization of the right panel containing map
        private void MapReset(object sender, EventArgs e)
        {
            string buttonCheckedName = this.FindDescendants<RadioButton>(r => (bool)r.IsChecked).FirstOrDefault().Name.ToString();
            SeedKind seedKind = (SeedKind)Enum.Parse(typeof(SeedKind), buttonCheckedName);
            InitializeMap(seedKind);
        }

        // start button click event
        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            if (startBtn.Content.ToString() == "Start")
            {
                Animate();
                startBtn.Content = "Pause";
            }
            else
            {
                Pause();
                startBtn.Content = "Start";
            }
        }

        // set color of the seed, tree or background
        private void SelectedColor_Changed(object sender, RoutedPropertyChangedEventArgs<Color> e)
        {
            if (drawingSurface != null)
            {
                switch ((sender as ColorPicker).Name.ToString())
                {
                    case "colorPicker1":
                        {
                            TreeColor = e.NewValue;
                            drawingSurface.TreeColor = TreeColor;
                            Redraw();
                            break;
                        }
                    case "colorPicker2":
                        {
                            SeedColor = e.NewValue;
                            drawingSurface.SeedColor = SeedColor;
                            Redraw();
                            break;
                        }
                    case "colorPicker3":
                        {
                            BackgroundColor = e.NewValue;
                            drawingSurface.Background = new SolidColorBrush(BackgroundColor);
                            drawingSurface.BackgroundColor = BackgroundColor;
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
            }
        }

        // change image height, width after window resize
        private void WindowSize_Changed(object sender, SizeChangedEventArgs e)
        {
            Image DLAImage = System.Windows.LogicalTreeHelper.FindLogicalNode(drawingSurface, "DLAImage") as Image;
            if (DLAImage != null)
            {
                DLAImage.Height = drawingSurface.ActualHeight;
                DLAImage.Width = drawingSurface.ActualWidth;
                drawingSurface.SetImage();
            }

        }

        // make single step when single step button has been pressed
        private void SingleStepButton_Click(object sender, RoutedEventArgs e)
        {
            SingleStep();
            Redraw();
            Pause();
        }

        // choose action for the chosen radio button
        private void RadioButton_Click(object sender, RoutedEventArgs e)
        {
            switch ((sender as RadioButton).Name.ToString())
            {
                case "PointSeed":
                    {
                        InitializeMap(SeedKind.PointSeed);
                        break;
                    }
                case "LineSeed":
                    {
                        InitializeMap(SeedKind.LineSeed);
                        break;
                    }
                case "CircleSeed":
                    {
                        InitializeMap(SeedKind.CircleSeed);
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }

    }
}
