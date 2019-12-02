using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Playground.WinForms
{
    public partial class Form1 : Form
    {
        private Field field;
        private Timer updateTimer;
        private Timer drawTimer;

        private Random random;

        public Form1()
        {
            InitializeComponent();
            Width = 800;
            Height = 600;
            DoubleBuffered = true;

            random = new Random();

            this.MouseClick += Click;

            field = new Field(Width - 50, Height - 50);

            updateTimer = new Timer();
            updateTimer.Interval = 1000 / 60; // 20 times per sec
            updateTimer.Tick += Update_Tick;

            updateTimer.Start();

            drawTimer = new Timer();
            drawTimer.Interval = 1000 / 60; // 60 fps
            drawTimer.Tick += Draw_Tick;

            drawTimer.Start();

            label2.Text = "0";
            label4.Text = "Disconnected";
            label6.Text = "0";
            label8.Text = "0";
        }

        private void Update_Tick(object sender, System.EventArgs e)
        {
            field.Update();
        }

        private void Draw_Tick(object sender, System.EventArgs e)
        {
            this.Invalidate();
        }

        private void Click(object sender, MouseEventArgs e)
        {            
            field.Info.Circles.Add(CreateCircle(e.X, e.Y, 1, 0));            
            field.Info.Circles.Add(CreateCircle(e.X, e.Y, -1, 0));
            field.Info.Circles.Add(CreateCircle(e.X, e.Y, 0, 1));
            field.Info.Circles.Add(CreateCircle(e.X, e.Y, 0, -1));

            field.Info.Circles.Add(CreateCircle(e.X, e.Y, 0.5f, 0.5f));
            field.Info.Circles.Add(CreateCircle(e.X, e.Y, -0.5f, 0.5f));
            field.Info.Circles.Add(CreateCircle(e.X, e.Y, 0.5f, -0.5f));
            field.Info.Circles.Add(CreateCircle(e.X, e.Y, -0.5f, -0.5f));

            label2.Text = field.Info.Circles.Count.ToString();
        }

        private Color GetColor(int velocity)
        {
            var i = (velocity * 255 / 20);
            var r = (int) Math.Round(Math.Sin(0.024 * i + 0) * 127 + 128);
            var g = (int) Math.Round(Math.Sin(0.024 * i + 2) * 127 + 128);
            var b = (int) Math.Round(Math.Sin(0.024 * i + 4) * 127 + 128);
            return Color.FromArgb(r, g, b);
        }

        private Circle CreateCircle(int x, int y, float vx, float vy)
        {
            int v = random.Next(5, 10);
            var color = GetColor(v);
            var circle = new Circle { Info = new CircleInfo { Position = new Vector2(x, y), Color = color }, Velocity = new Vector2(vx * v, vy * v) };
            return circle;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            DrawField(e.Graphics);

            e.Graphics.DrawRectangle(Pens.Blue, 0, 0, field.Info.Width, field.Info.Height);
        }

        private void DrawField(Graphics g)
        {
            foreach (var item in field.Info.Circles)
            {
                DrawCircle(g, item.Info.Color, (int)item.Info.Position.X, (int)item.Info.Position.Y);
            }
        }

        private void DrawCircle(Graphics g, Color color, int x, int y)
        {
            var pen = new Pen(color);
            g.DrawEllipse(pen, x, y, CircleInfo.Diameter, CircleInfo.Diameter);
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

            var width = Info.Width - CircleInfo.Diameter;
            if (circle.Info.Position.X > width)
            {
                circle.Info.Position.X = width - (circle.Info.Position.X - width);
                circle.Velocity.X = -circle.Velocity.X;
            }

            var height = Info.Height - CircleInfo.Diameter;
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
        public const int Diameter = 10;

        public Vector2 Position { get; set; }

        public Color Color { get; set; }
    }
}
