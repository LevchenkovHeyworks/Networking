using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using Newtonsoft.Json;
using Playground.Client.WinForms;
using Timer = System.Windows.Forms.Timer;

namespace Playground.WinForms
{
    public partial class Form1 : Form
    {
        private const int DrawUpdatesPerSec = 60;
        private const int NetworkUpdatesPerSec = 20;

        private Field field;
        private Timer drawTimer;
        private Timer networkTimer;

        private Random random;

        private readonly LiteNetLibClient client;

        public Form1()
        {
            Thread.Sleep(1000);
            InitializeComponent();
            Width = 800;
            Height = 600;
            DoubleBuffered = true;

            random = new Random();
            client = new LiteNetLibClient();
            client.MessageReceived += ClientOnMessageReceived;
            client.Connected += ClientOnConnected;
            client.Disconected += ClientOnDisconected;
            client.Connect();

            field = new Field(Width - 50, Height - 50);

            drawTimer = new Timer();
            drawTimer.Interval = 1000 / DrawUpdatesPerSec; // 60 fps
            drawTimer.Tick += Draw_Tick;

            drawTimer.Start();

            networkTimer = new Timer();
            networkTimer.Interval = 1000 / NetworkUpdatesPerSec;
            networkTimer.Tick += Network_Tick;

            networkTimer.Start();

            label2.Text = "0";
            label4.Text = "Disconnected";
            label6.Text = "0";
            label8.Text = "0";
        }

        private void ClientOnDisconected(object sender, EventArgs eventargs)
        {
            label4.Invoke((MethodInvoker)delegate {
                label4.Text = "Disconnected";
            });
        }

        private void ClientOnConnected(object sender, EventArgs eventargs)
        {
            label4.Invoke((MethodInvoker)delegate {
                label4.Text = "Connected";
            });
        }

        private void ClientOnMessageReceived(object sender, string message)
        {
            var fieldInfo = JsonConvert.DeserializeObject<FieldInfo>(message);
            field.Info = fieldInfo;
        }

        private void Network_Tick(object sender, EventArgs e)
        {
            client.PollEvents();
        }

        private void Draw_Tick(object sender, System.EventArgs e)
        {
            this.Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            DrawField(e.Graphics);

            e.Graphics.DrawRectangle(Pens.Blue, 0, 0, field.Info.Width, field.Info.Height);

            label2.Text = field.Info.Circles.Count.ToString();
        }

        private void DrawField(Graphics g)
        {
            foreach (var item in field.Info.Circles)
            {
                DrawCircle(g, item.Info.Color, (int)item.Info.Position.X, (int)item.Info.Position.Y, item.Info.Diameter);
            }
        }

        private void DrawCircle(Graphics g, Color color, int x, int y, int diam)
        {
            var brush = new SolidBrush(color);
            g.FillEllipse(brush, x, y, diam, diam);
        }
    }

    public class Field
    {
        public Field(int width, int height)
        {
            Info = new FieldInfo(width, height);
        }

        public FieldInfo Info { get; set; }

        public void Update()
        {
            foreach (var item in Info.Circles)
            {
                UpdateCircle(item);
            }
        }

        private void UpdateCircle(Circle circle)
        {
            circle.Info.Position.X += circle.Velocity.X;
            circle.Info.Position.Y += circle.Velocity.Y;

            if (circle.Info.Position.X < 0)
            {
                circle.Info.Position.X = -circle.Info.Position.X;
                circle.Velocity.X = -circle.Velocity.X;
            }

            if(circle.Info.Position.Y < 0)
            {
                circle.Info.Position.Y = -circle.Info.Position.Y;
                circle.Velocity.Y = -circle.Velocity.Y;
            }

            var width = Info.Width - circle.Info.Diameter;
            if (circle.Info.Position.X > width)
            {
                circle.Info.Position.X = width - (circle.Info.Position.X - width);
                circle.Velocity.X = -circle.Velocity.X;
            }

            var height = Info.Height - circle.Info.Diameter;
            if (circle.Info.Position.Y > height)
            {
                circle.Info.Position.Y = height - (circle.Info.Position.Y - height);
                circle.Velocity.Y = -circle.Velocity.Y;
            }
        }
    }

    public class FieldInfo
    {
        public int Height { get; set; }

        public int Width { get; set; }

        public List<Circle> Circles { get; set; }

        public FieldInfo(int width, int height)
        {
            Height = height;
            Width = width;
            Circles = new List<Circle>();
        }
    }

    public class Vector2
    {
        public Vector2()
        {
        }

        public Vector2(float x, float y)
        {
            X = x;
            Y = y;
        }

        public float X { get; set; }

        public float Y { get; set; }
    }

    public class Circle
    {
        public CircleInfo Info { get; set; }

        public Vector2 Velocity { get; set; }
    }

    public class CircleInfo
    {
        public int Diameter { get; set; }

        public Vector2 Position { get; set; }

        public Color Color { get; set; }
    }
}
