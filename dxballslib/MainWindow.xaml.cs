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
        public List<ContentControl> enemyBlockList = new List<ContentControl>();
        public double enemyMovementSpeed = 0.1;
        public int spawnInterval = 250;
        private int spawnCounter;
        public int score;
        public MainWindow()
        {
            InitializeComponent();
            //startGameUp();
        }

        public void startGameUp()
        {
            score = 0;
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
            spawnCounter = spawnInterval;
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
            for (int i = 0; i < enemyBlockList.Count(); i++ )
            {
                double ballTop = playArea.ActualHeight - (Canvas.GetBottom(ball1) + ball1.ActualHeight); //boldenes position bliver sat via "SetBottom" 
                //så de har ikke nogen "getTop" der kunne bruges, så udregn hvad dens "top" ville være.
                if (ballTop >= Canvas.GetTop(enemyBlockList[i]) && ballTop <= (Canvas.GetTop(enemyBlockList[i]) + enemyBlockList[i].ActualHeight))
                {//dens top er indenfor enemy'ens "top plus dens højde" område.
                    if (Canvas.GetLeft(ball1) >= Canvas.GetLeft(enemyBlockList[i]) && (Canvas.GetLeft(ball1) + ball1.ActualWidth) <= (Canvas.GetLeft(enemyBlockList[i]) + enemyBlockList[i].ActualWidth))
                    {
                        if (ydirection == 1)
                        {
                            ydirection = -1;
                        }
                        else
                        {
                            ydirection = 1;
                        }

                        playArea.Children.Remove(enemyBlockList[i]);
                        enemyBlockList.RemoveAt(i);
                        score += 100;
                    }
                }
            }
            Canvas.SetBottom(ball1, (Canvas.GetBottom(ball1) - (yspeed * ydirection)));
            Canvas.SetLeft(ball1, (Canvas.GetLeft(ball1) + (xspeed*xdirection)));
            //enemy movement downwards
            foreach (ContentControl enemy in enemyBlockList)
            {
                Canvas.SetTop(enemy, Canvas.GetTop(enemy) + enemyMovementSpeed);
            }
            if (spawnCounter >= spawnInterval)
            {
                addEnemyLine();
                spawnCounter = 0;
            }
            spawnCounter++;
            ScoreText.Text = score.ToString(); //update the score textblock
        }


        private void startButton_Click(object sender, RoutedEventArgs e)
        {
            startGameUp();
        }
        //added from GeneratedCode\main.cs
        private void addEnemy(Point spawnLocation)
        {
            ContentControl temp = new ContentControl();
            temp.Template = Resources["enemyStandardBlock"] as ControlTemplate;
            playArea.Children.Add(temp);
            Canvas.SetTop(temp, spawnLocation.Y);
            Canvas.SetLeft(temp, spawnLocation.X);
            enemyBlockList.Add(temp);
            //MessageBox.Show("derp");
        }
        public virtual void addEnemyLine()
        {
            int numberOfBlocksToAdd = 10;
            int widthPerBlock = 50;
            int spaceBetweenBlocks = 5;
            int totalWidthOfBlocksWhenPlaced = (numberOfBlocksToAdd * widthPerBlock) + ((numberOfBlocksToAdd - 1) * spaceBetweenBlocks);
            if (playArea.ActualWidth < (totalWidthOfBlocksWhenPlaced+16))
            {
                Application.Current.MainWindow.Width = totalWidthOfBlocksWhenPlaced+16;//ved ikke hvorfor +16, men ellers så gav den en spaceToFirstBlock på -8 / -4
            }
            int spaceToFirstBlock = ((int)playArea.ActualWidth - totalWidthOfBlocksWhenPlaced)/2;
            for(int i = 0; i <numberOfBlocksToAdd; i++){
                addEnemy(new Point((i*55)+spaceToFirstBlock, 0));   
            }
        }
    }
}