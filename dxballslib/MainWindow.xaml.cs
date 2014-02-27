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
            Canvas.SetBottom(ball1, (Canvas.GetBottom(ball1)-3));
            Canvas.SetLeft(ball1, (Canvas.GetLeft(ball1) + 3));
            /*
             hvis bold rammer side, hvis bold rammer player, sæt x speed / y speed til "xspeed *= -1", 
             "3" skal udregnes ud fra en variabel der hedder speed
             */
        }


        private void startButton_Click(object sender, RoutedEventArgs e)
        {
            startGameUp();
        }
    }
}