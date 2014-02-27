using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace dxballsLib
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public partial class MainPage : Page
    {

        public MainPage()
        {
            this.InitializeComponent();
        }
        public virtual object enemyList
        {
            get;
            set;
        }

        public List<ContentControl> ballList = new List<ContentControl>();
        private List<Point>oldBallCords = new List<Point>();
        public virtual object amountOfBalls
        {
            get;
            set;
        }

        public virtual object Score
        {
            get;
            set;
        }

        public virtual object droprate
        {
            get;
            set;
        }

        public virtual difficulty difficulty
        {
            get;
            set;
        }

        public virtual player player
        {
            get;
            set;
        }

        private void addPlayer()
        {
            throw new System.NotImplementedException();
        }

        public virtual void addEnemy(Point spawnLocation)
        {
            throw new System.NotImplementedException();
        }
            
        public virtual void addBall()
        {
            //ContentControl ballobj = new ContentControl();
            ballList.Add(new ContentControl() { Template = Resources["ballTemplate"] as ControlTemplate });
            //ballList[0].Template = Resources["ballTemplate"] as ControlTemplate;
            playArea.Children.Add(ballList[0]);
            Canvas.SetLeft(ballList[0], 500);
            Canvas.SetTop(ballList[0], 10);
//            oldBallCords.Add(new Point(500, 0));
            
        }

        public virtual void addEnemyLine()
        {
            throw new System.NotImplementedException();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            addBall();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            bool i = true;
            int speed = 10;
            double nextRelativeXPos = new double();
            double nextRelativeYPos = new double();
            if (oldBallCords.Count >= 1)
            {
            double ydifference = oldBallCords[0].Y - Canvas.GetLeft(ballList[0]);
            double xdifference = oldBallCords[0].X - Canvas.GetTop(ballList[0]);
            double angle = Math.Atan(ydifference / xdifference);
            nextRelativeXPos = Math.Cos(angle) * speed;
            nextRelativeYPos = Math.Sin(angle) * speed;
            i = false;
            }
            if (!i)
            {
                oldBallCords[0] = new Point(Canvas.GetLeft(ballList[0]), Canvas.GetTop(ballList[0]));
            }
            else
            {
                oldBallCords.Add(new Point(Canvas.GetLeft(ballList[0]), Canvas.GetTop(ballList[0])));
            }
            if (!i)
            {
                Canvas.SetLeft(ballList[0], Canvas.GetLeft(ballList[0]) + nextRelativeXPos);
                Canvas.SetTop(ballList[0], Canvas.GetLeft(ballList[0]) + nextRelativeYPos);
            }
        }
    }
}
