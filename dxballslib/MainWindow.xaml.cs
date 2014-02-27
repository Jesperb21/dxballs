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
using System.Timers

namespace dxballslib
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int dx;
        int dy;
        int x;
        int y;

        public MainWindow()
        {
            InitializeComponent();
            Random rnd = new Random();
dx = rnd.Next(1, 4);
dy = rnd.Next(1, 4);
x = rnd.Next(0, Convert.ToInt32(gameform1.RenderSize.Width) - 50);
y = rnd.Next(0, Convert.ToInt32(gameform1.RenderSize.Height) - 50);

           Timer ballmove = new Timer(100);
            ballmove.AutoReset = true;
            ballmove.Enabled = true;

            ballmove.Elapsed +=ballmove_Elapsed;

        }

void ballmove_Elapsed(object sender, ElapsedEventArgs e)
{
    x += dx;
if (x < 0)
{
dx = -dx;
}
else if (x + 50 > this.ClientSize.Width)
{
dx = -dx;
}

y += dy;
if (y < 0)
{
dy = -dy;
}
else if (y + 50 > this.ClientSize.Height)
{
dy = -dy;
}
this.Invalidate();

    
}

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            e.Graphics.Clear(this.BackColor);
e.Graphics.FillEllipse(Brushes.Black, x, y, 50, 50);
e.Graphics.DrawEllipse(Pens.Black, x, y, 50, 50);

        }
    }
}
