using System;
using System.Drawing;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        sectList snake; //queue for the snake
        sectList obs;   //queue for obstacles
        Boolean gameHandler = false;    //Indicates whether the game is live
        sect apple = new sect();
        Color snakeColor = Color.DarkOliveGreen;
        int maxSize = 600;

        public Form1()
        {
            InitializeComponent();
            label13.Hide();
            pictureBox1.Hide();
            timer1.Tick += new EventHandler(timer1_Tick);
            //Prepare the drawing area
            pictureBox1.Location = new Point(12, 12);
            pictureBox1.Width = maxSize + 10;
            pictureBox1.Height = maxSize + 10;
            pictureBox1.BackColor = this.BackColor;
        }

        public void timer1_Tick(object sender, EventArgs e)
        {
            //Memorizing the last increment, to avoid the situation when pressing opposite arrow keys at the same time kills the snake instantly
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
            addPoint.X = snake.getEnd().point.X + incre.x;
            addPoint.Y = snake.getEnd().point.Y + incre.y;
            if (checkBox1.Checked)
                //if crossing the boundries is allowed, when the snake reaches the boundries, set it to appear from the other side
                turnAround(ref addPoint);
            if (addPoint.X > snakeCoord.count - 1 || addPoint.Y > snakeCoord.count - 1 || addPoint.X < 0 || addPoint.Y < 0)
            {
                //if the snake moves out of the boundries
                gameOver();
                return;
            }
            if (snake.overlaps(addPoint) || obs.overlaps(addPoint))
            {
                //if the snake hits any obstacles
                gameOver();
                return;
            }
            if (sectList.collision(apple.point, addPoint))
            {
                //if the snake swallows an apple, its length will increase by 1, and we need to set a new apple
                snake.append(addPoint, pictureBox1, snakeColor);
                setApple();
            }
            else
            {
                //normal movement of the snake, by adding a new square to the head, as well as removing one from the tail
                snake.append(addPoint, pictureBox1, snakeColor);
                snake.remove_front();
            }
        }

        void turnAround(ref Point addPoint)
        {
            //Allow the snake to turn around upon the edges
            if (addPoint.X < 0)
                addPoint.X += snakeCoord.count;
            if (addPoint.Y < 0)
                addPoint.Y += snakeCoord.count;
            if (addPoint.X > snakeCoord.count - 1)
                addPoint.X -= snakeCoord.count;
            if (addPoint.Y > snakeCoord.count - 1)
                addPoint.Y -= snakeCoord.count;
        }


        void initGame()
        {
            gameHandler = true; //set game on
            label13.Hide(); //"Game over" will be hidden
            pictureBox1.Show(); //Main drawing area
            int speed = trackBar1.Value + 1;    //moving speed of the snake 
            //using a power function to modulate the logarithmic sense for speed of human
            int interval = (int)(500 * Math.Pow(0.6, speed));
            timer1.Interval = interval;
            //using a static class to record how many grid lines to show
            snakeCoord.unit = trackBar2.Value * 4 + 10;
            snakeCoord.count = maxSize / snakeCoord.unit;   //"maxsize" is the size of the drawing area
            Point startPoint = new Point();
            //start at about the centre
            startPoint.X = snakeCoord.count / 2;
            startPoint.Y = snakeCoord.count / 2;
            //Establish an instance of the queue recording the snake itself
            //Construction the list using the max possible length of the snake, which is the square of the grid size
            snake = new sectList(snakeCoord.count * snakeCoord.count);
            //The initial snake consists of 2 squares at nearly the centre of the drawing area
            snake.append(startPoint, pictureBox1, snakeColor);
            incre.x = 1;
            incre.y = 0;
            incRec.x = incre.x;
            incRec.y = incre.y;
            startPoint.X += incre.x;
            startPoint.Y += incre.y;
            //The second square of the initial snake
            snake.append(startPoint, pictureBox1, snakeColor);
            label7.Hide();  //Hise "Press any key to begin"
            setObstacles();
            setApple();
            this.Refresh();
            timer1.Start();
            //The obstacles the grids are generated in initGame() above and will be fixed throughout the game; therefore their controls must be locked
            trackBar2.Enabled = false;
            trackBar4.Enabled = false;
        }

        void gameOver()
        {
            timer1.Stop();
            //Show "Game over" and "Press any key to begin"
            label7.Show();
            label13.Show();
            trackBar2.Enabled = true;
            checkBox1.Enabled = true;
            trackBar4.Enabled = true;
            //remove apple
            apple.picBox.Dispose();
            //remove all snake and obstacles
            snake.clear();
            obs.clear();
            gameHandler = false;    //game is off
            //hide the grid
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
            //If the apple coordinates overlaps the snake or obstacles, we need to re-generate. This can be worst O(n2), but generally O(1) in most cases
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
            e.SuppressKeyPress = true;  //To remove the annoying "ding" noise when a key is pressed
            //Press any key to start, but Esc is reserved for a total reset
            if (gameHandler == false && e.KeyCode != Keys.Escape)
            {
                initGame();
                return;
            }
            //Use arrow keys to control the increments
            if (e.KeyCode == Keys.Up || e.KeyCode == Keys.W)
            {
                if (incre.y > 0)
                    return; //no need to change, same below
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
            //Press Esc to reset. Resetting is different from game over; it will not show the "Game Over" label 
            if (e.KeyCode == Keys.Escape)
            {
                label7.Text = "Press any key to start";
                label7.Show();
                label13.Hide();
                pictureBox1.Hide();
                timer1.Stop();
                snake.clear();
                obs.clear();
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
            //To remove the annoying "ding" noise when a key is pressed
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
            //Generate the grid lines. Paint will be called upon form1_update
            Pen bluePen = new Pen(Color.LightBlue, 1);
            int[] coord = new int[snakeCoord.count + 1];
            for (int i = 0; i <= snakeCoord.count; i++)
            {
                //coordinates of each x or y grid lines. The drawing area is an exact square, so only one value is needed. Otherwise need to set x and y separately
                coord[i] = i * snakeCoord.unit;
            }
            //Horizontal grid lines
            for (int i = 0; i <= snakeCoord.count; i++)
            {
                e.Graphics.DrawLine(bluePen, 0, coord[i], snakeCoord.unit * (snakeCoord.count), coord[i]);
            }
            //Vertical grid lines
            for (int i = 0; i <= snakeCoord.count; i++)
            {
                e.Graphics.DrawLine(bluePen, coord[i], 0, coord[i], snakeCoord.unit * (snakeCoord.count));
            }
        }

        void setObstacles()
        {
            obs = new sectList(snakeCoord.count * snakeCoord.count);
            Random rnd = new Random();
            //using a power function to modulate the logarithmic sense of human
            int difficulty = (int)(Math.Pow(1.4, trackBar4.Value) + 5) * trackBar4.Value / (trackBar2.Value + 1);
            //Number of obstacle groups is "difficulty"
            for (int i = 0; i < difficulty; i++)
            {
                int x = -1;
                int y = -1;
                x = rnd.Next(0, snakeCoord.count);
                y = rnd.Next(0, snakeCoord.count);
                obs.append(new Point(x, y), pictureBox1, Color.Black);
                //Each obstacle group has the size of a random number between o and "difficulty * 2"
                //but they may overlap with each other, or may round up an extra space, which allows them to occupy a more variable area
                int size = rnd.Next(0, difficulty * 2);
                for (int j = 0; j < size; j++)
                {
                    int x1, y1;
                    do
                    {
                        x1 = rnd.Next(-1, 2);
                        y1 = rnd.Next(-1, 2);
                    } while (x + x1 < 0 || y + y1 < 0 || x + x1 >= snakeCoord.count || y + y1 >= snakeCoord.count); //Only valid inside the drawing area
                    obs.append(new Point(x + x1, y + y1), pictureBox1, Color.Black);
                }
            }
        }
        //Allow changing the game speed at any time
        private void trackBar1_Changed(object sender, EventArgs e)
        {
            int speed = trackBar1.Value + 1;
            int interval = (int)(500 * Math.Pow(0.6, speed));
            timer1.Interval = interval;
        }
    }
    //using a static class to record how many grid lines to show
    public static class snakeCoord
    {
        public static int unit = 22;
        public static int count = 27;
    }
    //using a static class to control the increment of the snake for each tick
    public static class incre
    {
        public static int x = 1;
        public static int y = 0;
    }
    //Memorizing the last increment, to avoid the situation when pressing opposite arrow keys at the same time kills the snake instantly
    public static class incRec
    {
        public static int x = 1;
        public static int y = 0;
    }
    //each section of the snake
    public class sect
    {
        public Point point;
        //Reference to the picturebox that will be attached. It provides a handle through which we can dispose the picturebox later
        public PictureBox picBox;
        //To convert the grid coordinates into the form's coordinates and put a square picturebox
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
    //a queue class for the snake
    public class sectList
    {
        int head = 0, end = 0, max;
        //The core list that stores all the snake body. 
        sect[] lst;
        public sect getEnd()
        {
            return lst[end];
        }
        public sect getHead()
        {
            return lst[head];
        }
        //Construction the list using the max possible length of the snake, which is the square of the grid size
        public sectList(int count)
        {
            lst = new sect[count];
            max = count - 1;
        }
        public void append(Point newCoord, Control ctrl, Color color)
        {
            //When it reaches the max, turn around and reuse the beginning section of the list
            if (lst[end] != null)
                end++;
            if (lst[end] == null)
                //new separetely helps to eliminate the lag when creating 700+ new instances
                lst[end] = new sect();
            lst[end].point = newCoord;
            lst[end].draw(color, ctrl);
        }

        public void remove_front()
        {
            //When it reaches the max, turn around and scan the beginning section of the list
            lst[head].picBox.Dispose();
            if (head == max)
                head = 0;
            else
                head++;
        }
        //To judge if a point overlaps with the snake/obstacle
        public Boolean overlaps(Point pScanned)
        {
            if (head == end)
            {
                if (lst[end] == null)
                    return false;
                else if (lst[head].point == pScanned)
                    return true;
                else
                    return false;
            }
            //if the queue has not turned around in the array
            if (head < end) 
            {
                for (int i = head; i <= end; i++)
                {
                    if (lst[i].point == pScanned)
                    {
                        return true;
                    }
                }
            }
            //if the queue has turned around in the array, the queue is separeted near the head and tail of the array
            else
            {
                for (int i = head; i <= max; i++)
                {
                    if (lst[i].point == pScanned)
                    {
                        return true;
                    }
                }
                for (int i = 0; i <= end; i++)
                {
                    if (lst[i].point == pScanned)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        //Remove all pictureboxes
        public void clear()
        {

            if (head == end)
            {
                //if the array is empty, do nothing
                if (lst[head] == null)
                    return;
                lst[head].picBox.Dispose();
                head = 0;
                end = 0;
            }
            //if the queue has not turned around in the array
            if (head < end)
            {
                for (int i = head; i <= end; i++)
                    lst[i].picBox.Dispose();
                head = 0;
                end = 0;
            }
            //if the queue has turned around in the array, the queue is separeted near the head and tail of the array
            else
            {
                for (int i = 0; i <= end; i++)
                    lst[i].picBox.Dispose();
                for (int i = head; i <= max; i++)
                    lst[i].picBox.Dispose();
                head = 0;
                end = 0;
            }
        }
        //To judge if the snake reaches an apple
        public static Boolean collision(Point a, Point b)
        {
            if (Math.Abs(a.X - b.X) < 1 && Math.Abs(a.Y - b.Y) < 1)
                return true;
            else
                return false;
        }
    }
}
