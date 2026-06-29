using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;
using BubbleShooter.Enums;
using BubbleShooter.GameObjects;

namespace BubbleShooter.Managers
{
    public class GameController
    {
        private const float BubbleSpeed = 12f;
        private const int InitialRows = 6;
        private const float BoardWidth = 600f;
        private const float BoardHeight = 700f;
        private const float BoundaryY = 600f;
        private const float LauncherY = 660f;

        private List<GameObject> _gameObjects;
        private GridManager _gridManager;
        private CollisionManager _collisionManager;
        private InputManager _inputManager;
        private ScoreManager _scoreManager;
        private Launcher _launcher;
        private Bubble _flyingBubble;
        private Bubble _nextBubble;

        private GameState _currentState;
        private Random _random;

        public GameState CurrentState
        {
            get { return _currentState; }
        }

        public int Score
        {
            get { return _scoreManager.Score; }
        }

        public int HighScore
        {
            get { return _scoreManager.HighScore; }
        }

        public event Action<int> OnScoreChanged;

        public event Action<GameState> OnStateChanged;

        public GameController()
        {
            _gameObjects = new List<GameObject>();
            _gridManager = new GridManager();
            _collisionManager = new CollisionManager();
            _inputManager = new InputManager();
            _scoreManager = new ScoreManager();
            _random = new Random();
            _currentState = GameState.Menu;
        }

        public void StartGame()
        {
            _gameObjects.Clear();

            _gridManager.InitializeGrid(InitialRows, BoardWidth);

            _launcher = new Launcher(BoardWidth / 2f, LauncherY);
            _gameObjects.Add(_launcher);

            _scoreManager.Reset();
            OnScoreChanged?.Invoke(_scoreManager.Score);

            _flyingBubble = null;
            _nextBubble = CreateRandomBubble();
            LoadNextBubble();

            _currentState = GameState.Playing;
            OnStateChanged?.Invoke(_currentState);
        }

        public void PauseGame()
        {
            if (_currentState == GameState.Playing)
            {
                _currentState = GameState.Paused;
                OnStateChanged?.Invoke(_currentState);
            }
        }


        public void ResumeGame()
        {
            if (_currentState == GameState.Paused)
            {
                _currentState = GameState.Playing;
                OnStateChanged?.Invoke(_currentState);
            }
        }

        public void RestartGame()
        {
            StartGame();
        }

        public void Update()
        {
            if (_currentState != GameState.Playing) return;

            if (_inputManager.ShouldPause)
            {
                PauseGame();
                return;
            }

            _launcher.SetAngle(new PointF(
                _launcher.Position.X + _launcher.Size.Width / 2f + 
                (float)(Math.Cos(_inputManager.CurrentAngle) * 100),
                _launcher.Position.Y + _launcher.Size.Height / 2f - 
                (float)(Math.Sin(_inputManager.CurrentAngle) * 100)));

            if (_inputManager.ShouldFire && _flyingBubble == null)
            {
                FireBubble();
            }

            foreach (GameObject obj in _gameObjects)
            {
                if (obj.IsActive)
                {
                    obj.Update();
                }
            }

            if (_flyingBubble != null && _flyingBubble.IsActive)
            {
                _flyingBubble.Update();
            }

            if (_flyingBubble != null && _flyingBubble.IsFlying)
            {
                bool hitGrid = _collisionManager.CheckBubbleGridCollision(
                    _flyingBubble, _gridManager.Grid, _gridManager.Rows, _gridManager.Cols);
                bool hitCeiling = _collisionManager.CheckCeilingCollision(_flyingBubble, 0f);

                if (hitGrid || hitCeiling)
                {
                    HandleBubbleLanded();
                }
            }
        }

        public void Render(Graphics g)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            DrawBackground(g);

            if (_currentState == GameState.Menu)
            {
                DrawMenuScreen(g);
                return;
            }

            using (Pen boundaryPen = new Pen(Color.FromArgb(120, 231, 76, 60), 2f))
            {
                boundaryPen.DashStyle = DashStyle.Dash;
                g.DrawLine(boundaryPen, 0, BoundaryY, BoardWidth, BoundaryY);
            }

            _gridManager.DrawGrid(g);

            foreach (GameObject obj in _gameObjects)
            {
                if (obj.IsActive)
                {
                    obj.Draw(g);
                }
            }

            if (_flyingBubble != null && _flyingBubble.IsActive)
            {
                _flyingBubble.Draw(g);
            }

            DrawNextBubblePreview(g);

            DrawScoreOverlay(g);

            if (_currentState == GameState.Paused)
            {
                DrawPauseOverlay(g);
            }

            if (_currentState == GameState.GameOver)
            {
                DrawGameOverOverlay(g);
            }
        }

 
        public void OnMouseMove(Point pos)
        {
            if (_currentState != GameState.Playing) return;
            PointF launcherCenter = new PointF(
                _launcher.Position.X + _launcher.Size.Width / 2f,
                _launcher.Position.Y + _launcher.Size.Height / 2f);
            _inputManager.HandleMouseMove(pos, launcherCenter);

            _launcher.SetAngle(pos);
        }

        public void OnMouseClick(Point pos)
        {
            if (_currentState == GameState.Playing)
            {
                _inputManager.HandleMouseClick();
            }
        }

        /// <summary>Forwards keyboard events to the InputManager.</summary>
        public void OnKeyDown(Keys key)
        {
            _inputManager.HandleKeyDown(key);
        }

        // ===== Private Helper Methods =====

        /// <summary>
        /// Fires the loaded bubble from the launcher.
        /// </summary>
        private void FireBubble()
        {
            if (_launcher.LoadedBubble == null) return;

            _flyingBubble = _launcher.Fire(BubbleSpeed);
            if (_flyingBubble != null)
            {
                _flyingBubble.BoardWidth = BoardWidth;
            }
        }

        /// <summary>
        /// Handles the logic when a flying bubble lands (hits grid or ceiling).
        /// Snaps to grid, checks for matching clusters, pops them, drops disconnected bubbles.
        /// </summary>
        private void HandleBubbleLanded()
        {
            if (_flyingBubble == null) return;

            // Find nearest empty grid cell
            PointF center = _flyingBubble.GetCenter();
            var (row, col) = _gridManager.GetNearestEmptyCell(center);

            if (row < 0 || col < 0)
            {
                // No valid cell found - game over
                _flyingBubble.IsActive = false;
                _currentState = GameState.GameOver;
                OnStateChanged?.Invoke(_currentState);
                return;
            }

            // Snap bubble into the grid
            _gridManager.SnapBubble(_flyingBubble, row, col);

            // Find matching cluster
            List<Bubble> cluster = _gridManager.FindMatchingCluster(row, col);

            if (cluster.Count >= 3)
            {
                // Pop the matching cluster
                _gridManager.PopCluster(cluster);
                _scoreManager.AddClusterPoints(cluster.Count);

                // Find and drop disconnected bubbles
                List<Bubble> disconnected = _gridManager.FindDisconnectedBubbles();
                if (disconnected.Count > 0)
                {
                    _gridManager.DropDisconnected(disconnected);
                    _scoreManager.AddDropBonus(disconnected.Count);
                }

                OnScoreChanged?.Invoke(_scoreManager.Score);
            }

            // Check for game over
            if (_gridManager.IsGameOver(BoundaryY))
            {
                _currentState = GameState.GameOver;
                OnStateChanged?.Invoke(_currentState);
                return;
            }

            // Check for win (all bubbles cleared)
            if (_gridManager.GetAllActiveBubbles().Count == 0)
            {
                _scoreManager.AddPoints(100); // Win bonus
                OnScoreChanged?.Invoke(_scoreManager.Score);
                _currentState = GameState.GameOver; // Could be a Win state
                OnStateChanged?.Invoke(_currentState);
                return;
            }

            // Load next bubble
            _flyingBubble = null;
            LoadNextBubble();
        }

        /// <summary>
        /// Loads the next bubble into the launcher and prepares a new next bubble.
        /// </summary>
        private void LoadNextBubble()
        {
            _launcher.LoadBubble(_nextBubble);
            _nextBubble = CreateRandomBubble();
        }

        /// <summary>
        /// Creates a new bubble with a random color chosen from colors still present in the grid.
        /// </summary>
        private Bubble CreateRandomBubble()
        {
            BubbleColor color = _gridManager.GetRandomColorFromGrid();
            return new Bubble(0, 0, color);
        }

        // ===== Drawing Helpers =====

        /// <summary>Draws the game background with a subtle gradient.</summary>
        private void DrawBackground(Graphics g)
        {
            RectangleF bgRect = new RectangleF(0, 0, BoardWidth, BoardHeight);
            using (LinearGradientBrush bgBrush = new LinearGradientBrush(
                bgRect, Color.FromArgb(15, 15, 30), Color.FromArgb(25, 25, 50),
                LinearGradientMode.Vertical))
            {
                g.FillRectangle(bgBrush, bgRect);
            }

            // Draw subtle side borders
            using (Pen borderPen = new Pen(Color.FromArgb(40, 100, 100, 120), 2f))
            {
                g.DrawLine(borderPen, 0, 0, 0, BoardHeight);
                g.DrawLine(borderPen, BoardWidth - 1, 0, BoardWidth - 1, BoardHeight);
            }
        }

        /// <summary>Draws the menu/start screen.</summary>
        private void DrawMenuScreen(Graphics g)
        {
            using (Font titleFont = new Font("Segoe UI", 36f, FontStyle.Bold))
            using (Font subFont = new Font("Segoe UI", 14f, FontStyle.Regular))
            using (StringFormat sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
            {
                // Title
                RectangleF titleRect = new RectangleF(0, 200, BoardWidth, 60);
                g.DrawString("BUBBLE SHOOTER", titleFont, Brushes.White, titleRect, sf);

                // Subtitle
                RectangleF subRect = new RectangleF(0, 280, BoardWidth, 40);
                using (SolidBrush subBrush = new SolidBrush(Color.FromArgb(180, 200, 200, 220)))
                {
                    g.DrawString("Click 'Start' to begin!", subFont, subBrush, subRect, sf);
                }
            }
        }

        /// <summary>Draws the next bubble preview in the bottom-left corner.</summary>
        private void DrawNextBubblePreview(Graphics g)
        {
            if (_nextBubble == null) return;

            using (Font labelFont = new Font("Segoe UI", 9f, FontStyle.Regular))
            using (SolidBrush labelBrush = new SolidBrush(Color.FromArgb(180, 200, 200, 220)))
            {
                g.DrawString("Next:", labelFont, labelBrush, 15f, LauncherY - 55f);
            }

            // Draw the preview bubble
            Bubble preview = new Bubble(20f, LauncherY - 40f, _nextBubble.BubbleColor);
            preview.Draw(g);
        }

        /// <summary>Draws the score display.</summary>
        private void DrawScoreOverlay(Graphics g)
        {
            using (Font scoreFont = new Font("Segoe UI", 12f, FontStyle.Bold))
            using (SolidBrush scoreBrush = new SolidBrush(Color.FromArgb(220, 255, 255, 255)))
            {
                g.DrawString($"Score: {_scoreManager.Score}", scoreFont, scoreBrush, BoardWidth - 150f, 10f);
            }
        }

        /// <summary>Draws the pause screen overlay.</summary>
        private void DrawPauseOverlay(Graphics g)
        {
            using (SolidBrush overlay = new SolidBrush(Color.FromArgb(150, 0, 0, 0)))
            {
                g.FillRectangle(overlay, 0, 0, BoardWidth, BoardHeight);
            }

            using (Font pauseFont = new Font("Segoe UI", 32f, FontStyle.Bold))
            using (StringFormat sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
            {
                RectangleF pauseRect = new RectangleF(0, 300, BoardWidth, 60);
                g.DrawString("PAUSED", pauseFont, Brushes.White, pauseRect, sf);
            }

            using (Font subFont = new Font("Segoe UI", 12f, FontStyle.Regular))
            using (StringFormat sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
            using (SolidBrush subBrush = new SolidBrush(Color.FromArgb(180, 200, 200, 220)))
            {
                RectangleF subRect = new RectangleF(0, 360, BoardWidth, 30);
                g.DrawString("Press 'P' or click Resume to continue", subFont, subBrush, subRect, sf);
            }
        }

        /// <summary>Draws the game over screen overlay.</summary>
        private void DrawGameOverOverlay(Graphics g)
        {
            using (SolidBrush overlay = new SolidBrush(Color.FromArgb(180, 0, 0, 0)))
            {
                g.FillRectangle(overlay, 0, 0, BoardWidth, BoardHeight);
            }

            using (Font gameOverFont = new Font("Segoe UI", 36f, FontStyle.Bold))
            using (StringFormat sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
            {
                RectangleF goRect = new RectangleF(0, 250, BoardWidth, 60);
                using (SolidBrush goBrush = new SolidBrush(Color.FromArgb(231, 76, 60)))
                {
                    g.DrawString("GAME OVER", gameOverFont, goBrush, goRect, sf);
                }
            }

            using (Font scoreFont = new Font("Segoe UI", 18f, FontStyle.Bold))
            using (StringFormat sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
            {
                RectangleF scoreRect = new RectangleF(0, 320, BoardWidth, 40);
                g.DrawString($"Final Score: {_scoreManager.Score}", scoreFont, Brushes.White, scoreRect, sf);

                RectangleF highRect = new RectangleF(0, 360, BoardWidth, 40);
                using (SolidBrush highBrush = new SolidBrush(Color.FromArgb(241, 196, 15)))
                {
                    g.DrawString($"High Score: {_scoreManager.HighScore}", scoreFont, highBrush, highRect, sf);
                }
            }

            using (Font subFont = new Font("Segoe UI", 12f, FontStyle.Regular))
            using (StringFormat sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
            using (SolidBrush subBrush = new SolidBrush(Color.FromArgb(180, 200, 200, 220)))
            {
                RectangleF restartRect = new RectangleF(0, 410, BoardWidth, 30);
                g.DrawString("Click 'Restart' to play again", subFont, subBrush, restartRect, sf);
            }
        }
    }
}
