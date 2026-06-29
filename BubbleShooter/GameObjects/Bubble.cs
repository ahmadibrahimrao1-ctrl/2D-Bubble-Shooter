using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using BubbleShooter.Enums;
using BubbleShooter.Interfaces;

namespace BubbleShooter.GameObjects
{
    public class Bubble : GameObject, ICollidable
    {
        
        public const float Diameter = 40f;

        public const float Radius = Diameter / 2f;

        private BubbleColor _bubbleColor;
        private int _gridRow;
        private int _gridCol;
        private bool _isFlying;

        public BubbleColor BubbleColor
        {
            get { return _bubbleColor; }
            private set { _bubbleColor = value; }
        }

        public int GridRow
        {
            get { return _gridRow; }
            set { _gridRow = value; }
        }


        public int GridCol
        {
            get { return _gridCol; }
            set { _gridCol = value; }
        }

        public bool IsFlying
        {
            get { return _isFlying; }
            set { _isFlying = value; }
        }

        public float BoardWidth { get; set; }

        public Bubble(float x, float y, BubbleColor color)
            : base(x, y, Diameter, Diameter)
        {
            _bubbleColor = color;
            _gridRow = -1;
            _gridCol = -1;
            _isFlying = false;
            BoardWidth = 600f;
        }

        public override void Update()
        {
            if (!IsActive || !_isFlying) return;

            Position = new PointF(Position.X + Velocity.X, Position.Y + Velocity.Y);

            if (Position.X <= 0)
            {
                Position = new PointF(0, Position.Y);
                Velocity = new PointF(-Velocity.X, Velocity.Y);
            }

            if (Position.X + Diameter >= BoardWidth)
            {
                Position = new PointF(BoardWidth - Diameter, Position.Y);
                Velocity = new PointF(-Velocity.X, Velocity.Y);
            }
        }

        public override void Draw(Graphics g)
        {
            if (!IsActive) return;

            Color baseColor = GetDrawingColor(_bubbleColor);
            RectangleF rect = new RectangleF(Position.X, Position.Y, Diameter, Diameter);

            using (GraphicsPath path = new GraphicsPath())
            {
                path.AddEllipse(rect);
                using (PathGradientBrush gradient = new PathGradientBrush(path))
                {
                    gradient.CenterColor = Color.FromArgb(255,
                        Math.Min(baseColor.R + 80, 255),
                        Math.Min(baseColor.G + 80, 255),
                        Math.Min(baseColor.B + 80, 255));
                    gradient.SurroundColors = new Color[] { baseColor };
                    gradient.CenterPoint = new PointF(
                        Position.X + Diameter * 0.35f,
                        Position.Y + Diameter * 0.35f);
                    g.FillPath(gradient, path);
                }
            }

            using (Pen outlinePen = new Pen(Color.FromArgb(60, 0, 0, 0), 1.5f))
            {
                g.DrawEllipse(outlinePen, rect);
            }

            RectangleF highlight = new RectangleF(
                Position.X + Diameter * 0.25f,
                Position.Y + Diameter * 0.15f,
                Diameter * 0.25f,
                Diameter * 0.2f);
            using (SolidBrush shineBrush = new SolidBrush(Color.FromArgb(90, 255, 255, 255)))
            {
                g.FillEllipse(shineBrush, highlight);
            }
        }

        

        public bool CheckCollision(GameObject other)
        {
            if (!IsActive || !other.IsActive) return false;

            float myCenterX = Position.X + Radius;
            float myCenterY = Position.Y + Radius;
            float otherCenterX = other.Position.X + other.Size.Width / 2f;
            float otherCenterY = other.Position.Y + other.Size.Height / 2f;

            float dx = myCenterX - otherCenterX;
            float dy = myCenterY - otherCenterY;
            float distance = (float)Math.Sqrt(dx * dx + dy * dy);

            float otherRadius = Math.Min(other.Size.Width, other.Size.Height) / 2f;
            return distance < (Radius + otherRadius);
        }

        
        public static Color GetDrawingColor(BubbleColor c)
        {
            switch (c)
            {
                case BubbleColor.Red:    return Color.FromArgb(231, 76, 60);    // Vibrant Red
                case BubbleColor.Blue:   return Color.FromArgb(52, 152, 219);   // Bright Blue
                case BubbleColor.Green:  return Color.FromArgb(46, 204, 113);   // Emerald Green
                case BubbleColor.Yellow: return Color.FromArgb(241, 196, 15);   // Sunflower Yellow
                case BubbleColor.Purple: return Color.FromArgb(155, 89, 182);   // Amethyst Purple
                case BubbleColor.Orange: return Color.FromArgb(230, 126, 34);   // Carrot Orange
                default:                 return Color.Gray;
            }
        }

        
        public PointF GetCenter()
        {
            return new PointF(Position.X + Radius, Position.Y + Radius);
        }
    }
}
