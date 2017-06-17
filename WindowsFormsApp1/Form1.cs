using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        sectList snake = new sectList();
        sectList obs = new sectList();
        Boolean gameHandler = false;
        sect apple = new sect();
        Color snakeColor = Color.DarkOliveGreen;
        int maxSize = 600;

        public Form1()
        {
            InitializeComponent();
            label13.Hide();
            pictureBox1.Hide();
            timer1.Tick += new EventHandler(timer1_Tick);
            pictureBox1.Location = new Point(12, 12);
            pictureBox1.Width = maxSize + 10;
            pictureBox1.Height = maxSize + 10;
            pictureBox1.BackColor = this.BackColor;
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
            addPoint.X = snake.end.point.X + incre.x;
            addPoint.Y = snake.end.point.Y + incre.y;
            if (checkBox1.Checked)
                turnAround(ref addPoint);
            if (addPoint.X > snakeCoord.count - 1 || addPoint.Y > snakeCoord.count - 1 || addPoint.X < 0 || addPoint.Y < 0)
            {
                gameOver();
                return;
            }
            if (snake.overlaps(addPoint) || obs.overlaps(addPoint))
            {
                gameOver();
                return;
            }
            if (sectList.collis(apple.point, addPoint))
            {
                snake.push_b(addPoint, pictureBox1, snakeColor);
                setApple();
            }
            else
            {
                snake.push_b(addPoint, pictureBox1, snakeColor);
                snake.pop_f();
            }
        }

        void turnAround(ref Point addPoint)
        {
            if (addPoint.X < 0)
                addPoint.X += snakeCoord.count;
            if (addPoint.Y < 0)
                addPoint.Y += snakeCoord.count;
            if (addPoint.X >= snakeCoord.count - 1)
                addPoint.X %= snakeCoord.count;
            if (addPoint.Y >= snakeCoord.count - 1)
                addPoint.Y %= snakeCoord.count;
        }


        void initGame()
        {
            gameHandler = true;
            snake.clear();
            label13.Hide();
            pictureBox1.Show();
            int speed = trackBar1.Value + 1;
            int interval = (int)(500 * Math.Pow(0.6, speed));
            timer1.Interval = interval;
            snakeCoord.unit = trackBar2.Value * 4 + 10;
            snakeCoord.count = maxSize / snakeCoord.unit;
            Point startPoint = new Point();
            startPoint.X = snakeCoord.count / 2;
            startPoint.Y = snakeCoord.count / 2;
            snake.push_b(startPoint, pictureBox1, snakeColor);
            incre.x = 1;
            incre.y = 0;
            incRec.x = incre.x;
            incRec.y = incre.y;
            startPoint.X += incre.x;
            startPoint.Y += incre.y;
            snake.push_b(startPoint, pictureBox1, snakeColor);
            label7.Hide();
            setObstructs();
            setApple();
            this.Refresh();
            timer1.Start();
            trackBar2.Enabled = false;
            trackBar4.Enabled = false;
            checkBox1.Enabled = false;
        }

        void gameOver()
        {
            this.Update();
            timer1.Stop();
            label7.Show();
            label13.Show();
            this.Update();
            trackBar2.Enabled = true;
            checkBox1.Enabled = true;
            trackBar4.Enabled = true;
            apple.picBox.Dispose();
            snake.clear();
            gameHandler = false;
            pictureBox1.Hide();
            this.Update();
        }

        void setApple()
        {
            if (apple.picBox != null)
                apple.picBox.Dispose();
            Random random = new Random();
            int x = random.Next(0, snakeCoord.count);
            int y = random.Next(0, snakeCoord.count);
            Point applePoint = new Point(x, y);
            while (snake.overlaps(applePoint) || obs.overlaps(applePoint))
            {
                x = random.Next(0, snakeCoord.count);
                y = random.Next(0, snakeCoord.count);
                applePoint.X = x;
                applePoint.Y = y;
            }
            apple.point = applePoint;
            apple.draw(Color.Red, pictureBox1);
        }

        private void Form1_KeyDown_1(object sender, KeyEventArgs e)
        {
            e.SuppressKeyPress = true;
            if (gameHandler == false && e.KeyCode != Keys.Escape)
            {
                initGame();
                return;
            }
            if (e.KeyCode == Keys.Up || e.KeyCode == Keys.W)
            {
                if (incre.y > 0)
                    return;
                incre.x = 0;
                incre.y = -1;
            }
            if (e.KeyCode == Keys.Down || e.KeyCode == Keys.S)
            {
                if (incre.y < 0)
                    return;
                incre.x = 0;
                incre.y = 1;
            }
            if (e.KeyCode == Keys.Left || e.KeyCode == Keys.A)
            {
                if (incre.x > 0)
                    return;
                incre.x = -1;
                incre.y = 0;
            }
            if (e.KeyCode == Keys.Right || e.KeyCode == Keys.D)
            {
                if (incre.x < 0)
                    return;
                incre.x = 1;
                incre.y = 0;
            }
            if (e.KeyCode == Keys.Escape)
            {
                label7.Text = "Press any key to start";
                label7.Show();
                label13.Hide();
                pictureBox1.Hide();
                timer1.Stop();
                snake.clear();
                apple.picBox.Dispose();
                gameHandler = false;
                trackBar2.Enabled = true;
                checkBox1.Enabled = true;
                trackBar4.Enabled = true;
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

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            Pen bluePen = new Pen(Color.LightBlue, 1);
            int[] coord = new int[snakeCoord.count + 1];
            for (int i = 0; i <= snakeCoord.count; i++)
            {
                coord[i] = i * snakeCoord.unit;
            }
            for (int i = 0; i <= snakeCoord.count; i++)
            {
                e.Graphics.DrawLine(bluePen, 0, coord[i], snakeCoord.unit * (snakeCoord.count), coord[i]);
            }
            for (int i = 0; i <= snakeCoord.count; i++)
            {
                e.Graphics.DrawLine(bluePen, coord[i], 0, coord[i], snakeCoord.unit * (snakeCoord.count));
            }
        }

        void setObstructs()
        {
            obs.clear();
            Random rnd = new Random();
            int difficulty = (int)(Math.Pow(1.4, trackBar4.Value) + 5) * trackBar4.Value / (trackBar2.Value + 1);
            for (int i = 0; i < difficulty; i++)
            {
                int x = -1;
                int y = -1;
                while (x < 0 || y < 0 || x >= snakeCoord.count || y >= snakeCoord.count || snake.overlaps(new Point(x, y)))
                {
                    x = rnd.Next(0, snakeCoord.count);
                    y = rnd.Next(0, snakeCoord.count);
                }
                obs.push_b(new Point(x, y), pictureBox1, Color.Black);
                int size = rnd.Next(0, difficulty * 2);
                for (int j = 0; j < size; j++)
                {
                    int x1 = -65535, y1 = -65535;
                    while (x + x1 < 0 || y + y1 < 0 || x + x1 >= snakeCoord.count || y + y1 >= snakeCoord.count || snake.overlaps(new Point(x + x1, y + y1)))
                    {
                        x1 = rnd.Next(-1, 2);
                        y1 = rnd.Next(-1, 2);
                    }
                    obs.push_b(new Point(x + x1, y + y1), pictureBox1, Color.Black);
                }
            }
        }

        private void trackBar1_Changed(object sender, EventArgs e)
        {
            int speed = trackBar1.Value + 1;
            int interval = (int)(500 * Math.Pow(0.6, speed));
            timer1.Interval = interval;
        }
    }

    public static class snakeCoord
    {
        public static int unit = 22;
        public static int count = 27;
    }

    public static class incre
    {
        public static int x = 1;
        public static int y = 0;
    }

    public static class incRec
    {
        public static int x = 1;
        public static int y = 0;
    }

    public class sect
    {
        public sect next;
        public Point point;
        public PictureBox picBox;
        public void draw(Color color, Control ctrl)
        {
            Point topLeft = new Point();
            topLeft.X = point.X * snakeCoord.unit;
            topLeft.Y = point.Y * snakeCoord.unit;
            picBox = new PictureBox();
            picBox.Location = topLeft;
            picBox.Width = snakeCoord.unit;
            picBox.Height = snakeCoord.unit;
            picBox.BackColor = color;
            ctrl.Controls.Add(picBox);
        }
    }

    public class sectList
    {
        public sect head, end;

        public void push_b(Point newCoord, Control ctrl, Color color)
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
            end.draw(color, ctrl);
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

        public static Boolean collis(Point a, Point b)
        {
            if (Math.Abs(a.X - b.X) < 1 && Math.Abs(a.Y - b.Y) < 1)
                return true;
            else
                return false;
        }
    }
}
