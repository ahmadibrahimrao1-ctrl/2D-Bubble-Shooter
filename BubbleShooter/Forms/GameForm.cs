using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using BubbleShooter.Enums;
using BubbleShooter.Managers;

namespace BubbleShooter.Forms
{
    /// <summary>
    /// The main game window. Acts as the UI shell and delegates all game logic
    /// to the composed GameController instance.
    /// Demonstrates COMPOSITION: the Form owns and orchestrates the GameController.
    /// </summary>
    public partial class GameForm : Form
    {
        // ===== Composed game controller (Composition) =====
        private GameController _gameController;

        // ===== Constructor =====

        /// <summary>
        /// Initializes the GameForm and sets up the game controller.
        /// </summary>
        public GameForm()
        {
            InitializeComponent();

            // Enable double-buffering on the game panel to prevent flickering
            // Using reflection because Panel.DoubleBuffered is protected
            typeof(Panel).InvokeMember(
                "DoubleBuffered",
                BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
                null, gamePanel, new object[] { true });

            // Initialize the game controller (Composition)
            _gameController = new GameController();

            // Subscribe to controller events for UI updates
            _gameController.OnScoreChanged += HandleScoreChanged;
            _gameController.OnStateChanged += HandleStateChanged;
        }

        // ===== Event Handlers =====

        /// <summary>
        /// Game loop tick - updates game state and triggers repaint.
        /// </summary>
        private void gameTimer_Tick(object sender, EventArgs e)
        {
            _gameController.Update();
            gamePanel.Invalidate(); // Request repaint
        }

        /// <summary>
        /// Renders the game scene using GDI+.
        /// </summary>
        private void gamePanel_Paint(object sender, PaintEventArgs e)
        {
            _gameController.Render(e.Graphics);
        }

        /// <summary>
        /// Forwards mouse movement to the game controller for aiming.
        /// </summary>
        private void gamePanel_MouseMove(object sender, MouseEventArgs e)
        {
            _gameController.OnMouseMove(e.Location);
        }

        /// <summary>
        /// Forwards mouse clicks to the game controller to fire bubbles.
        /// </summary>
        private void gamePanel_MouseClick(object sender, MouseEventArgs e)
        {
            _gameController.OnMouseClick(e.Location);
        }

        /// <summary>
        /// Forwards keyboard input to the game controller.
        /// </summary>
        private void GameForm_KeyDown(object sender, KeyEventArgs e)
        {
            _gameController.OnKeyDown(e.KeyCode);
        }

        // ===== Button Click Handlers =====

        /// <summary>
        /// Starts the game when the Start button is clicked.
        /// </summary>
        private void btnStart_Click(object sender, EventArgs e)
        {
            _gameController.StartGame();
            gameTimer.Start();
            gamePanel.Focus(); // Ensure game panel receives keyboard input
        }

        /// <summary>
        /// Toggles pause/resume when the Pause button is clicked.
        /// </summary>
        private void btnPause_Click(object sender, EventArgs e)
        {
            if (_gameController.CurrentState == GameState.Playing)
            {
                _gameController.PauseGame();
            }
            else if (_gameController.CurrentState == GameState.Paused)
            {
                _gameController.ResumeGame();
                gamePanel.Focus();
            }
        }

        /// <summary>
        /// Restarts the game when the Restart button is clicked.
        /// </summary>
        private void btnRestart_Click(object sender, EventArgs e)
        {
            _gameController.RestartGame();
            gameTimer.Start();
            gamePanel.Focus();
        }

        // ===== UI Update Callbacks =====

        /// <summary>
        /// Updates the score display when the score changes.
        /// </summary>
        /// <param name="newScore">The new score value.</param>
        private void HandleScoreChanged(int newScore)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<int>(HandleScoreChanged), newScore);
                return;
            }
            lblScore.Text = $"Score: {newScore}";
            lblHighScore.Text = $"Best: {_gameController.HighScore}";
        }

        /// <summary>
        /// Updates the UI controls when the game state changes.
        /// </summary>
        /// <param name="newState">The new game state.</param>
        private void HandleStateChanged(GameState newState)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<GameState>(HandleStateChanged), newState);
                return;
            }

            switch (newState)
            {
                case GameState.Menu:
                    lblStatus.Text = "Ready";
                    lblStatus.ForeColor = Color.FromArgb(180, 180, 200);
                    btnStart.Enabled = true;
                    btnPause.Enabled = false;
                    btnRestart.Enabled = false;
                    break;

                case GameState.Playing:
                    lblStatus.Text = "Playing";
                    lblStatus.ForeColor = Color.FromArgb(46, 204, 113);
                    btnStart.Enabled = false;
                    btnPause.Enabled = true;
                    btnPause.Text = "⏸  Pause";
                    btnRestart.Enabled = true;
                    break;

                case GameState.Paused:
                    lblStatus.Text = "Paused";
                    lblStatus.ForeColor = Color.FromArgb(241, 196, 15);
                    btnPause.Text = "▶  Resume";
                    break;

                case GameState.GameOver:
                    lblStatus.Text = "Game Over!";
                    lblStatus.ForeColor = Color.FromArgb(231, 76, 60);
                    btnStart.Enabled = false;
                    btnPause.Enabled = false;
                    btnRestart.Enabled = true;
                    gameTimer.Stop();
                    gamePanel.Invalidate(); // Final render with game-over overlay
                    break;
            }
        }
    }
}
