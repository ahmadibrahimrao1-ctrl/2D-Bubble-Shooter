using System;
using System.Drawing;

namespace BubbleShooter.GameObjects
{
    public abstract class GameObject
    {
        private PointF _position;
        private SizeF _size;
        private PointF _velocity;
        private bool _isActive;

        
        public PointF Position
        {
            get { return _position; }
            set { _position = value; }
        }

        public SizeF Size
        {
            get { return _size; }
            protected set { _size = value; }
        }

        public PointF Velocity
        {
            get { return _velocity; }
            set { _velocity = value; }
        }

        public bool IsActive
        {
            get { return _isActive; }
            set { _isActive = value; }
        }

        public RectangleF Bounds
        {
            get { return new RectangleF(_position, _size); }
        }

        /// <param name="x">Initial X coordinate.</param>
        /// <param name="y">Initial Y coordinate.</param>
        /// <param name="width">Width of the object.</param>
        /// <param name="height">Height of the object.</param>
        protected GameObject(float x, float y, float width, float height)
        {
            _position = new PointF(x, y);
            _size = new SizeF(width, height);
            _velocity = new PointF(0, 0);
            _isActive = true;
        }

        
        public abstract void Update();

        public abstract void Draw(Graphics g);
    }
}
