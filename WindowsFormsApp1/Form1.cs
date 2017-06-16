using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        sectList snake = new sectList();
        Boolean gameHandler = false;
        sect apple = new sect();
        const int SIZE = 599;

        public Form1()
        {
            InitializeComponent();
            Point gbP = new Point();
            gbP.X = 12;
            gbP.Y = 12;
            groupBox1.Location = gbP;
            groupBox1.Height = SIZE + 1;
            groupBox1.Width = SIZE + 1;
            label13.Hide();
            timer1.Tick += new EventHandler(timer1_Tick);
        }

        public void timer1_Tick(object sender, EventArgs e)
        {
            if (incre.x == incRec.x && incre.y == -(incRec.y) && incre.y != 0)
            {
                incre.y = incRec.y;
            }
            else if (incre.x == -(incRec.x) && incre.y == incRec.y && incre.x != 0)
            {
                incre.x = incRec.x;
            }
            incRec.x = incre.x;
            incRec.y = incre.y;
            Point addPoint = new Point();
            if (checkBox1.Checked)
                turnAround(ref addPoint);
            else
            {
                addPoint.X = snake.end.point.X + incre.x;
                addPoint.Y = snake.end.point.Y + incre.y;
                if (addPoint.X > SIZE - snakeWidth.width || addPoint.Y > SIZE - snakeWidth.width)
                {
                    gameOver();
                    return;
                }
            }
            if (snake.overlaps(addPoint))
            {
                gameOver();
                return;
            }
            if (sectList.collis(apple.point, addPoint))
            {
                snake.push_b(addPoint, groupBox1);
                setApple();
            }
            else
            {
                snake.push_b(addPoint, groupBox1);
                snake.pop_f();
            }
        }

        void turnAround(ref Point addPoint)
        {
            int unit = snakeWidth.width;
            if (snake.end.point.X <= unit && incre.x < 0)
                addPoint.X = SIZE;
            else if (snake.end.point.X >= SIZE - unit && incre.x > 0)
                addPoint.X = 0;
            else
                addPoint.X = snake.end.point.X + incre.x;

            if (snake.end.point.Y <= unit && incre.y < 0)
                addPoint.Y = SIZE;
            else if (snake.end.point.Y >= SIZE - unit && incre.y > 0)
                addPoint.Y = 0;
            else
                addPoint.Y = snake.end.point.Y + incre.y;
        }


        void initGame()
        {
            gameHandler = true;
            snake.clear();
            int speed = trackBar1.Value + 1;
            int interval = (int)(500 * Math.Pow(0.6, speed));
            timer1.Interval = interval;
            snakeWidth.width = trackBar2.Value * 2 + 5;
            Point startPoint = new Point();
            int unit = snakeWidth.width * 2 + 1;
            int unitCount = SIZE / unit - 1;
            unitCount /= 2;
            startPoint.X = unitCount * unit + snakeWidth.width;
            startPoint.Y = unitCount * unit + snakeWidth.width;
            snake.push_b(startPoint, groupBox1);
            incre.x = (snakeWidth.width * 2 + 1);
            incre.y = 0;
            incRec.x = incre.x;
            incRec.y = incre.y;
            startPoint.X += incre.x;
            startPoint.Y += incre.y;
            snake.push_b(startPoint, groupBox1);
            label7.Hide();
            this.Update();
            timer1.Start();
            setApple();
            trackBar1.Enabled = false;
            trackBar2.Enabled = false;
            checkBox1.Enabled = false;
        }

        void gameOver()
        {
            this.Update();
            timer1.Stop();
            for (int i = 0; i < 3; i++)
            {
                label13.Show();
                this.Update();
                Thread.Sleep(1000);
                label13.Hide();
                this.Update();
                Thread.Sleep(500);
            }
            trackBar1.Enabled = true;
            trackBar2.Enabled = true;
            checkBox1.Enabled = true;
            apple.picBox.Dispose();
            snake.clear();
            gameHandler = false;
            label7.Show();
            this.Update();
        }

        void setApple()
        {
            if (apple.picBox != null)
                apple.picBox.Dispose();
            Random random = new Random();
            int unit = snakeWidth.width * 2 + 1;
            int unitCount = SIZE / unit - 1;
            int x = random.Next(0, unitCount) * unit + snakeWidth.width;
            int y = random.Next(0, unitCount) * unit + snakeWidth.width;
            Point applePoint = new Point(x, y);
            while (snake.overlaps(applePoint))
            {
                x = random.Next(0, unitCount) * unit + snakeWidth.width;
                y = random.Next(0, unitCount) * unit + snakeWidth.width;
                applePoint.X = x;
                applePoint.Y = y;
            }
            apple.point = applePoint;
            apple.draw(Color.Red, groupBox1);
        }

        private void Form1_KeyDown_1(object sender, KeyEventArgs e)
        {
            e.SuppressKeyPress = true;
            if (gameHandler == false)
            {
                initGame();
                return;
            }
            if (e.KeyCode == Keys.Up || e.KeyCode == Keys.W)
            {
                if (incre.y > 0)
                    return;
                incre.x = 0;
                incre.y = -1 * (snakeWidth.width * 2 + 1);
            }
            if (e.KeyCode == Keys.Down || e.KeyCode == Keys.S)
            {
                if (incre.y < 0)
                    return;
                incre.x = 0;
                incre.y = 1 * (snakeWidth.width * 2 + 1);
            }
            if (e.KeyCode == Keys.Left || e.KeyCode == Keys.A)
            {
                if (incre.x > 0)
                    return;
                incre.x = -1 * (snakeWidth.width * 2 + 1);
                incre.y = 0;
            }
            if (e.KeyCode == Keys.Right || e.KeyCode == Keys.D)
            {
                if (incre.x < 0)
                    return;
                incre.x = 1 * (snakeWidth.width * 2 + 1);
                incre.y = 0;
            }
            if (e.KeyCode == Keys.Escape)
            {
                label7.Text = "Press any key to start";
                label7.Show();
                timer1.Stop();
                snake.clear();
                apple.picBox.Dispose();
                gameHandler = false;
                trackBar1.Enabled = true;
                trackBar2.Enabled = true;
                checkBox1.Enabled = true;
                this.Update();
            }
        }

        private void Form1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Up:
                case Keys.Down:
                case Keys.Left:
                case Keys.Right:
                    e.IsInputKey = true;
                    break;
            }
        }
    }

    public static class snakeWidth
    {
        public static int width = 0;
    }

    public static class incre
    {
        public static int x;
        public static int y;
    }

    public static class incRec
    {
        public static int x;
        public static int y;
    }

    public class sect
    {
        public sect next;
        public Point point;
        public PictureBox picBox;
        public void draw(Color color, GroupBox gb)
        {
            Point topLeft = new Point();
            topLeft.X = point.X - snakeWidth.width;
            topLeft.Y = point.Y - snakeWidth.width;
            picBox = new PictureBox();
            picBox.Location = topLeft;
            picBox.Width = (snakeWidth.width * 2 + 1);
            picBox.Height = (snakeWidth.width * 2 + 1);
            picBox.BackColor = color;
            gb.Controls.Add(picBox);
        }
    }

    public class sectList
    {
        public sect head, end;

        public void push_b(Point newCoord, GroupBox gb)
        {
            if (head == null)
            {
                head = new sect();
                head.point = newCoord;
                head.next = null;
                end = head;
            }
            else
            {
                sect newSect = new sect();
                newSect.point = newCoord;
                end.next = newSect;
                end = end.next;
            }
            end.draw(Color.DarkGray, gb);
        }

        public void pop_f()
        {
            head.picBox.Dispose();
            sect temp = head.next;
            head = temp;
        }

        public Boolean overlaps(Point pScanned)
        {
            if (head == null)
                return false;
            sect iterate = head;
            while (iterate != end)
            {
                if (collis(iterate.point, pScanned))
                    return true;
                iterate = iterate.next;
            }
            if (collis(iterate.point, pScanned))
                return true;
            return false;
        }

        public void clear()
        {
            if (head == null)
                return;
            sect i = head;
            while (i != end)
            {
                i.picBox.Dispose();
                i = i.next;
            }
            i.picBox.Dispose();
            head = null;
            end = null;
        }

        //public Boolean secondLast(Point p)
        //{
        //    if (head == null || head == end)
        //        return false;
        //    sect iterate = head;
        //    while (iterate.next != end)
        //        iterate = iterate.next;
        //    if (collis(iterate.point, p))
        //        return true;
        //    return false;
        //}

        public static Boolean collis(Point a, Point b)
        {
            if (Math.Abs(a.X - b.X) < (snakeWidth.width * 2 + 1) && Math.Abs(a.Y - b.Y) < (snakeWidth.width * 2 + 1))
                return true;
            else
                return false;
        }
    }
}
