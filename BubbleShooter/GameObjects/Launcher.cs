using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using BubbleShooter.Enums;

namespace BubbleShooter.GameObjects
{
    public class Launcher : GameObject
    {
        // ===== Constants =====
        private const float MinAngle = 0.26f;
        private const float MaxAngle = 2.88f;
        private const float CannonLength = 50f;
        private const float CannonWidth = 12f;

        private float _angle;
        private Bubble _loadedBubble;

        // ===== Public Properties =====
        public float Angle
        {
            get { return _angle; }
            private set
            {
                _angle = Math.Max(MinAngle, Math.Min(MaxAngle, value));
            }
        }

        public Bubble LoadedBubble
        {
            get { return _loadedBubble; }
            private set { _loadedBubble = value; }
        }

        public Launcher(float x, float y)
            : base(x - 25f, y - 25f, 50f, 50f)
        {
            _angle = (float)(Math.PI / 2);
            _loadedBubble = null;
        }

        public void SetAngle(PointF mousePos)
        {
            float centerX = Position.X + Size.Width / 2f;
            float centerY = Position.Y + Size.Height / 2f;

            float dx = mousePos.X - centerX;
            float dy = centerY - mousePos.Y;

            if (dy > 0)
            {
                Angle = (float)Math.Atan2(dy, dx);
            }
        }

        public Bubble Fire(float speed)
        {
            if (_loadedBubble == null) return null;

            Bubble firedBubble = _loadedBubble;

            float vx = (float)(Math.Cos(_angle)) * speed;
            float vy = -(float)(Math.Sin(_angle)) * speed;

            firedBubble.Velocity = new PointF(vx, vy);
            firedBubble.IsFlying = true;

            float centerX = Position.X + Size.Width / 2f;
            float centerY = Position.Y + Size.Height / 2f;
            float tipX = centerX + (float)(Math.Cos(_angle)) * CannonLength - Bubble.Radius;
            float tipY = centerY - (float)(Math.Sin(_angle)) * CannonLength - Bubble.Radius;
            firedBubble.Position = new PointF(tipX, tipY);

            _loadedBubble = null;
            return firedBubble;
        }


        public void LoadBubble(Bubble bubble)
        {
            _loadedBubble = bubble;
            if (_loadedBubble != null)
            {
                // Position the loaded bubble at the launcher center
                float centerX = Position.X + Size.Width / 2f - Bubble.Radius;
                float centerY = Position.Y + Size.Height / 2f - Bubble.Radius;
                _loadedBubble.Position = new PointF(centerX, centerY);
                _loadedBubble.IsFlying = false;
            }
        }

        public override void Update()
        {
            if (_loadedBubble != null && !_loadedBubble.IsFlying)
            {
                float centerX = Position.X + Size.Width / 2f - Bubble.Radius;
                float centerY = Position.Y + Size.Height / 2f - Bubble.Radius;
                _loadedBubble.Position = new PointF(centerX, centerY);
            }
        }

        public override void Draw(Graphics g)
        {
            if (!IsActive) return;

            float centerX = Position.X + Size.Width / 2f;
            float centerY = Position.Y + Size.Height / 2f;

            GraphicsState state = g.Save();

            g.TranslateTransform(centerX, centerY);
            g.RotateTransform(-(float)(_angle * 180.0 / Math.PI));

            RectangleF barrel = new RectangleF(0, -CannonWidth / 2f, CannonLength, CannonWidth);
            using (LinearGradientBrush barrelBrush = new LinearGradientBrush(
                barrel, Color.FromArgb(100, 100, 110), Color.FromArgb(60, 60, 70),
                LinearGradientMode.Vertical))
            {
                g.FillRectangle(barrelBrush, barrel);
            }
            using (Pen barrelOutline = new Pen(Color.FromArgb(40, 40, 50), 1.5f))
            {
                g.DrawRectangle(barrelOutline, barrel.X, barrel.Y, barrel.Width, barrel.Height);
            }

            g.Restore(state);

            float baseRadius = 22f;
            RectangleF baseRect = new RectangleF(
                centerX - baseRadius, centerY - baseRadius,
                baseRadius * 2, baseRadius * 2);
            using (LinearGradientBrush baseBrush = new LinearGradientBrush(
                baseRect, Color.FromArgb(80, 80, 90), Color.FromArgb(50, 50, 60),
                LinearGradientMode.ForwardDiagonal))
            {
                g.FillEllipse(baseBrush, baseRect);
            }
            using (Pen basePen = new Pen(Color.FromArgb(100, 180, 180, 190), 2f))
            {
                g.DrawEllipse(basePen, baseRect);
            }

            if (_loadedBubble != null && !_loadedBubble.IsFlying)
            {
                _loadedBubble.Draw(g);
            }

            float guideLength = 120f;
            float guideEndX = centerX + (float)(Math.Cos(_angle)) * guideLength;
            float guideEndY = centerY - (float)(Math.Sin(_angle)) * guideLength;
            using (Pen guidePen = new Pen(Color.FromArgb(80, 255, 255, 255), 1.5f))
            {
                guidePen.DashStyle = DashStyle.Dot;
                g.DrawLine(guidePen, centerX, centerY, guideEndX, guideEndY);
            }
        }
    }
}
