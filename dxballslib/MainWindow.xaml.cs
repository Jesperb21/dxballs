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
using System.Timers;
using System.Windows.Threading;

namespace dxballslib
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DispatcherTimer mainLoop = new DispatcherTimer();
        private ContentControl playerBlock = new ContentControl();
        private ContentControl ball1 = new ContentControl();
        private int xdirection = 1;
        private int ydirection = 1;
        private double ballspeed = 6;
        private double xspeed;
        private double yspeed;
        public MainWindow()
        {
            InitializeComponent();
            //startGameUp();
        }

        public void startGameUp()
        {
            playerBlock.Template = Resources["playerTemplate"] as ControlTemplate;
            playArea.Children.Add(playerBlock);
            Canvas.SetBottom(playerBlock, 10);
            Canvas.SetLeft(playerBlock, (playArea.ActualWidth / 2) - 25);
            ball1.Template = Resources["ballTemplate"] as ControlTemplate;
            playArea.Children.Add(ball1);
            Canvas.SetBottom(ball1, (playArea.ActualHeight/2));
            Canvas.SetLeft(ball1, (playArea.ActualWidth / 2) - 7.5);
            xspeed = ballspeed / 2;
            yspeed = ballspeed / 2;
            startButton.Visibility = Visibility.Hidden;
            
            mainLoop.Interval = TimeSpan.FromMilliseconds(20); //50 per second
            mainLoop.Tick += mainLoop_Tick;
            mainLoop.Start();
        }

        void mainLoop_Tick(object sender, EventArgs e)//bliver kørt 50 gange i sekundet
        {
            //player controls
            if (Keyboard.IsKeyDown(Key.Left))
            {
                Canvas.SetLeft(playerBlock, (Canvas.GetLeft(playerBlock) - 5));//5 er playerens hastighed
            }
            if (Keyboard.IsKeyDown(Key.Right))
            {
                Canvas.SetLeft(playerBlock, (Canvas.GetLeft(playerBlock) + 5));
            }
            //ball controls
            if (Canvas.GetLeft(ball1) >= (playArea.ActualWidth-ball1.ActualWidth))
            {
                xdirection = -1;
            }
            if(Canvas.GetLeft(ball1) <= 0)
            {
                xdirection = 1;
            }
            if (Canvas.GetBottom(ball1) <= 0)//ball hits buttom
            {
                ydirection = -1;
            }
            if (Canvas.GetBottom(ball1) >= playArea.ActualHeight)
            {
                ydirection = 1;
            }
            if (Canvas.GetBottom(ball1) <= 10 + playerBlock.ActualHeight && Canvas.GetBottom(ball1) >= 10)//ball player hittest
            {
                if((Canvas.GetLeft(ball1) >= Canvas.GetLeft(playerBlock)) && (Canvas.GetLeft(ball1) <= Canvas.GetLeft(playerBlock)+playerBlock.ActualWidth))
                {
                    double v1 = ((Canvas.GetLeft(ball1) + ball1.ActualWidth / 2) - Canvas.GetLeft(playerBlock));
                    double v2 = ((Canvas.GetLeft(playerBlock) + playerBlock.ActualWidth / 2) - Canvas.GetLeft(playerBlock));
                    v1 -= v2;
                    xspeed = v1 / v2;
                    xspeed *= ballspeed;
                    if (xspeed < 0)
                    {//left side of the player block
                        xspeed = -xspeed;
                        xdirection = -1;
                    }
                    else
                    {
                        xdirection = 1;
                    }
                    if (xspeed > ballspeed - 0.5)
                    {
                        xspeed = ballspeed-0.5;
                    }
                    yspeed = ballspeed - xspeed;
                    if (yspeed < 0)
                    {
                        yspeed = -yspeed;
                    }
                    ydirection = -1;
                }
            }
            Canvas.SetBottom(ball1, (Canvas.GetBottom(ball1) - (yspeed * ydirection)));
            Canvas.SetLeft(ball1, (Canvas.GetLeft(ball1) + (xspeed*xdirection)));
        }


        private void startButton_Click(object sender, RoutedEventArgs e)
        {
            startGameUp();
        }
    }
}