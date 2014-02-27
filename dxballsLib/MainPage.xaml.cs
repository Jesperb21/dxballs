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
    public sealed partial class MainPage : Page
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

        public virtual object ballList
        {
            get;
            set;
        }

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
            ContentControl ballobj = new ContentControl();
            ballobj.Template = Resources["ballTemplate"] as ControlTemplate;
            playArea.Children.Add(ballobj);
        }

        public virtual void addEnemyLine()
        {
            throw new System.NotImplementedException();
        }
    }
}
