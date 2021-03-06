﻿using System;
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
using System.Data.SqlClient;
using System.Data;

namespace dxballslib
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<ball> ballList = new List<ball>(); //list of the ball objects in the game at the moment
        public List<ContentControl> enemyBlockList = new List<ContentControl>(); //list of enemyblocks in the game at the moment
        public List<player> playerList = new List<player>();
        public List<drops> dropList = new List<drops>();
        public double enemyMovementSpeed = new double(); //enemyBlocks descending speed
        public int spawnInterval = 250;//spawn interval for a new line of enemyBlocks, measured in ticks from the mainLoop
        public int score
        {
            get;set;
        } //the score

        public int playerMovementSpeed = 5;
        public double droprate = new double();

        private int spawningPlayerSpeed = 5;
        private DispatcherTimer mainLoop = new DispatcherTimer();//main game loop
        private ContentControl playerBlock = new ContentControl();//the player
        private int spawnCounter; //counter for spawnInterval
        private Random rand = new Random();

        //nothing to see here
        public MainWindow()
        {
            InitializeComponent();
            gameOverText.Visibility = Visibility.Hidden;//hide gameover text
            mainLoop.Interval = TimeSpan.FromMilliseconds(20); //50 per second
            mainLoop.Tick += mainLoop_Tick; //add an event handler for the "mainLoop.Tick" event
        } 

        //the main startup function, add a difficulty parameter etc here
        public void startGameUp(difficulty difficultyClass)
        {
            score = 0; //set the score to 0
            droprate = difficultyClass.dropRate;//get droprate from difficulty object
            enemyMovementSpeed = difficultyClass.descendSpeed;//get enemy movement speed from difficulty object
            difficultyGrid.Visibility = Visibility.Hidden;//hide difficulty selection grid
            submitForm.Visibility = Visibility.Hidden;//hide submitForm
            gameOverText.Visibility = Visibility.Hidden;//hide gameOverText
            addNewBall();//add a ball
            //place a player on the screen in the middle
            addPlayer(1);//add the local player
            startButton.Visibility = Visibility.Hidden; //hide the start game button
            spawnCounter = spawnInterval; // spawn a new line of enemies on the first tick of the game
            mainLoop.Start();//start the dispatchertimer
        }

        void mainLoop_Tick(object sender, EventArgs e)//bliver kørt 50 gange i sekundet
        {
            foreach (player playerClass in playerList)
            {
                //fetch variables from the playerclass
                int playerMovementSpeed = playerClass.speed;
                ContentControl playerBlock = playerClass.playerBlock;
                int playerNumber = playerClass.playerNumber;

                //player controls
                if (playerNumber == 1)//local player
                {
                    if (Keyboard.IsKeyDown(Key.Left) || (Keyboard.IsKeyDown(Key.A)))//move left
                    {
                        playerClass.move("LEFT");
                    }
                    if (Keyboard.IsKeyDown(Key.Right) || (Keyboard.IsKeyDown(Key.D)))//move right
                    {
                        playerClass.move("RIGHT");
                    }
                }
                //ball controls
                for (int ballindex = 0; ballindex < ballList.Count; ballindex++)
                {
                    ball ballClass = ballList[ballindex];
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
                        playArea.Children.Remove(ball1);
                        ballList.RemoveAt(ballindex);
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
                                death(enemyBlockList[i], 100);
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
                //drop hit-testing
                for (int i = 0; i < dropList.Count; i++)
                {
                    drops drop = dropList[i];
                    ContentControl dropObj = drop.DropObject;
                    if (Canvas.GetBottom(dropObj) <= 10 + playerBlock.ActualHeight && Canvas.GetBottom(dropObj) >= 10)//drop player hittest
                    {
                        if ((Canvas.GetLeft(dropObj) >= Canvas.GetLeft(playerBlock)) && (Canvas.GetLeft(dropObj) <= Canvas.GetLeft(playerBlock) + playerBlock.ActualWidth))
                        {
                            switch (drop.hitPlayer())
                            {
                                case "BUFF":
                                    break;
                                case "DEBUFF":
                                    break;
                                case "POINTBONUS":
                                    score += (int)drop.Points;
                                    playArea.Children.Remove(dropObj);
                                    dropList.Remove(drop);
                                    break;
                                case "LIFEBONUS":
                                    break;
                                case "BALLBUFF":
                                    for (int i2 = 0; i2 < drop.ballBuff; i2++)
                                    {
                                        addNewBall();
                                    }
                                    break;
                            }
                        }
                    }
                    if (Canvas.GetBottom(dropObj) <= 0)
                    {
                        playArea.Children.Remove(dropObj);
                        dropList.Remove(drop);
                    }
                    Canvas.SetBottom(dropObj, Canvas.GetBottom(dropObj) - drop.dropSpeed);
                }
            }
            //enemy movement downwards
            foreach (ContentControl enemy in enemyBlockList)//for all enemies
            {
                Canvas.SetTop(enemy, Canvas.GetTop(enemy) + enemyMovementSpeed);//move a bit downward, depending on their movementspeed
                if (Canvas.GetTop(enemy) >= playArea.ActualHeight)
                {
                    playerDied();
                }
            }
            if (spawnCounter >= spawnInterval)
            {//spawn some enemies hvis du har ventet nok ticks
                addEnemyLine();
                //addNewBall();
                spawnCounter = 0; //reset spawncounteren
            }
             //test if player is dead
            if (ballList.Count == 0)//if there are no more balls
            {
                playerDied();//player dies
            }
            spawnCounter++;//another tick went by
            ScoreText.Text = score.ToString(); //update the score textblock
        }


        private void startButton_Click(object sender, RoutedEventArgs e)
        {
            if (easy.IsChecked.Value)
            {
                startGameUp(new difficulty(1));   
            }
            else if (medium.IsChecked.Value)
            {
                startGameUp(new difficulty(2));
            }
            else if (hard.IsChecked.Value)
            {
                startGameUp(new difficulty(3));
            }
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
        public void addNewBall()
        {
            double ballSpeed = 6;//the balls standard spawning speed
            ContentControl tempBall = new ContentControl();//temporary ball ContentControl
            tempBall.Template = Resources["ballTemplate"] as ControlTemplate;//set a template for it
            playArea.Children.Add(tempBall);//add it to the screen
            Canvas.SetBottom(tempBall, (playArea.ActualHeight / 2));//set where it should start (y)
            Canvas.SetLeft(tempBall, (playArea.ActualWidth / 2) - 7.5);//set where it should start(x)
            //Random r = new Random();//a random value to determine what direction the ball should move & what speed it should move with
            double xspeedTemp = rand.NextDouble()*ballSpeed;//find a random speed
            int tempYDirection = rand.Next(1,3);//find a random y direction based on r
            if(tempYDirection.Equals(2)){
                tempYDirection = -1;
            }
            int tempXDirection = rand.Next(1,3);//find a random X direction based on r
            if(tempXDirection.Equals(2)){
                tempXDirection = -1;
            }
            ball tempBallClass = new ball(tempBall,xspeedTemp,(6-xspeedTemp),tempXDirection,tempYDirection,ballSpeed);//create a temporary instance of the class
            ballList.Add(tempBallClass);//add the object to the ballList
        }

        public void addPlayer(int controller)
        {
            ContentControl tempPlayer = new ContentControl();
            tempPlayer.Template = Resources["playerTemplate"] as ControlTemplate;
            playArea.Children.Add(tempPlayer);
            Canvas.SetBottom(tempPlayer, 10);
            Canvas.SetLeft(tempPlayer, (playArea.ActualWidth / 2) - 25);
            player tempPlayerClass = new player(tempPlayer, controller, spawningPlayerSpeed);
            playerList.Add(tempPlayerClass);
        }

        private void death(ContentControl enemy, int scoreToAdd)
        {
            Random r = new Random();
            if (r.NextDouble() <= droprate)
            {
                string[] typesOfBuffs = new string[] { "ballBuff", "scoreBuff" };
                double y = (playArea.ActualHeight - Canvas.GetTop(enemy) - enemy.ActualHeight);
                double x = (Canvas.GetLeft(enemy) + (enemy.ActualWidth / 2) - 2.5);
                addDrop(new Point(x, y), typesOfBuffs[r.Next(0, typesOfBuffs.Count())]);
            }

            playArea.Children.Remove(enemy);//fjern enemyblock'en fra skærmen
            enemyBlockList.Remove(enemy);//og fra listen
            score += scoreToAdd;//tilføj lidt score   
        }
        private void addDrop(Point whereToSpawn, string type)
        {
            ContentControl buff = new ContentControl();//temporary drop ContentControl
            buff.Template = Resources[type] as ControlTemplate;//set a template for it
            playArea.Children.Add(buff);//add it to the screen
            Canvas.SetBottom(buff, whereToSpawn.Y);//set where it should start (y)
            Canvas.SetLeft(buff, whereToSpawn.X);//set where it should start(x)
            drops tempDrop;
            switch(type){
                case "scoreBuff":
                    tempDrop = new drops(buff, type, 500);
                    break;
                case "ballBuff":
                    int? numOfBalls = (int?)(rand.NextDouble() * 3);
                    numOfBalls++;
                    tempDrop = new drops(buff, type, null, null, null, null, numOfBalls);
                    break;
                default:
                    tempDrop = new drops(buff, type, 500);
                    break;
            }
            //Hvis droppet så er et buff drop eller et debuff drop, så sender du et buff eller debuff objekt med når du instantierer objektet som ovenfor..
            dropList.Add(tempDrop);//add it to the dropList
        }
        private void playerDied()
        {
            mainLoop.Stop();//stop the game loop
            gameOverText.Visibility = Visibility.Visible;//make gameovertext visiible
            submitForm.Visibility = Visibility.Visible;//make the submitform visible
            difficultyGrid.Visibility = Visibility.Visible;
            finalScoreText.Text = score.ToString();
            int e = enemyBlockList.Count;
            for (int i = 1; i <= e; i++)//remove all the enemyblocks
            {
                playArea.Children.Remove(enemyBlockList[e - i]);
                enemyBlockList.RemoveAt(e-i);
            }
            e = playerList.Count();
            for (int i = 1; i <= e; i++)//remove all players
            {
                playArea.Children.Remove(playerList[e - i].playerBlock);
                playerList.RemoveAt(e - i);
            }
            e = dropList.Count;
            for (int i = 1; i <= e; i++)//remove all drops
            {
                playArea.Children.Remove(dropList[e - i].DropObject);
                dropList.RemoveAt(e - i);
            }

        }

        private void submitButton_Click(object sender, RoutedEventArgs e)
        {
            if (name.Text != "")
            {
                SqlConnection connection = new SqlConnection(@"Data Source=192.168.139.91\SQLEXPRESS;Initial Catalog=DXBallHighScore;Persist Security Info=True;User ID=DXBALL;Password=password");
                connection.Open();

                SqlCommand insertCommand = new SqlCommand();
                insertCommand.CommandType = CommandType.StoredProcedure;
                insertCommand.CommandText = "InsertDXBallScore";
                insertCommand.Connection = connection;

                insertCommand.Parameters.AddWithValue("@PlayerName", name.Text);
                insertCommand.Parameters.AddWithValue("@Score", score);
                insertCommand.ExecuteNonQuery();
                //input score to database, "name.Text" holds playername, "score" holds the score
                MessageBox.Show("show");
            }
            else
            {
                MessageBox.Show("please input a name");
            }
        }

        private void restart_Click(object sender, RoutedEventArgs e)
        {
            if (easy.IsChecked.Value)
            {
                startGameUp(new difficulty(1));
            }
            else if (medium.IsChecked.Value)
            {
                startGameUp(new difficulty(2));
            }
            else if (hard.IsChecked.Value)
            {
                startGameUp(new difficulty(3));
            }
        }
    }
}