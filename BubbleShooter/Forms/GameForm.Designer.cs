namespace BubbleShooter.Forms
{
    partial class GameForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        // ===== UI Controls =====
        private System.Windows.Forms.Panel gamePanel;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnPause;
        private System.Windows.Forms.Button btnRestart;
        private System.Windows.Forms.Label lblScore;
        private System.Windows.Forms.Label lblHighScore;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Timer gameTimer;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Panel controlPanel;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();

            // ===== Game Timer =====
            this.gameTimer = new System.Windows.Forms.Timer(this.components);
            this.gameTimer.Interval = 16; // ~60 FPS
            this.gameTimer.Tick += new System.EventHandler(this.gameTimer_Tick);

            // ===== Game Panel (rendering surface) =====
            this.gamePanel = new System.Windows.Forms.Panel();
            this.gamePanel.Location = new System.Drawing.Point(0, 0);
            this.gamePanel.Size = new System.Drawing.Size(600, 700);
            this.gamePanel.BackColor = System.Drawing.Color.FromArgb(15, 15, 30);
            this.gamePanel.Paint += new System.Windows.Forms.PaintEventHandler(this.gamePanel_Paint);
            this.gamePanel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.gamePanel_MouseMove);
            this.gamePanel.MouseClick += new System.Windows.Forms.MouseEventHandler(this.gamePanel_MouseClick);

            // ===== Control Panel (right sidebar) =====
            this.controlPanel = new System.Windows.Forms.Panel();
            this.controlPanel.Location = new System.Drawing.Point(600, 0);
            this.controlPanel.Size = new System.Drawing.Size(200, 700);
            this.controlPanel.BackColor = System.Drawing.Color.FromArgb(20, 20, 40);

            // ===== Title Label =====
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblTitle.Text = "BUBBLE\nSHOOTER";
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 20F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(230, 230, 250);
            this.lblTitle.Location = new System.Drawing.Point(15, 20);
            this.lblTitle.Size = new System.Drawing.Size(170, 70);
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            // ===== Score Label =====
            this.lblScore = new System.Windows.Forms.Label();
            this.lblScore.Text = "Score: 0";
            this.lblScore.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblScore.ForeColor = System.Drawing.Color.FromArgb(46, 204, 113);
            this.lblScore.Location = new System.Drawing.Point(15, 110);
            this.lblScore.Size = new System.Drawing.Size(170, 30);
            this.lblScore.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            // ===== High Score Label =====
            this.lblHighScore = new System.Windows.Forms.Label();
            this.lblHighScore.Text = "Best: 0";
            this.lblHighScore.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular);
            this.lblHighScore.ForeColor = System.Drawing.Color.FromArgb(241, 196, 15);
            this.lblHighScore.Location = new System.Drawing.Point(15, 145);
            this.lblHighScore.Size = new System.Drawing.Size(170, 25);
            this.lblHighScore.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            // ===== Status Label =====
            this.lblStatus = new System.Windows.Forms.Label();
            this.lblStatus.Text = "Ready";
            this.lblStatus.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic);
            this.lblStatus.ForeColor = System.Drawing.Color.FromArgb(180, 180, 200);
            this.lblStatus.Location = new System.Drawing.Point(15, 185);
            this.lblStatus.Size = new System.Drawing.Size(170, 25);
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            // ===== Start Button =====
            this.btnStart = new System.Windows.Forms.Button();
            this.btnStart.Text = "▶  Start Game";
            this.btnStart.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.btnStart.Location = new System.Drawing.Point(15, 240);
            this.btnStart.Size = new System.Drawing.Size(170, 45);
            this.btnStart.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStart.BackColor = System.Drawing.Color.FromArgb(46, 204, 113);
            this.btnStart.ForeColor = System.Drawing.Color.White;
            this.btnStart.FlatAppearance.BorderSize = 0;
            this.btnStart.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);

            // ===== Pause Button =====
            this.btnPause = new System.Windows.Forms.Button();
            this.btnPause.Text = "⏸  Pause";
            this.btnPause.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.btnPause.Location = new System.Drawing.Point(15, 295);
            this.btnPause.Size = new System.Drawing.Size(170, 45);
            this.btnPause.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPause.BackColor = System.Drawing.Color.FromArgb(52, 152, 219);
            this.btnPause.ForeColor = System.Drawing.Color.White;
            this.btnPause.FlatAppearance.BorderSize = 0;
            this.btnPause.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnPause.Enabled = false;
            this.btnPause.Click += new System.EventHandler(this.btnPause_Click);

            // ===== Restart Button =====
            this.btnRestart = new System.Windows.Forms.Button();
            this.btnRestart.Text = "🔄  Restart";
            this.btnRestart.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.btnRestart.Location = new System.Drawing.Point(15, 350);
            this.btnRestart.Size = new System.Drawing.Size(170, 45);
            this.btnRestart.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRestart.BackColor = System.Drawing.Color.FromArgb(231, 76, 60);
            this.btnRestart.ForeColor = System.Drawing.Color.White;
            this.btnRestart.FlatAppearance.BorderSize = 0;
            this.btnRestart.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRestart.Enabled = false;
            this.btnRestart.Click += new System.EventHandler(this.btnRestart_Click);

            // ===== Controls Label =====
            System.Windows.Forms.Label lblControls = new System.Windows.Forms.Label();
            lblControls.Text = "Controls:";
            lblControls.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            lblControls.ForeColor = System.Drawing.Color.FromArgb(200, 200, 220);
            lblControls.Location = new System.Drawing.Point(15, 430);
            lblControls.Size = new System.Drawing.Size(170, 25);

            System.Windows.Forms.Label lblControlsInfo = new System.Windows.Forms.Label();
            lblControlsInfo.Text = "Mouse: Aim\nClick/Space: Fire\nArrows: Fine-tune\nP: Pause";
            lblControlsInfo.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular);
            lblControlsInfo.ForeColor = System.Drawing.Color.FromArgb(140, 140, 160);
            lblControlsInfo.Location = new System.Drawing.Point(15, 458);
            lblControlsInfo.Size = new System.Drawing.Size(170, 80);

            // ===== Add controls to controlPanel =====
            this.controlPanel.Controls.Add(this.lblTitle);
            this.controlPanel.Controls.Add(this.lblScore);
            this.controlPanel.Controls.Add(this.lblHighScore);
            this.controlPanel.Controls.Add(this.lblStatus);
            this.controlPanel.Controls.Add(this.btnStart);
            this.controlPanel.Controls.Add(this.btnPause);
            this.controlPanel.Controls.Add(this.btnRestart);
            this.controlPanel.Controls.Add(lblControls);
            this.controlPanel.Controls.Add(lblControlsInfo);

            // ===== Form Configuration =====
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 700);
            this.Controls.Add(this.gamePanel);
            this.Controls.Add(this.controlPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "GameForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Bubble Shooter - OOP Project";
            this.BackColor = System.Drawing.Color.FromArgb(15, 15, 30);
            this.KeyPreview = true;
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.GameForm_KeyDown);
        }

        #endregion
    }
}
