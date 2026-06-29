using System;
using System.Drawing;
using System.Windows.Forms;

namespace BubbleShooter.Managers
{
    /// <summary>
    /// Translates raw WinForms input events into game actions.
    /// Handles mouse aiming, click-to-fire, and keyboard controls.
    /// </summary>
    public class InputManager
    {
        // ===== Private fields =====
        private float _currentAngle;
        private bool _shouldFire;
        private bool _shouldPause;

        // ===== Public Properties =====

        /// <summary>
        /// Gets the current aim angle in radians, computed from mouse position.
        /// </summary>
        public float CurrentAngle
        {
            get { return _currentAngle; }
        }

        /// <summary>
        /// Gets whether a fire action has been requested. Resets after being read.
        /// </summary>
        public bool ShouldFire
        {
            get
            {
                bool val = _shouldFire;
                _shouldFire = false; // Auto-reset after reading
                return val;
            }
        }

        /// <summary>
        /// Gets whether a pause toggle has been requested. Resets after being read.
        /// </summary>
        public bool ShouldPause
        {
            get
            {
                bool val = _shouldPause;
                _shouldPause = false;
                return val;
            }
        }

        // ===== Constructor =====

        /// <summary>
        /// Initializes the InputManager with default values.
        /// </summary>
        public InputManager()
        {
            _currentAngle = (float)(Math.PI / 2); // Default: straight up
            _shouldFire = false;
            _shouldPause = false;
        }

        // ===== Public Methods =====

        /// <summary>
        /// Processes mouse movement to compute the aim angle.
        /// </summary>
        /// <param name="mousePos">Current mouse position in game panel coordinates.</param>
        /// <param name="launcherPos">Center position of the launcher.</param>
        public void HandleMouseMove(Point mousePos, PointF launcherPos)
        {
            float dx = mousePos.X - launcherPos.X;
            float dy = launcherPos.Y - mousePos.Y; // Inverted Y for screen coordinates

            if (dy > 10) // Only aim when mouse is sufficiently above launcher
            {
                _currentAngle = (float)Math.Atan2(dy, dx);

                // Clamp to prevent horizontal/downward shots
                _currentAngle = Math.Max(0.26f, Math.Min(2.88f, _currentAngle));
            }
        }

        /// <summary>
        /// Processes a mouse click event (fire a bubble).
        /// </summary>
        public void HandleMouseClick()
        {
            _shouldFire = true;
        }

        /// <summary>
        /// Processes keyboard input for game controls.
        /// Arrow keys: fine-tune aim angle. Space: fire. P: pause.
        /// </summary>
        /// <param name="key">The key that was pressed.</param>
        public void HandleKeyDown(Keys key)
        {
            switch (key)
            {
                case Keys.Left:
                    _currentAngle = Math.Min(_currentAngle + 0.05f, 2.88f);
                    break;
                case Keys.Right:
                    _currentAngle = Math.Max(_currentAngle - 0.05f, 0.26f);
                    break;
                case Keys.Space:
                    _shouldFire = true;
                    break;
                case Keys.P:
                    _shouldPause = true;
                    break;
            }
        }
    }
}
