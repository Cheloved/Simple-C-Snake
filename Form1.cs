using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Timers;

namespace Snake
{
    public enum Direction { Up, Down, Right, Left };
    public partial class MainForm : Form
    {
        int playSpeed = 150;
        System.Timers.Timer Frame_Timer;
        PictureBox lbl = new PictureBox();
        List<SnakePart> _Snake = new List<SnakePart>();
        Nyam n;
        Direction dir;

        public MainForm()
        {
            InitializeComponent();
            Frame_Timer = new System.Timers.Timer(playSpeed);
            this.Size = new System.Drawing.Size(220, 220);
            this.KeyDown += new KeyEventHandler(KeyDown_Event);
            dir = Direction.Right;
            Frame_Timer.Elapsed += new ElapsedEventHandler(Update);
              
            _Snake.Add(new SnakePart(80, 0, this));
            _Snake.Add(new SnakePart(60, 0, this));
            _Snake.Add(new SnakePart(40, 0, this));
            _Snake.Add(new SnakePart(20, 0, this));
            _Snake.Add(new SnakePart(0, 0, this));

            Random rand = new Random();
            n = new Nyam(rand.Next(0,11), rand.Next(0,11), this);
            Frame_Timer.Enabled = true;
        }

        private void Update(object owner, ElapsedEventArgs _event)
        {
            _Snake[0].MovePart(dir);
            MoveOthers();
            if(_Snake[0].Location == n.Location)
            {
                while (true)
                {
                    n.Respawn();
                    int times = 0;
                    for (int i = 0; i < _Snake.Count; i++)
                    {
                        if (n.Location == _Snake[i].Location)
                        {
                            times++;
                            break;
                        }
                    }
                    if (times == 0)
                        break;
                }
                addPart();
            }
            for(int i = 1; i < _Snake.Count; i++)
            {
                if(_Snake[0].Location == _Snake[i].Location)
                {
                    Frame_Timer.Enabled = false;
                    GameOver();
                    
                }
            }
        }
        public void MoveOthers()
        {
            for (int i = _Snake.Count - 1; i >= 1; i--)
            {
                _Snake[i].MoveAtPosition(_Snake[i - 1].oldPosition);
            }
        }
        private void KeyDown_Event(object owner, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                Application.Exit();
            if (e.KeyCode == Keys.R)
                Application.Restart();
            if(e.KeyCode == Keys.W)
                dir = Direction.Up;
            if (e.KeyCode == Keys.S)
                dir = Direction.Down;
            if (e.KeyCode == Keys.D)
                dir = Direction.Right;
            if (e.KeyCode == Keys.A)
                dir = Direction.Left;
        }
        void addPart()
        {
            int lastX = _Snake[_Snake.Count - 1].Location.X;
            int lastY = _Snake[_Snake.Count - 1].Location.Y;
            int prelastX = _Snake[_Snake.Count - 2].Location.X;
            int prelastY = _Snake[_Snake.Count - 2].Location.Y;
            int x = lastX - (prelastX - lastX);
            int y = lastY - (prelastY - lastY);
            this.Invoke((MethodInvoker)delegate
            {
                _Snake.Add(new SnakePart(x, y, this));
            });
        }
        public void GameOver()
        {
            for (int i = 0; i < _Snake.Count; i++)
            {
                _Snake[i] = null;
            }
            _Snake.Clear();
            Label lbl = new Label();
            lbl.Location = new Point(80, 100);
            lbl.Size = new System.Drawing.Size(65,15);
            lbl.Text = "Game Over!";
            lbl.BackColor = System.Drawing.Color.Gray;
            lbl.ForeColor = System.Drawing.Color.Red;
            lbl.Visible = true;
            lbl.BringToFront();
            this.Invoke((MethodInvoker)delegate
            {
                lbl.Parent = this;
            });
        }
    }
    public class Nyam : PictureBox
    {
        int startX;
        int startY;
        MainForm mf;
        public Nyam(int startX, int startY, MainForm mf)
        {
            this.startX = startX * 20;
            this.startY = startY * 20;
            this.mf = mf;
            Begin();
        }

        public void Begin()
        {
            this.Size = new Size(20, 20);
            this.Location = new Point(startX, startY);
            this.Image = Snake.Properties.Resources.nyam;
            this.Visible = true;
            this.Parent = mf;
        }
        public void Respawn()
        {
            Random random = new Random();
            int x = random.Next(0, 11) * 20;
            int y = random.Next(0, 11) * 20;
            
            this.Invoke((MethodInvoker)delegate
            {
                this.Location = new Point(x, y);
            });
        }

    }
    public class SnakePart : PictureBox
    {
        int startX;
        int startY;

        MainForm mf;

        public Point oldPosition;
        public SnakePart(int startX, int startY, MainForm mf)
        {
            this.startX = startX;
            this.startY = startY;
            this.mf = mf;
            Begin();
        }

        public void Begin()
        {
            this.Size = new Size(20, 20);
            this.Location = new Point(startX, startY);
            oldPosition = this.Location;
            this.Image = Snake.Properties.Resources.square;
            this.Visible = true;
            this.Parent = mf;
        }

        public void MoveAtPosition(Point pos)
        {
            this.Invoke((MethodInvoker)delegate
            {
                this.Location = pos;
                oldPosition = this.Location;
            });
        }

        public void MovePart(Direction dir)
        {
            oldPosition = this.Location;
            if(dir == Direction.Up)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    this.Location = new Point(this.Location.X, this.Location.Y - 20);
                });
            }
            if (dir == Direction.Down)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    this.Location = new Point(this.Location.X, this.Location.Y + 20);
                });
            }
            if (dir == Direction.Right)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    this.Location = new Point(this.Location.X + 20, this.Location.Y);
                });
            }
            if (dir == Direction.Left)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    this.Location = new Point(this.Location.X - 20, this.Location.Y);
                });
            }
            if (this.Location.X >= mf.Size.Width)
                this.Invoke((MethodInvoker)delegate
                {
                    this.Location = new Point(0, this.Location.Y);
                });
            if (this.Location.X < 0)
                this.Invoke((MethodInvoker)delegate
                {
                    this.Location = new Point(mf.Size.Width - 20, this.Location.Y);
                });
            if (this.Location.Y >= mf.Size.Height)
                this.Invoke((MethodInvoker)delegate
                {
                    this.Location = new Point(this.Location.X, 0);
                });
            if (this.Location.Y < 0)
                this.Invoke((MethodInvoker)delegate
                {
                    this.Location = new Point(this.Location.X, mf.Size.Height - 20);
                });
        }
    }
}
