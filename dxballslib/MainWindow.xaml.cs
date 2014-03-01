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
        public List<ball> ballList = new List<ball>(); //list of the ball objects in the game at the moment
        public List<ContentControl> enemyBlockList = new List<ContentControl>(); //list of enemyblocks in the game at the moment
        public double enemyMovementSpeed = 0.1; //enemyBlocks descending speed
        public int spawnInterval = 250;//spawn interval for a new line of enemyBlocks, measured in ticks from the mainLoop
        public int score; //the score
        public int playerMovementSpeed = 5;

        private DispatcherTimer mainLoop = new DispatcherTimer();//main game loop
        private ContentControl playerBlock = new ContentControl();//the player
        private int spawnCounter; //counter for spawnInterval

        //nothing to see here
        public MainWindow()
        {
            InitializeComponent();
        } 

        //the main startup function, add a difficulty parameter etc here
        public void startGameUp()
        {
            score = 0; //set the score to 0

            //place a player on the screen in the middle
            playerBlock.Template = Resources["playerTemplate"] as ControlTemplate;
            playArea.Children.Add(playerBlock);
            Canvas.SetBottom(playerBlock, 10);
            Canvas.SetLeft(playerBlock, (playArea.ActualWidth / 2) - 25);
            startButton.Visibility = Visibility.Hidden; //hide the start game button
            spawnCounter = spawnInterval; // spawn a new line of enemies on the first tick of the game
            mainLoop.Interval = TimeSpan.FromMilliseconds(20); //50 per second
            mainLoop.Tick += mainLoop_Tick; //add an event handler for the "mainLoop.Tick" event
            mainLoop.Start();//start the dispatchertimer
        }

        void mainLoop_Tick(object sender, EventArgs e)//bliver kørt 50 gange i sekundet
        {
            //player controls
            if (Keyboard.IsKeyDown(Key.Left) || (Keyboard.IsKeyDown(Key.A)))//move left
            {
                Canvas.SetLeft(playerBlock, (Canvas.GetLeft(playerBlock) - playerMovementSpeed));
            }
            if (Keyboard.IsKeyDown(Key.Right) || (Keyboard.IsKeyDown(Key.D)))//move right
            {
                Canvas.SetLeft(playerBlock, (Canvas.GetLeft(playerBlock) + playerMovementSpeed));
            }
            //ball controls
            foreach (ball ballClass in ballList)
            {
                //get the variables from the ballclass
                ContentControl ball1 = ballClass.ballObject;
                int xdirection = ballClass.xdirection;
                int ydirection = ballClass.ydirection;
                double xspeed = ballClass.xspeed;
                double yspeed = ballClass.yspeed;
                double ballspeed = ballClass.ballSpeed;

                if (Canvas.GetLeft(ball1) >= (playArea.ActualWidth - ball1.ActualWidth))//hvis bolden rammer højre side
                {
                    xdirection = -1;
                }
                if (Canvas.GetLeft(ball1) <= 0)// hvis bolden rammer venstre side
                {
                    xdirection = 1;
                }
                if (Canvas.GetBottom(ball1) <= 0)//ball hits buttom
                {
                    ydirection = -1;
                }
                if (Canvas.GetBottom(ball1) >= playArea.ActualHeight)//hvis bolden rammer toppen
                {
                    ydirection = 1;
                }

                if (Canvas.GetBottom(ball1) <= 10 + playerBlock.ActualHeight && Canvas.GetBottom(ball1) >= 10)//ball player hittest
                {
                    if ((Canvas.GetLeft(ball1) >= Canvas.GetLeft(playerBlock)) && (Canvas.GetLeft(ball1) <= Canvas.GetLeft(playerBlock) + playerBlock.ActualWidth))
                    {
                        //find ud af hvor bolden rammer playerblocken 
                        double v1 = ((Canvas.GetLeft(ball1) + ball1.ActualWidth / 2) - Canvas.GetLeft(playerBlock));
                        double v2 = ((Canvas.GetLeft(playerBlock) + playerBlock.ActualWidth / 2) - Canvas.GetLeft(playerBlock));
                        v1 -= v2;
                        xspeed = v1 / v2;
                        xspeed *= ballspeed; //gang forholdet med boldens hastighed
                        if (xspeed < 0) //hvis bolden ramte playerblockens venstre side
                        {
                            xspeed = -xspeed;
                            xdirection = -1;//få bolden til at bevæge sig mod venstre
                        }
                        else
                        {//ellers bevæg den mod højre
                            xdirection = 1;
                        }
                        if (xspeed > ballspeed - 0.5)//hvis bolden bevæger sig for hurtigt vandret(og ikke efterlader noget af boldens max hastighed til y-hastigheden)
                        {
                            xspeed = ballspeed - 0.5;//så sænk dens x-hastighed til et maximum, 0.5 mindre end boldens totale max hastighed
                        }
                        yspeed = ballspeed - xspeed;//y hastighed udregnes ud fra boldens max hastighed og dens x hastighed
                        if (yspeed < 0) //dont go negative
                        {
                            yspeed = -yspeed;
                        }

                        ydirection = -1; //bevæg bolden opad nu
                    }
                }
                for (int i = 0; i < enemyBlockList.Count(); i++) //loop igennem hver enemyBlock i enemyBlockList'en
                {
                    double ballTop = playArea.ActualHeight - (Canvas.GetBottom(ball1) + ball1.ActualHeight); //boldenes position bliver sat via "SetBottom" 
                    //så de har ikke nogen "getTop" der kunne bruges, så udregn hvad dens "top" ville være.
                    if (ballTop >= Canvas.GetTop(enemyBlockList[i]) && ballTop <= (Canvas.GetTop(enemyBlockList[i]) + enemyBlockList[i].ActualHeight))
                    {//dens top er indenfor enemy'ens "top plus dens højde" område.
                        if (Canvas.GetLeft(ball1) >= Canvas.GetLeft(enemyBlockList[i]) && (Canvas.GetLeft(ball1) + ball1.ActualWidth) <= (Canvas.GetLeft(enemyBlockList[i]) + enemyBlockList[i].ActualWidth))
                        {//dens left er indefor enemy'ens "left plus dens bredde" område
                            if (ydirection == 1)
                            {//bevæg dig op hvis du er på vej ned
                                ydirection = -1;
                            }
                            else
                            {//ellers gå nedad
                                ydirection = 1;
                            }

                            playArea.Children.Remove(enemyBlockList[i]);//fjern enemyblock'en fra skærmen
                            enemyBlockList.RemoveAt(i);//og fra listen
                            score += 100;//tilføj lidt score
                        }
                    }
                }
                Canvas.SetBottom(ball1, (Canvas.GetBottom(ball1) - (yspeed * ydirection)));//flyt bolden
                Canvas.SetLeft(ball1, (Canvas.GetLeft(ball1) + (xspeed * xdirection)));//flyt bolden

                //change the values of the ballClass to the ones calculated in this loop
                ballClass.xdirection = xdirection;
                ballClass.ydirection = ydirection;
                ballClass.xspeed = xspeed;
                ballClass.yspeed = yspeed;
                ballClass.ballSpeed = ballspeed;
            }
            //enemy movement downwards
            foreach (ContentControl enemy in enemyBlockList)//for all enemies
            {
                Canvas.SetTop(enemy, Canvas.GetTop(enemy) + enemyMovementSpeed);//move a bit downward, depending on their movementspeed
            }
            if (spawnCounter >= spawnInterval)
            {//spawn some enemies hvis du har ventet nok ticks
                addEnemyLine();
                addNewBall();
                spawnCounter = 0; //reset spawncounteren
            }
            spawnCounter++;//another tick went by
            ScoreText.Text = score.ToString(); //update the score textblock
        }


        private void startButton_Click(object sender, RoutedEventArgs e)
        {
            startGameUp();//start game
        }
        //added from GeneratedCode\main.cs
        private void addEnemy(Point spawnLocation)//spawn enemy on location specified in the spawnLocation
        {
            ContentControl temp = new ContentControl();
            temp.Template = Resources["enemyStandardBlock"] as ControlTemplate;
            playArea.Children.Add(temp);
            Canvas.SetTop(temp, spawnLocation.Y);
            Canvas.SetLeft(temp, spawnLocation.X);
            enemyBlockList.Add(temp);//add it to the array
        }
        public virtual void addEnemyLine()//spawn a line of enemies
        {
            int numberOfBlocksToAdd = 10;//Enemies in a line
            int widthPerBlock = 50;//width of each enemy
            int spaceBetweenBlocks = 5;//space between enemies
            int totalWidthOfBlocksWhenPlaced = (numberOfBlocksToAdd * widthPerBlock) + ((numberOfBlocksToAdd - 1) * spaceBetweenBlocks);//total required for the enemy "field"
            if (playArea.ActualWidth < (totalWidthOfBlocksWhenPlaced+16)) //if screen is too small to fit all enemies resize screen
            {
                Application.Current.MainWindow.Width = totalWidthOfBlocksWhenPlaced+16;//ved ikke hvorfor +16, men ellers så gav den en spaceToFirstBlock på -8 / -4
            }
            int spaceToFirstBlock = ((int)playArea.ActualWidth - totalWidthOfBlocksWhenPlaced)/2; //space to move in before the first enemy block can be spawned (so all will be at the center)
            for(int i = 0; i <numberOfBlocksToAdd; i++){//spawn enemies
                addEnemy(new Point((i*55)+spaceToFirstBlock, 0));   
            }
        }
        private void addNewBall()
        {
            double ballSpeed = 6;//the balls standard spawning speed
            ContentControl tempBall = new ContentControl();//temporary ball ContentControl
            tempBall.Template = Resources["ballTemplate"] as ControlTemplate;//set a template for it
            playArea.Children.Add(tempBall);//add it to the screen
            Canvas.SetBottom(tempBall, (playArea.ActualHeight / 2));//set where it should start (y)
            Canvas.SetLeft(tempBall, (playArea.ActualWidth / 2) - 7.5);//set where it should start(x)
            Random r = new Random();//a random value to determine what direction the ball should move & what speed it should move with
            double xspeedTemp = r.NextDouble()*ballSpeed;//find a random speed
            int tempYDirection = r.Next(0,1);//find a random y direction based on r
            if(tempYDirection.Equals(0)){
                tempYDirection = -1;
            }
            int tempXDirection = r.Next(0,1);//find a random X direction based on r
            if(tempXDirection.Equals(0)){
                tempXDirection = -1;
            }
            ball tempBallClass = new ball(tempBall,xspeedTemp,(6-xspeedTemp),tempXDirection,tempYDirection,ballSpeed);//create a temporary instance of the class
            ballList.Add(tempBallClass);//add the object to the ballList
        }
    }
}