using ChessEngine;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;

namespace StockfishV0
{
    public class ChessForm : Form
    {
        private Panel mainMenuPanel;
        private Panel playPanel;
        private Panel colorSelectPanel;
        private Panel gamePanel;
        private Panel settingsPanel;

        private ChessBoardControl chessBoard;
        private ChessBoardControl settingsBoard;
        private Label gameModeLabel;
        private TextBox fenTextBox;
        private Label fenStatusLabel;
        private Button solvePuzzlesButton;
        private Label puzzleProgressLabel;

        private Panel whitePiecePanel;
        private Panel blackPiecePanel;
        private int selectedPalettePieceType = -1;

        private Panel puzzlePanel;
        private ChessBoardControl puzzleBoard;
        private Label puzzleTitleLabel;
        private Label puzzleProgressLabel2;
        private FlowLayoutPanel puzzleResultsPanel2;
        private bool isDoingPuzzles = false;

        private System.Windows.Forms.Timer fenDebounceTimer;
        private bool fenLoadingFromBoard = false;
        private bool settingsAiVsAiRunning = false;

        private enum GameMode
        {
            Pvp,
            Pvai,
            Aivai
        }

        private GameMode currentGameMode = GameMode.Pvp;

        public ChessForm()
        {
            Text = "StockfishV0";
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.FromArgb(32, 32, 32);
            ClientSize = new Size(1000, 900);

            BuildGameScreen();
            BuildPlayScreen();
            BuildColorSelectScreen();
            BuildSettingsScreen();
            BuildMainMenuScreen();

            Controls.Add(gamePanel);
            Controls.Add(colorSelectPanel);
            Controls.Add(playPanel);
            Controls.Add(settingsPanel);
            Controls.Add(mainMenuPanel);

            ShowMainMenuScreen();
        }

        private void BuildMainMenuScreen()
        {
            mainMenuPanel = new Panel();
            mainMenuPanel.Dock = DockStyle.Fill;
            mainMenuPanel.BackColor = Color.FromArgb(32, 32, 32);

            TableLayoutPanel layout = CreateMenuLayout();

            Label title = CreateTitleLabel("StockfishV0", 42);

            Button playButton = CreateMenuButton("PLAY");
            playButton.Click += PlayButton_Click;

            Button puzzlesButton = CreateMenuButton("DO PUZZLES");
            puzzlesButton.Click += DoPuzzlesButton_Click;

            Button settingsButton = CreateMenuButton("CUSTOM");
            settingsButton.Click += SettingsButton_Click;

            layout.Controls.Add(title, 0, 1);
            layout.Controls.Add(playButton, 0, 2);
            layout.Controls.Add(puzzlesButton, 0, 3);
            layout.Controls.Add(settingsButton, 0, 4);

            mainMenuPanel.Controls.Add(layout);
        }

        private void BuildPlayScreen()
        {
            playPanel = new Panel();
            playPanel.Dock = DockStyle.Fill;
            playPanel.BackColor = Color.FromArgb(32, 32, 32);

            TableLayoutPanel layout = CreateMenuLayout();

            Label title = CreateTitleLabel("Choose Game Mode", 36);

            Button pvpButton = CreateMenuButton("PVP");
            pvpButton.Click += PvpButton_Click;

            Button pvaiButton = CreateMenuButton("PVAI");
            pvaiButton.Click += PvaiButton_Click;

            Button aivaiButton = CreateMenuButton("AIVAI");
            aivaiButton.Click += AivaiButton_Click;

            Button backButton = CreateMenuButton("BACK");
            backButton.Click += BackButtonToMain_Click;

            layout.Controls.Add(title, 0, 1);
            layout.Controls.Add(pvpButton, 0, 2);
            layout.Controls.Add(pvaiButton, 0, 3);
            layout.Controls.Add(aivaiButton, 0, 4);
            layout.Controls.Add(backButton, 0, 5);

            playPanel.Controls.Add(layout);
        }

        private void BuildColorSelectScreen()
        {
            colorSelectPanel = new Panel();
            colorSelectPanel.Dock = DockStyle.Fill;
            colorSelectPanel.BackColor = Color.FromArgb(32, 32, 32);

            TableLayoutPanel layout = CreateMenuLayout();

            Label title = CreateTitleLabel("Choose Your Color", 36);

            Button whiteButton = CreateMenuButton("WHITE");
            whiteButton.Click += PlayWhiteButton_Click;

            Button blackButton = CreateMenuButton("BLACK");
            blackButton.Click += PlayBlackButton_Click;

            Button backButton = CreateMenuButton("BACK");
            backButton.Click += BackButtonToPlay_Click;

            layout.Controls.Add(title, 0, 1);
            layout.Controls.Add(whiteButton, 0, 2);
            layout.Controls.Add(blackButton, 0, 3);
            layout.Controls.Add(backButton, 0, 4);

            colorSelectPanel.Controls.Add(layout);
        }

        private void BuildGameScreen()
        {
            gamePanel = new Panel();
            gamePanel.Dock = DockStyle.Fill;
            gamePanel.BackColor = Color.FromArgb(32, 32, 32);

            chessBoard = new ChessBoardControl();
            chessBoard.Dock = DockStyle.Fill;
            chessBoard.BackColor = Color.FromArgb(32, 32, 32);

            Button menuButton = new Button();
            menuButton.Text = "Menu";
            menuButton.Width = 80;
            menuButton.Height = 32;
            menuButton.Location = new Point(10, 10);
            menuButton.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            menuButton.ForeColor = Color.White;
            menuButton.Click += MenuButton_Click;

            gameModeLabel = new Label();
            gameModeLabel.Text = "PVP";
            gameModeLabel.AutoSize = false;
            gameModeLabel.Width = 160;
            gameModeLabel.Height = 32;
            gameModeLabel.Location = new Point(100, 10);
            gameModeLabel.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            gameModeLabel.TextAlign = ContentAlignment.MiddleCenter;
            gameModeLabel.Font = new Font("Arial", 10, FontStyle.Bold);
            gameModeLabel.ForeColor = Color.White;
            gameModeLabel.BackColor = Color.FromArgb(55, 55, 55);

            gamePanel.Controls.Add(chessBoard);
            gamePanel.Controls.Add(menuButton);
            gamePanel.Controls.Add(gameModeLabel);

            menuButton.BringToFront();
            gameModeLabel.BringToFront();
        }

        private void BuildSettingsScreen()
        {
            settingsPanel = new Panel();
            settingsPanel.Dock = DockStyle.Fill;
            settingsPanel.BackColor = Color.FromArgb(32, 32, 32);

            Panel topBar = new Panel();
            topBar.Height = 60;
            topBar.Dock = DockStyle.Top;
            topBar.BackColor = Color.FromArgb(42, 42, 42);
            topBar.Padding = new Padding(8, 10, 8, 10);

            TableLayoutPanel topLayout = new TableLayoutPanel();
            topLayout.Dock = DockStyle.Fill;
            topLayout.ColumnCount = 4;
            topLayout.RowCount = 1;
            topLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 70));
            topLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            topLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 70));
            topLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110));

            Button backButton = new Button();
            backButton.Text = "Back";
            backButton.Width = 60;
            backButton.Height = 36;
            backButton.Font = new Font("Arial", 10, FontStyle.Bold);
            backButton.ForeColor = Color.White;
            backButton.BackColor = Color.FromArgb(95, 65, 55);
            backButton.FlatStyle = FlatStyle.Flat;
            backButton.FlatAppearance.BorderSize = 0;
            backButton.Cursor = Cursors.Hand;
            backButton.Click += BackButtonToMain_Click;
            backButton.Anchor = AnchorStyles.Left;
            topLayout.Controls.Add(backButton, 0, 0);

            fenTextBox = new TextBox();
            fenTextBox.Height = 36;
            fenTextBox.Font = new Font("Consolas", 10, FontStyle.Regular);
            fenTextBox.Text = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
            fenTextBox.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            fenTextBox.Dock = DockStyle.Fill;
            fenTextBox.TextChanged += FenTextBox_TextChanged;
            fenTextBox.KeyDown += FenTextBox_KeyDown;
            topLayout.Controls.Add(fenTextBox, 1, 0);

            Button loadFenButton = new Button();
            loadFenButton.Text = "Load";
            loadFenButton.Width = 60;
            loadFenButton.Height = 36;
            loadFenButton.Font = new Font("Arial", 10, FontStyle.Bold);
            loadFenButton.ForeColor = Color.White;
            loadFenButton.BackColor = Color.FromArgb(75, 105, 55);
            loadFenButton.FlatStyle = FlatStyle.Flat;
            loadFenButton.FlatAppearance.BorderSize = 0;
            loadFenButton.Cursor = Cursors.Hand;
            loadFenButton.Click += LoadFenButton_Click;
            loadFenButton.Anchor = AnchorStyles.Right;
            topLayout.Controls.Add(loadFenButton, 2, 0);

            fenStatusLabel = new Label();
            fenStatusLabel.Text = "";
            fenStatusLabel.AutoSize = true;
            fenStatusLabel.ForeColor = Color.FromArgb(210, 210, 210);
            fenStatusLabel.Font = new Font("Arial", 9, FontStyle.Bold);
            fenStatusLabel.Anchor = AnchorStyles.Left;
            fenStatusLabel.TextAlign = ContentAlignment.MiddleLeft;
            fenStatusLabel.Padding = new Padding(8, 0, 0, 0);
            topLayout.Controls.Add(fenStatusLabel, 3, 0);

            topBar.Controls.Add(topLayout);

            fenDebounceTimer = new System.Windows.Forms.Timer();
            fenDebounceTimer.Interval = 500;
            fenDebounceTimer.Tick += FenDebounceTimer_Tick;

            Panel middleArea = new Panel();
            middleArea.Dock = DockStyle.Fill;
            middleArea.BackColor = Color.FromArgb(32, 32, 32);

            BuildPiecePalettes();

            TableLayoutPanel boardLayout = new TableLayoutPanel();
            boardLayout.Dock = DockStyle.Fill;
            boardLayout.ColumnCount = 3;
            boardLayout.RowCount = 1;
            boardLayout.BackColor = Color.FromArgb(32, 32, 32);
            boardLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 64));
            boardLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            boardLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 64));

            settingsBoard = new ChessBoardControl();
            settingsBoard.EditorMode = true;
            settingsBoard.BackColor = Color.FromArgb(32, 32, 32);
            settingsBoard.OnBoardChanged += SettingsBoard_OnBoardChanged;
            settingsBoard.Dock = DockStyle.Fill;

            boardLayout.Controls.Add(whitePiecePanel, 0, 0);
            boardLayout.Controls.Add(settingsBoard, 1, 0);
            boardLayout.Controls.Add(blackPiecePanel, 2, 0);

            middleArea.Controls.Add(boardLayout);

            Panel bottomBar = new Panel();
            bottomBar.Height = 50;
            bottomBar.Dock = DockStyle.Bottom;
            bottomBar.BackColor = Color.FromArgb(42, 42, 42);

            solvePuzzlesButton = new Button();
            solvePuzzlesButton.Text = "Start AI vs AI";
            solvePuzzlesButton.Width = 160;
            solvePuzzlesButton.Height = 34;
            solvePuzzlesButton.Location = new Point(12, 8);
            solvePuzzlesButton.Font = new Font("Arial", 10, FontStyle.Bold);
            solvePuzzlesButton.ForeColor = Color.White;
            solvePuzzlesButton.BackColor = Color.FromArgb(75, 105, 55);
            solvePuzzlesButton.FlatStyle = FlatStyle.Flat;
            solvePuzzlesButton.FlatAppearance.BorderSize = 0;
            solvePuzzlesButton.Cursor = Cursors.Hand;
            solvePuzzlesButton.Click += AiVsAiButton_Click;

            puzzleProgressLabel = new Label();
            puzzleProgressLabel.Text = "";
            puzzleProgressLabel.AutoSize = true;
            puzzleProgressLabel.Location = new Point(182, 14);
            puzzleProgressLabel.ForeColor = Color.FromArgb(210, 210, 210);
            puzzleProgressLabel.Font = new Font("Arial", 10, FontStyle.Bold);

            bottomBar.Controls.Add(solvePuzzlesButton);
            bottomBar.Controls.Add(puzzleProgressLabel);

            settingsPanel.Controls.Add(topBar);
            settingsPanel.Controls.Add(bottomBar);
            settingsPanel.Controls.Add(middleArea);
        }

        private void BuildPiecePalettes()
        {
            whitePiecePanel = new Panel();
            whitePiecePanel.Width = 64;
            whitePiecePanel.Dock = DockStyle.Fill;
            whitePiecePanel.BackColor = Color.FromArgb(32, 32, 32);
            whitePiecePanel.Padding = new Padding(4, 4, 4, 4);

            blackPiecePanel = new Panel();
            blackPiecePanel.Width = 64;
            blackPiecePanel.Dock = DockStyle.Fill;
            blackPiecePanel.BackColor = Color.FromArgb(32, 32, 32);
            blackPiecePanel.Padding = new Padding(4, 4, 4, 4);

            string[] whiteCodes = { "wP", "wN", "wB", "wR", "wQ", "wK" };
            int[] whiteTypes = { 0, 1, 2, 3, 4, 5 };
            string[] blackCodes = { "bP", "bN", "bB", "bR", "bQ", "bK" };
            int[] blackTypes = { 6, 7, 8, 9, 10, 11 };

            for (int i = 0; i < 6; i++)
            {
                Button whiteBtn = CreatePaletteButton(whiteCodes[i], whiteTypes[i], 0);
                whiteBtn.Dock = DockStyle.Top;
                whitePiecePanel.Controls.Add(whiteBtn);
            }
            for (int i = 0; i < 6; i++)
            {
                Button blackBtn = CreatePaletteButton(blackCodes[i], blackTypes[i], 6);
                blackBtn.Dock = DockStyle.Top;
                blackPiecePanel.Controls.Add(blackBtn);
            }
        }

        private Button CreatePaletteButton(string pieceCode, int pieceType, int startIndex)
        {
            Button btn = new Button();
            btn.Width = 52;
            btn.Height = 52;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 2;
            btn.FlatAppearance.BorderColor = Color.FromArgb(60, 60, 60);
            btn.BackColor = Color.FromArgb(50, 50, 50);
            btn.Cursor = Cursors.Hand;
            btn.Tag = pieceType;
            btn.Text = "";
            btn.Margin = new Padding(4, 2, 4, 2);

            if (chessBoard != null)
            {
                Image img = chessBoard.GetPieceImage(pieceCode);
                if (img != null)
                {
                    btn.Image = new Bitmap(img, 36, 36);
                    btn.ImageAlign = ContentAlignment.MiddleCenter;
                }
                else
                {
                    btn.Text = pieceCode;
                    btn.Font = new Font("Arial", 8, FontStyle.Bold);
                    btn.ForeColor = Color.White;
                }
            }
            else
            {
                btn.Text = pieceCode;
                btn.Font = new Font("Arial", 8, FontStyle.Bold);
                btn.ForeColor = Color.White;
            }

            btn.Click += PalettePiece_Click;
            return btn;
        }

        private void FenTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                LoadFenButton_Click(sender, e);
                e.SuppressKeyPress = true;
            }
        }

        private TableLayoutPanel CreateMenuLayout()
        {
            TableLayoutPanel layout = new TableLayoutPanel();

            layout.Dock = DockStyle.Fill;
            layout.ColumnCount = 1;
            layout.RowCount = 7;

            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 22F));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 13F));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 11F));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 11F));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 11F));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 11F));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 21F));

            return layout;
        }

        private Label CreateTitleLabel(string text, int fontSize)
        {
            Label title = new Label();

            title.Text = text;
            title.ForeColor = Color.White;
            title.Font = new Font("Arial", fontSize, FontStyle.Bold);
            title.TextAlign = ContentAlignment.MiddleCenter;
            title.Dock = DockStyle.Fill;

            return title;
        }

        private Button CreateMenuButton(string text)
        {
            Button button = new Button();

            button.Text = text;
            button.Width = 240;
            button.Height = 60;
            button.Anchor = AnchorStyles.None;

            button.Font = new Font("Arial", 18, FontStyle.Bold);
            button.ForeColor = Color.White;
            button.BackColor = Color.FromArgb(75, 105, 55);

            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.Cursor = Cursors.Hand;

            return button;
        }

        private void PlayButton_Click(object sender, EventArgs e)
        {
            ShowPlayScreen();
        }

        private void SettingsButton_Click(object sender, EventArgs e)
        {
            ShowSettingsScreen();
        }

        private void LoadFenButton_Click(object sender, EventArgs e)
        {
            if (fenTextBox == null || settingsBoard == null) return;

            string error;
            if (!settingsBoard.LoadFenPosition(fenTextBox.Text, out error))
            {
                if (fenStatusLabel != null)
                {
                    fenStatusLabel.ForeColor = Color.FromArgb(235, 110, 95);
                    fenStatusLabel.Text = "FEN error: " + error;
                }
                return;
            }

            if (fenStatusLabel != null)
            {
                fenStatusLabel.ForeColor = Color.FromArgb(120, 220, 120);
                fenStatusLabel.Text = "Loaded.";
            }
        }

        private void SettingsBoard_OnBoardChanged()
        {
            if (fenTextBox != null && settingsBoard != null)
            {
                fenLoadingFromBoard = true;
                fenTextBox.Text = settingsBoard.GetFen();
                fenLoadingFromBoard = false;
                fenStatusLabel.Text = "";
            }
        }

        private void FenTextBox_TextChanged(object sender, EventArgs e)
        {
            if (fenLoadingFromBoard) return;
            fenDebounceTimer?.Stop();
            fenDebounceTimer?.Start();
        }

        private void FenDebounceTimer_Tick(object sender, EventArgs e)
        {
            fenDebounceTimer.Stop();
            if (fenTextBox == null || settingsBoard == null) return;
            string error;
            if (!settingsBoard.LoadFenPosition(fenTextBox.Text, out error))
            {
                if (fenStatusLabel != null)
                {
                    fenStatusLabel.ForeColor = Color.FromArgb(235, 110, 95);
                    fenStatusLabel.Text = "FEN error: " + error;
                }
            }
            else
            {
                if (fenStatusLabel != null)
                {
                    fenStatusLabel.ForeColor = Color.FromArgb(120, 220, 120);
                    fenStatusLabel.Text = "Loaded.";
                }
            }
        }

        private void PalettePiece_Click(object sender, EventArgs e)
        {
            if (settingsBoard == null) return;
            Button btn = sender as Button;
            if (btn == null) return;

            int pieceType = (int)btn.Tag;

            if (selectedPalettePieceType == pieceType)
            {
                selectedPalettePieceType = -1;
                settingsBoard.PalettePieceType = -1;
                UpdatePaletteButtonBorders();
            }
            else
            {
                selectedPalettePieceType = pieceType;
                settingsBoard.PalettePieceType = pieceType;
                UpdatePaletteButtonBorders();
            }
        }

        private void UpdatePaletteButtonBorders()
        {
            foreach (Control c in whitePiecePanel.Controls)
            {
                if (c is Button btn && btn.Tag is int pt)
                {
                    btn.FlatAppearance.BorderColor = pt == selectedPalettePieceType
                        ? Color.FromArgb(245, 178, 38) : Color.FromArgb(60, 60, 60);
                }
            }
            foreach (Control c in blackPiecePanel.Controls)
            {
                if (c is Button btn && btn.Tag is int pt)
                {
                    btn.FlatAppearance.BorderColor = pt == selectedPalettePieceType
                        ? Color.FromArgb(245, 178, 38) : Color.FromArgb(60, 60, 60);
                }
            }
        }

        public int GetSelectedPalettePieceType()
        {
            return selectedPalettePieceType;
        }

        private void DoPuzzlesButton_Click(object sender, EventArgs e)
        {
            BuildPuzzleScreenIfNeeded();
            ShowPuzzleScreen();
            BeginPuzzleSolver();
        }

        private void BuildPuzzleScreenIfNeeded()
        {
            if (puzzlePanel != null) return;

            puzzlePanel = new Panel();
            puzzlePanel.Dock = DockStyle.Fill;
            puzzlePanel.BackColor = Color.FromArgb(32, 32, 32);

            Panel puzzleTopBar = new Panel();
            puzzleTopBar.Height = 50;
            puzzleTopBar.Dock = DockStyle.Top;
            puzzleTopBar.BackColor = Color.FromArgb(42, 42, 42);

            Button puzzleBackButton = new Button();
            puzzleBackButton.Text = "Back";
            puzzleBackButton.Width = 60;
            puzzleBackButton.Height = 34;
            puzzleBackButton.Location = new Point(12, 8);
            puzzleBackButton.Font = new Font("Arial", 10, FontStyle.Bold);
            puzzleBackButton.ForeColor = Color.White;
            puzzleBackButton.BackColor = Color.FromArgb(95, 65, 55);
            puzzleBackButton.FlatStyle = FlatStyle.Flat;
            puzzleBackButton.FlatAppearance.BorderSize = 0;
            puzzleBackButton.Cursor = Cursors.Hand;
            puzzleBackButton.Click += PuzzleBackButton_Click;
            puzzleTopBar.Controls.Add(puzzleBackButton);

            puzzleTitleLabel = new Label();
            puzzleTitleLabel.Text = "Puzzles";
            puzzleTitleLabel.AutoSize = true;
            puzzleTitleLabel.Location = new Point(90, 12);
            puzzleTitleLabel.ForeColor = Color.White;
            puzzleTitleLabel.Font = new Font("Arial", 14, FontStyle.Bold);
            puzzleTopBar.Controls.Add(puzzleTitleLabel);

            puzzleProgressLabel2 = new Label();
            puzzleProgressLabel2.Text = "";
            puzzleProgressLabel2.AutoSize = true;
            puzzleProgressLabel2.Location = new Point(250, 14);
            puzzleProgressLabel2.ForeColor = Color.FromArgb(210, 210, 210);
            puzzleProgressLabel2.Font = new Font("Arial", 10, FontStyle.Bold);
            puzzleTopBar.Controls.Add(puzzleProgressLabel2);

            puzzlePanel.Controls.Add(puzzleTopBar);

            puzzleBoard = new ChessBoardControl();
            puzzleBoard.Dock = DockStyle.Fill;
            puzzleBoard.BackColor = Color.FromArgb(32, 32, 32);
            puzzleBoard.Enabled = false;
            puzzleBoard.ShowEngineBar = false;

            Panel puzzleBottomBar = new Panel();
            puzzleBottomBar.Height = 46;
            puzzleBottomBar.Dock = DockStyle.Bottom;
            puzzleBottomBar.BackColor = Color.FromArgb(42, 42, 42);

            puzzleResultsPanel2 = new FlowLayoutPanel();
            puzzleResultsPanel2.Location = new Point(12, 8);
            puzzleResultsPanel2.Width = 900;
            puzzleResultsPanel2.Height = 30;
            puzzleResultsPanel2.BackColor = Color.Transparent;
            puzzleResultsPanel2.FlowDirection = FlowDirection.LeftToRight;
            puzzleResultsPanel2.WrapContents = false;
            puzzleBottomBar.Controls.Add(puzzleResultsPanel2);

            puzzlePanel.Controls.Add(puzzleBottomBar);
            puzzlePanel.Controls.Add(puzzleBoard);

            Controls.Add(puzzlePanel);
        }

        private void ShowPuzzleScreen()
        {
            mainMenuPanel.Visible = false;
            playPanel.Visible = false;
            colorSelectPanel.Visible = false;
            gamePanel.Visible = false;
            settingsPanel.Visible = false;
            puzzlePanel.Visible = true;
            puzzlePanel.BringToFront();
        }

        private void PuzzleBackButton_Click(object sender, EventArgs e)
        {
            isDoingPuzzles = false;
            puzzleBoard?.StopAiLoop();
            ShowMainMenuScreen();
        }

        private async void BeginPuzzleSolver()
        {
            if (isDoingPuzzles) return;
            isDoingPuzzles = true;

            string csvPath = FindPuzzlesCsv();
            if (csvPath == null)
            {
                puzzleProgressLabel2.Text = "Puzzles.csv not found!";
                isDoingPuzzles = false;
                return;
            }

            puzzleResultsPanel2.Controls.Clear();
            puzzleProgressLabel2.Text = "Loading puzzles...";

            string[] lines;
            try { lines = File.ReadAllLines(csvPath); }
            catch { puzzleProgressLabel2.Text = "Failed to read puzzles."; isDoingPuzzles = false; return; }

            if (lines.Length < 2) { puzzleProgressLabel2.Text = "No puzzles found."; isDoingPuzzles = false; return; }

            int maxPuzzles = Math.Min(100, lines.Length - 1);

            List<int> indices = new List<int>();
            for (int idx = 1; idx < lines.Length; idx++)
                indices.Add(idx);

            Random rng = new Random();
            for (int k = indices.Count - 1; k > 0; k--)
            {
                int j = rng.Next(k + 1);
                int tmp = indices[k];
                indices[k] = indices[j];
                indices[j] = tmp;
            }

            string newCsvDir = Path.GetDirectoryName(csvPath);
            string newCsvPath = Path.Combine(newCsvDir, "newPuzzles.csv");
            List<string> successfulLines = new List<string>();
            successfulLines.Add(lines[0]);

            int solvedCount = 0;
            int attemptedCount = 0;

            for (int pi = 0; pi < maxPuzzles && isDoingPuzzles; pi++)
            {
                int i = indices[pi];
                string line = lines[i];
                if (string.IsNullOrWhiteSpace(line)) continue;

                string[] cols = line.Split(',');
                if (cols.Length < 3) continue;

                string fen = cols[1];
                string solutionPgn = cols.Length > 2 ? cols[2] : "";

                attemptedCount++;

                Invoke((MethodInvoker)(() =>
                {
                    puzzleProgressLabel2.Text = $"Puzzle {attemptedCount}/{maxPuzzles} | {solvedCount} solved";
                }));

                bool solved = await SolvePuzzleOnBoard(fen, solutionPgn);

                Invoke((MethodInvoker)(() =>
                {
                    Label resultLabel = new Label();
                    resultLabel.Margin = new Padding(0, 2, 6, 0);
                    resultLabel.Text = solved ? "✔" : "✘";
                    resultLabel.ForeColor = solved ? Color.FromArgb(120, 220, 120) : Color.FromArgb(235, 110, 95);
                    resultLabel.Font = new Font("Arial", 12, FontStyle.Bold);
                    resultLabel.AutoSize = true;
                    puzzleResultsPanel2.Controls.Add(resultLabel);
                }));

                if (solved)
                {
                    solvedCount++;
                    successfulLines.Add(line);
                }

                await Task.Delay(500);
            }

            if (successfulLines.Count > 1)
            {
                File.WriteAllLines(newCsvPath, successfulLines);
            }

            puzzleProgressLabel2.Text = $"Done. {solvedCount}/{attemptedCount} written to newPuzzles.csv";
            isDoingPuzzles = false;
        }

        private async Task<bool> SolvePuzzleOnBoard(string fen, string solutionPgn)
        {
            Board puzzleBoardLocal = new Board();
            if (!EngineHelpers.TryLoadFen(puzzleBoardLocal, fen, out _))
                return false;

            Invoke((MethodInvoker)(() =>
            {
                puzzleBoard?.LoadFenPosition(fen, out _);
                puzzleBoard!.BoardPerspective = 0;
            }));

            int puzzleSide = puzzleBoardLocal.SideToMove;

            List<string> solutionMoves = null;
            if (!string.IsNullOrWhiteSpace(solutionPgn))
            {
                solutionMoves = ParseSolutionPgn(solutionPgn);
            }

            int maxMoves = 20;
            int moveCount = 0;

            List<ulong> posHistory = new List<ulong>();
            posHistory.Add(puzzleBoardLocal.ZobristKey);

            while (moveCount < maxMoves && isDoingPuzzles)
            {
                int state = puzzleBoardLocal.GetBoardState();
                if (state == 0 || state == 1) return true;
                if (state == puzzleSide) return true;
                if (state == 1 - puzzleSide || state == 2) return false;

                if (CheckThreefoldRepetitionLocal(posHistory, puzzleBoardLocal.ZobristKey)) return false;

                Move[] moveBuffer = new Move[500];
                int legalCount = allMoves.GenerateAllLegalMoves(puzzleBoardLocal, moveBuffer.AsSpan(), puzzleBoardLocal.SideToMove);
                if (legalCount == 0)
                {
                    int endState = puzzleBoardLocal.GetBoardState();
                    return endState == puzzleSide;
                }

                Move nextMove;
                if (solutionMoves != null && moveCount < solutionMoves.Count)
                {
                    nextMove = ConvertSanToMove(solutionMoves[moveCount], puzzleBoardLocal, moveBuffer, legalCount);
                    if (nextMove.FromSquare == -1)
                        return false;
                }
                else
                {
                    Board sandbox = puzzleBoardLocal.Clone();
                    Move bestMove = await Task.Run(() => Bot.Think(sandbox, 8, 0));
                    bool moveValid = false;
                    nextMove = bestMove;
                    for (int j = 0; j < legalCount; j++)
                    {
                        if (moveBuffer[j].FromSquare == bestMove.FromSquare &&
                            moveBuffer[j].ToSquare == bestMove.ToSquare)
                        {
                            nextMove = moveBuffer[j];
                            moveValid = true;
                            break;
                        }
                    }
                    if (!moveValid) return false;
                }

                puzzleBoardLocal.MakeMove(nextMove);
                posHistory.Add(puzzleBoardLocal.ZobristKey);
                moveCount++;

                Invoke((MethodInvoker)(() =>
                {
                    if (puzzleBoard != null && puzzlePanel.Visible)
                    {
                        puzzleBoard.LoadFenPosition(GetBoardFen(puzzleBoardLocal), out _);
                        puzzleBoard.BoardPerspective = 0;
                    }
                }));

                await Task.Delay(1000);
            }

            return puzzleBoardLocal.GetBoardState() == puzzleSide;
        }

        private List<string> ParseSolutionPgn(string pgn)
        {
            List<string> moves = new List<string>();
            string[] tokens = pgn.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string token in tokens)
            {
                if (token == "*") continue;
                if (char.IsDigit(token[0]) && (token.EndsWith(".") || token.EndsWith("...")))
                    continue;

                if (char.IsDigit(token[0]))
                {
                    int dotIdx = token.IndexOf('.');
                    if (dotIdx >= 0)
                    {
                        string after = token.Substring(dotIdx + 1);
                        if (!string.IsNullOrEmpty(after))
                            moves.Add(after);
                    }
                    continue;
                }

                moves.Add(token);
            }

            return moves;
        }

        private Move ConvertSanToMove(string san, Board board, Move[] legalMoves, int legalCount)
        {
            string token = san;

            if (token.EndsWith("+") || token.EndsWith("#"))
                token = token.TrimEnd('+', '#');

            int promotedPieceType = -1;
            int eqIdx = token.IndexOf('=');
            if (eqIdx >= 0)
            {
                char promoChar = token[eqIdx + 1];
                promotedPieceType = SanCharToPromoType(promoChar);
                token = token.Substring(0, eqIdx);
            }

            bool isCapture = token.Contains("x");

            int pieceType;
            string destStr;
            string disambiguation = "";

            if (char.IsUpper(token[0]) && token[0] != 'O')
            {
                char pieceChar = token[0];
                token = token.Substring(1);
                token = token.Replace("x", "");

                if (token.Length > 2)
                {
                    disambiguation = token.Substring(0, token.Length - 2);
                }
                destStr = token.Substring(token.Length - 2);
                pieceType = SanPieceCharToType(pieceChar, board.SideToMove);
            }
            else
            {
                token = token.Replace("x", "");
                pieceType = board.SideToMove == 0 ? 0 : 6;

                if (isCapture && token.Length > 2)
                {
                    disambiguation = token.Substring(0, token.Length - 2);
                }
                destStr = token.Substring(token.Length - 2);
            }

            int destSquare;
            if (!EngineHelpers.NotationToIndex.TryGetValue(destStr, out destSquare))
                return new Move(-1, -1, -1);

            Move bestMatch = new Move(-1, -1, -1);

            for (int i = 0; i < legalCount; i++)
            {
                Move m = legalMoves[i];
                if (m.ToSquare != destSquare) continue;
                if (m.PieceType != pieceType) continue;
                if (m.IsPromotion != (promotedPieceType >= 0)) continue;
                if (promotedPieceType >= 0 && m.PromotedPieceType != promotedPieceType) continue;
                if (m.IsCapture != isCapture) continue;

                if (disambiguation.Length > 0)
                {
                    string fromStr = EngineHelpers.IndexToNotation[m.FromSquare];
                    bool disambigOk = true;
                    foreach (char d in disambiguation)
                    {
                        bool found = false;
                        foreach (char f in fromStr)
                        {
                            if (char.ToLower(d) == char.ToLower(f)) { found = true; break; }
                        }
                        if (!found) { disambigOk = false; break; }
                    }
                    if (!disambigOk) continue;
                }

                bestMatch = m;
                break;
            }

            return bestMatch;
        }

        private int SanPieceCharToType(char c, int sideToMove)
        {
            int offset = sideToMove * 6;
            switch (char.ToUpper(c))
            {
                case 'P': return 0 + offset;
                case 'N': return 1 + offset;
                case 'B': return 2 + offset;
                case 'R': return 3 + offset;
                case 'Q': return 4 + offset;
                case 'K': return 5 + offset;
            }
            return -1;
        }

        private int SanCharToPromoType(char c)
        {
            switch (char.ToUpper(c))
            {
                case 'Q': return 4;
                case 'R': return 3;
                case 'B': return 2;
                case 'N': return 1;
            }
            return -1;
        }

        private bool CheckThreefoldRepetitionLocal(List<ulong> history, ulong currentKey)
        {
            int count = 0;
            for (int i = history.Count - 1; i >= 0; i--)
            {
                if (history[i] == currentKey) count++;
                if (count >= 3) return true;
            }
            return false;
        }

        private void AiVsAiButton_Click(object sender, EventArgs e)
        {
            if (settingsAiVsAiRunning)
            {
                settingsAiVsAiRunning = false;
                settingsBoard?.StopAiLoop();
                solvePuzzlesButton.Text = "Start AI vs AI";
                solvePuzzlesButton.BackColor = Color.FromArgb(75, 105, 55);
                puzzleProgressLabel.Text = "";
                return;
            }

            if (settingsBoard == null) return;

            settingsAiVsAiRunning = true;
            solvePuzzlesButton.Text = "Stop AI vs AI";
            solvePuzzlesButton.BackColor = Color.FromArgb(185, 75, 55);
            puzzleProgressLabel.Text = "AI vs AI running...";

            string currentFen = settingsBoard.GetFen();
            settingsBoard.EditorMode = false;
            settingsBoard.LoadFenPosition(currentFen, out _);
            settingsBoard.StartAiVsAi();

            System.Windows.Forms.Timer monitorTimer = new System.Windows.Forms.Timer();
            monitorTimer.Interval = 300;
            monitorTimer.Tick += (s, ev) =>
            {
                if (!settingsAiVsAiRunning || settingsBoard == null)
                {
                    monitorTimer.Stop();
                    monitorTimer.Dispose();
                    return;
                }

                if (settingsBoard.IsGameOver)
                {
                    settingsAiVsAiRunning = false;
                    settingsBoard.StopAiLoop();
                    Invoke((MethodInvoker)(() =>
                    {
                        solvePuzzlesButton.Text = "Start AI vs AI";
                        solvePuzzlesButton.BackColor = Color.FromArgb(75, 105, 55);
                        string fen = settingsBoard.GetFen();
                        Board checkB = new Board();
                        if (EngineHelpers.TryLoadFen(checkB, fen, out _))
                        {
                            int st = checkB.GetBoardState();
                            if (st == 0) puzzleProgressLabel.Text = "White won!";
                            else if (st == 1) puzzleProgressLabel.Text = "Black won!";
                            else puzzleProgressLabel.Text = "Draw!";
                        }
                        settingsBoard.EditorMode = true;
                    }));
                    monitorTimer.Stop();
                    monitorTimer.Dispose();
                }
            };
            monitorTimer.Start();
        }

        private string GetBoardFen(Board board)
        {
            System.Text.StringBuilder fen = new System.Text.StringBuilder();
            for (int rank = 7; rank >= 0; rank--)
            {
                int emptyCount = 0;
                for (int file = 0; file < 8; file++)
                {
                    int square = rank * 8 + file;
                    int pieceType = board.GetPieceTypeAtSquare(square);
                    if (pieceType == -1) emptyCount++;
                    else
                    {
                        if (emptyCount > 0) { fen.Append(emptyCount); emptyCount = 0; }
                        char[] chars = { 'P', 'N', 'B', 'R', 'Q', 'K', 'p', 'n', 'b', 'r', 'q', 'k' };
                        fen.Append(chars[pieceType]);
                    }
                }
                if (emptyCount > 0) fen.Append(emptyCount);
                if (rank > 0) fen.Append('/');
            }
            fen.Append(' ');
            fen.Append(board.SideToMove == 0 ? 'w' : 'b');
            fen.Append(" - - 0 1");
            return fen.ToString();
        }

        private string FindPuzzlesCsv()
        {
            DirectoryInfo dir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            while (dir != null)
            {
                string candidate = Path.Combine(dir.FullName, "chessEngine", "src", "Puzzles.csv");
                if (File.Exists(candidate)) return candidate;
                candidate = Path.Combine(dir.FullName, "Puzzles.csv");
                if (File.Exists(candidate)) return candidate;
                dir = dir.Parent;
            }
            return null;
        }

        private void PvpButton_Click(object sender, EventArgs e)
        {
            StartGame(GameMode.Pvp, false, 0);
        }

        private void PvaiButton_Click(object sender, EventArgs e)
        {
            ShowColorSelectScreen();
        }

        private void AivaiButton_Click(object sender, EventArgs e)
        {
            StartGame(GameMode.Aivai, true, 0);
        }

        private void PlayWhiteButton_Click(object sender, EventArgs e)
        {
            StartGame(GameMode.Pvai, true, 0);
        }

        private void PlayBlackButton_Click(object sender, EventArgs e)
        {
            StartGame(GameMode.Pvai, true, 1);
        }

        private void BackButtonToMain_Click(object sender, EventArgs e)
        {
            if (settingsAiVsAiRunning)
            {
                settingsAiVsAiRunning = false;
                settingsBoard?.StopAiLoop();
            }
            ShowMainMenuScreen();
        }

        private void BackButtonToPlay_Click(object sender, EventArgs e)
        {
            ShowPlayScreen();
        }

        private void MenuButton_Click(object sender, EventArgs e)
        {
            chessBoard.ResetBoard();
            chessBoard.StopAiLoop();
            ShowMainMenuScreen();
        }

        private void StartGame(GameMode gameMode, bool useAi, int playerColor)
        {
            currentGameMode = gameMode;

            if (gameModeLabel != null)
            {
                gameModeLabel.Text = GetGameModeText(gameMode, playerColor);
            }

            bool flipBoardEveryMove = gameMode == GameMode.Pvp;


            chessBoard.StartNewGame(useAi, playerColor, flipBoardEveryMove, gameMode == GameMode.Aivai);
            ShowGameScreen();
        }

        private string GetGameModeText(GameMode gameMode, int playerColor)
        {
            if (gameMode == GameMode.Pvp) return "PVP";
            if (gameMode == GameMode.Pvai && playerColor == 0) return "PVAI WHITE";
            if (gameMode == GameMode.Pvai && playerColor == 1) return "PVAI BLACK";
            if (gameMode == GameMode.Aivai) return "AIVAI";

            return "PVP";
        }

        private void ShowMainMenuScreen()
        {
            mainMenuPanel.Visible = true;
            playPanel.Visible = false;
            colorSelectPanel.Visible = false;
            gamePanel.Visible = false;
            settingsPanel.Visible = false;
            if (puzzlePanel != null) puzzlePanel.Visible = false;

            mainMenuPanel.BringToFront();
        }

        private void ShowPlayScreen()
        {
            mainMenuPanel.Visible = false;
            playPanel.Visible = true;
            colorSelectPanel.Visible = false;
            gamePanel.Visible = false;
            settingsPanel.Visible = false;
            if (puzzlePanel != null) puzzlePanel.Visible = false;

            playPanel.BringToFront();
        }

        private void ShowColorSelectScreen()
        {
            mainMenuPanel.Visible = false;
            playPanel.Visible = false;
            colorSelectPanel.Visible = true;
            gamePanel.Visible = false;
            settingsPanel.Visible = false;
            if (puzzlePanel != null) puzzlePanel.Visible = false;

            colorSelectPanel.BringToFront();
        }

        private void ShowGameScreen()
        {
            mainMenuPanel.Visible = false;
            playPanel.Visible = false;
            colorSelectPanel.Visible = false;
            gamePanel.Visible = true;
            settingsPanel.Visible = false;
            if (puzzlePanel != null) puzzlePanel.Visible = false;

            gamePanel.BringToFront();

            BeginInvoke(new MethodInvoker(delegate
            {
                chessBoard.Focus();
            }));
        }

        private void InitializeComponent()
        {

        }

        private void ShowSettingsScreen()
        {
            mainMenuPanel.Visible = false;
            playPanel.Visible = false;
            colorSelectPanel.Visible = false;
            gamePanel.Visible = false;
            settingsPanel.Visible = true;
            if (puzzlePanel != null) puzzlePanel.Visible = false;

            settingsPanel.BringToFront();

            if (settingsBoard != null && chessBoard != null)
            {
                string fen = chessBoard.GetFen();
                settingsBoard.LoadFenPosition(fen, out _);
                if (fenTextBox != null)
                    fenTextBox.Text = fen;
            }

            settingsAiVsAiRunning = false;
            if (settingsBoard != null) settingsBoard.EditorMode = true;
            if (solvePuzzlesButton != null)
            {
                solvePuzzlesButton.Text = "Start AI vs AI";
                solvePuzzlesButton.BackColor = Color.FromArgb(75, 105, 55);
            }
            if (puzzleProgressLabel != null) puzzleProgressLabel.Text = "";
        }
    }

    public class ChessBoardControl : Control
    {
        private readonly Dictionary<string, Image> pieceImages = new Dictionary<string, Image>();

        private static bool engineHasBeenInitialized = false;
        private Board engineBoard = new Board();

        private bool aiEnabled = false;
        private int humanColor = 0;
        private int aiColor = 1;
        private int boardPerspective = 0;
        private bool aiMoveQueued = false;
        private bool aiVsAiEnabled = false;
        private bool flipBoardEveryMove = false;
        private bool boardInputLocked = false;
        private int boardFlipDelayMs = 20;
        
        private const int aiVsAiMoveDelayMs = 100;

        private readonly List<Move> selectedPieceLegalMoves = new List<Move>();

        private readonly List<ulong> positionKeyHistory = new List<ulong>();

        private Panel promotionPanel = null;
        private bool promotionChoiceOpen = false;
        private Move pendingPromotionMove;

        private readonly Color lightSquare = Color.FromArgb(238, 238, 210);
        private readonly Color darkSquare = Color.FromArgb(118, 150, 86);
        private readonly Color background = Color.FromArgb(32, 32, 32);
        private readonly Color lightSelectionColor = ColorTranslator.FromHtml("#F5F682");
        private readonly Color darkSelectionColor = ColorTranslator.FromHtml("#B9CA43");

        private int lastMoveFromSquare = -1;
        private int lastMoveToSquare = -1;

        private const int outerPadding = 50;
        private const int engineBarWidth = 34;
        private const int engineBarGap = 18;

        private const int pieceScalePercent = 96;

        private bool isDragging = false;
        private bool hasSelectedPiece = false;
        private int selectedRow = -1;
        private int selectedCol = -1;
        private int selectedEngineSquare = -1;
        private string draggedPiece = "";
        private Point dragPoint = Point.Empty;
        public bool showEngineBar = true;
        public bool ShowEngineBar { get => showEngineBar; set { showEngineBar = value; Invalidate(); } }
        private bool editorMode = false;
        public bool EditorMode
        {
            get => editorMode;
            set => editorMode = value;
        }
        public bool IsGameOver => gameIsOver;
        public int PalettePieceType { get; set; } = -1;
        public Image GetPieceImage(string pieceCode)
        {
            if (pieceImages.TryGetValue(pieceCode, out Image img))
                return img;
            return null;
        }
        public event Action OnBoardChanged;
        public int BoardPerspective
        {
            get => boardPerspective;
            set { boardPerspective = value; Invalidate(); }
        }

        private readonly bool[,] moveDots = new bool[8, 8];
        private readonly bool[,] captureCircles = new bool[8, 8];

        private int engineEvalCentipawns = 0;
        private bool gameIsOver = false;
        private int gameOverState = -1;
        private string gameOverTitle = "";
        private string gameOverSubtitle = "";

        private struct BoardArrow
        {
            public int StartSquare;
            public int EndSquare;

            public BoardArrow(int startSquare, int endSquare)
            {
                StartSquare = startSquare;
                EndSquare = endSquare;
            }
        }
        private struct BoardVisualArrow
        {
            public int StartRow;
            public int StartCol;
            public int EndRow;
            public int EndCol;

            public BoardVisualArrow(int startRow, int startCol, int endRow, int endCol)
            {
                StartRow = startRow;
                StartCol = startCol;
                EndRow = endRow;
                EndCol = endCol;
            }
        }

        private readonly List<BoardArrow> arrows = new List<BoardArrow>();

        private bool isDrawingArrow = false;
        private int arrowStartRow = -1;
        private int arrowStartCol = -1;
        private int arrowCurrentRow = -1;
        private int arrowCurrentCol = -1;
        private readonly bool[] coloredSquares = new bool[64];
        public ChessBoardControl()
        {
            DoubleBuffered = true;
            ResizeRedraw = true;

            InitializeEngineBoard();
            LoadPieceImages();
            InitializeEngineBoard();
            MouseDown += ChessBoardControl_MouseDown;
            MouseMove += ChessBoardControl_MouseMove;
            MouseUp += ChessBoardControl_MouseUp;

            TabStop = true;
            KeyDown += ChessBoardControl_KeyDown;
        }

        public void StartNewGame(bool useAi, int playerColor, bool shouldFlipBoardEveryMove, bool shouldAiVsAi)
        {
            aiEnabled = useAi;
            aiVsAiEnabled = shouldAiVsAi;
            humanColor = playerColor;
            aiColor = playerColor == 0 ? 1 : 0;
            boardPerspective = playerColor;
            flipBoardEveryMove = shouldFlipBoardEveryMove;
            aiMoveQueued = false;
            boardInputLocked = false;

            positionKeyHistory.Clear();
            positionKeyHistory.Add(engineBoard.ZobristKey);

            //ResetBoard();

            UpdateBoardPerspectiveForTurn();
            QueueBotMoveIfNeeded();
        }

        public void StopAiLoop()
        {
            HidePromotionDropdown();

            aiEnabled = false;
            aiVsAiEnabled = false;
            aiMoveQueued = false;
            boardInputLocked = false;
        }

        public void StartAiVsAi()
        {
            aiEnabled = true;
            aiVsAiEnabled = true;
            humanColor = -1;
            aiColor = -1;
            flipBoardEveryMove = false;
            boardPerspective = 0;
            aiMoveQueued = false;
            boardInputLocked = false;
            editorMode = false;

            gameIsOver = false;
            gameOverState = -1;
            gameOverTitle = "";
            gameOverSubtitle = "";

            lastMoveFromSquare = -1;
            lastMoveToSquare = -1;

            QueueBotMoveIfNeeded();
        }

        public bool LoadFenPosition(string fen, out string error) // WIP
        {
            StopAiLoop();

            Board loadedBoard = new Board();

            if (!EngineHelpers.TryLoadFen(loadedBoard, fen, out error))
            {
                return false;
            }

            engineBoard = loadedBoard;

            aiEnabled = false;
            aiVsAiEnabled = false;
            humanColor = 0;
            aiColor = 1;
            flipBoardEveryMove = true;
            aiMoveQueued = false;
            boardInputLocked = false;
            boardPerspective = engineBoard.SideToMove;

            gameIsOver = false;
            gameOverState = -1;
            gameOverTitle = "";
            gameOverSubtitle = "";

            lastMoveFromSquare = -1;
            lastMoveToSquare = -1;
            engineEvalCentipawns = engineBoard.GetBoardEval();

            positionKeyHistory.Clear();
            positionKeyHistory.Add(engineBoard.ZobristKey);

            HidePromotionDropdown();
            ClearSelectedPiece();
            arrows.Clear();
            ClearColoredSquares();
            CheckGameOverState();

            Invalidate();
            return true;
        }

        private Button CreatePromotionButton(string pieceCode, int promotedPieceType, int rowIndex, int squareSize)
        {
            Button button = new Button();

            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.BackColor = Color.White;
            button.Cursor = Cursors.Hand;
            button.Tag = promotedPieceType;

            button.Width = squareSize - 2;
            button.Height = squareSize;
            button.Location = new Point(0, rowIndex * squareSize);

            if (pieceImages.ContainsKey(pieceCode))
            {
                int imageSize = squareSize * 90 / 100;

                button.Image = new Bitmap(pieceImages[pieceCode], imageSize, imageSize);
                button.ImageAlign = ContentAlignment.MiddleCenter;
            }
            else
            {
                button.Text = pieceCode;
                button.Font = new Font("Arial", 12, FontStyle.Bold);
            }

            button.Click += PromotionOptionButton_Click;

            return button;
        }

        private Button CreatePromotionCancelButton(int rowIndex, int squareSize)
        {
            Button button = new Button();

            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.BackColor = Color.FromArgb(240, 240, 240);
            button.ForeColor = Color.FromArgb(120, 120, 120);
            button.Cursor = Cursors.Hand;
            button.Tag = -1;

            button.Text = "✕";
            button.Font = new Font("Arial", 16, FontStyle.Bold);

            button.Width = squareSize - 2;
            button.Height = squareSize;
            button.Location = new Point(0, rowIndex * squareSize);

            button.Click += PromotionOptionButton_Click;

            return button;
        }


        public void ResetBoard()
        {
            InitializeEngineBoard();

            gameIsOver = false;
            gameOverState = -1;
            gameOverTitle = "";
            gameOverSubtitle = "";

            lastMoveFromSquare = -1;
            lastMoveToSquare = -1;

            HidePromotionDropdown();
            ClearSelectedPiece();
            arrows.Clear();
            ClearColoredSquares();
            Invalidate();

        }
        private void UpdateBoardPerspectiveForTurn()
        {
            if (!flipBoardEveryMove)
            {
                return;
            }

            boardPerspective = engineBoard.SideToMove;
            Invalidate();
        }

        private async void QueueBoardPerspectiveFlip()
        {
            if (!flipBoardEveryMove)
            {
                return;
            }

            boardInputLocked = true;

            await Task.Delay(boardFlipDelayMs);

            UpdateBoardPerspectiveForTurn();
            boardInputLocked = false;
        }



        private void InitializeEngineBoard()
        {
            if (!engineHasBeenInitialized)
            {
                EngineHelpers.init();
                engineHasBeenInitialized = true;
            }

            engineBoard = new Board();
            EngineHelpers.InitializeStartingPosition(engineBoard);
        }

        private void ChessBoardControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.E)
            {
                showEngineBar = !showEngineBar;
                Invalidate();
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.C)
            {
                arrows.Clear();
                ClearColoredSquares();
                Invalidate();
                e.Handled = true;
            }
        }

        private void ClearColoredSquares()
        {
            for (int i = 0; i < coloredSquares.Length; i++)
            {
                coloredSquares[i] = false;
            }
        }
        private void ToggleArrow(int startSquare, int endSquare)
        {
            bool arrowAlreadyExists = false;

            for (int i = arrows.Count - 1; i >= 0; i--)
            {
                if (arrows[i].StartSquare == startSquare &&
                    arrows[i].EndSquare == endSquare)
                {
                    arrows.RemoveAt(i);
                    arrowAlreadyExists = true;
                }
            }

            if (!arrowAlreadyExists)
            {
                arrows.Add(new BoardArrow(startSquare, endSquare));
            }
        }


        private void ChessBoardControl_MouseDown(object sender, MouseEventArgs e)
        {
            Focus();

            if (gameIsOver)
            {
                Invalidate();
                return;
            }

            if (promotionChoiceOpen)
            {
                return;
            }

            int row;
            int col;

            if (e.Button == MouseButtons.Right)
            {
                if (editorMode)
                {
                    if (GetSquareFromPoint(e.Location, out row, out col))
                    {
                        int esq = VisualToEngineSquare(row, col);
                        for (int j = 0; j < 12; j++)
                            engineBoard.Pieces[j] &= ~(1UL << esq);
                        engineBoard.ComputeInitialOccupancy();
                        engineBoard.ZobristKey = engineBoard.GenerateKey();
                        lastMoveFromSquare = -1;
                        lastMoveToSquare = -1;
                        OnBoardChanged?.Invoke();
                        Invalidate();
                    }
                    return;
                }

                if (GetSquareFromPoint(e.Location, out row, out col))
                {
                    isDrawingArrow = true;

                    arrowStartRow = row;
                    arrowStartCol = col;
                    arrowCurrentRow = row;
                    arrowCurrentCol = col;

                    Capture = true;
                    Cursor = Cursors.Cross;

                    Invalidate();
                }

                return;
            }

            if (e.Button != MouseButtons.Left)
            {
                return;
            }
            if (boardInputLocked)
            {
                return;
            }
            if (!editorMode && !IsHumanTurn())
            {
                ClearSelectedPiece();
                Invalidate();
                return;
            }

            if (!GetSquareFromPoint(e.Location, out row, out col))
            {
                ClearSelectedPiece();
                Invalidate();
                return;
            }

            int engineSquare = VisualToEngineSquare(row, col);

            if (editorMode && PalettePieceType >= 0)
            {
                for (int j = 0; j < 12; j++)
                    engineBoard.Pieces[j] &= ~(1UL << engineSquare);
                engineBoard.Pieces[PalettePieceType] |= 1UL << engineSquare;
                engineBoard.ComputeInitialOccupancy();
                engineBoard.ZobristKey = engineBoard.GenerateKey();
                lastMoveFromSquare = -1;
                lastMoveToSquare = -1;
                OnBoardChanged?.Invoke();
                Invalidate();
                return;
            }

            if (hasSelectedPiece)
            {
                if (editorMode)
                {
                    MakeEditorMove(selectedEngineSquare, engineSquare);
                    ClearSelectedPiece();
                    Capture = false;
                    Cursor = Cursors.Default;
                    Invalidate();
                    return;
                }
                if (TryMakeSelectedMoveToSquare(engineSquare))
                {
                    ClearSelectedPiece();

                    Capture = false;
                    Cursor = Cursors.Default;

                    Invalidate();
                    return;
                }
            }

            int pieceType = GetPieceTypeAtSquare(engineSquare);

            if (pieceType == -1)
            {
                ClearSelectedPiece();
                Invalidate();
                return;
            }

            int pieceColor = GetColorFromPieceType(pieceType);

            if (!editorMode && pieceColor != engineBoard.SideToMove)
            {
                ClearSelectedPiece();
                Invalidate();
                return;
            }

            if (editorMode)
            {
                selectedRow = row;
                selectedCol = col;
                selectedEngineSquare = engineSquare;
                draggedPiece = GetPieceCodeFromPieceType(pieceType);
                hasSelectedPiece = true;
                ClearMoveHints();
                selectedPieceLegalMoves.Clear();
            }
            else
            {
                SelectPieceAtSquare(row, col, engineSquare, pieceType);
            }

            isDragging = true;
            dragPoint = e.Location;

            Capture = true;
            Cursor = Cursors.Hand;

            Invalidate();
        }

        private void ChessBoardControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                dragPoint = e.Location;
                Invalidate();
            }

            if (isDrawingArrow)
            {
                int row;
                int col;

                if (GetSquareFromPoint(e.Location, out row, out col))
                {
                    arrowCurrentRow = row;
                    arrowCurrentCol = col;
                }
                else
                {
                    arrowCurrentRow = -1;
                    arrowCurrentCol = -1;
                }

                Invalidate();
            }
        }

        private void ChessBoardControl_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (isDrawingArrow)
                {
                    int targRow;
                    int targCol;

                    if (GetSquareFromPoint(e.Location, out targRow, out targCol))
                    {
                        int startSquare = VisualToEngineSquare(arrowStartRow, arrowStartCol);
                        int targetSquare = VisualToEngineSquare(targRow, targCol);

                        bool sameSquare = startSquare == targetSquare;

                        if (sameSquare)
                        {
                            coloredSquares[targetSquare] = !coloredSquares[targetSquare];
                        }
                        else
                        {
                            ToggleArrow(startSquare, targetSquare);
                        }
                    }
                }

                isDrawingArrow = false;
                arrowStartRow = -1;
                arrowStartCol = -1;
                arrowCurrentRow = -1;
                arrowCurrentCol = -1;

                Capture = false;
                Cursor = Cursors.Default;

                Invalidate();
                return;
            }

            if (e.Button != MouseButtons.Left)
            {
                return;
            }

            if (promotionChoiceOpen)
            {
                return;
            }

            if (!isDragging)
            {
                return;
            }

            bool moveWasMade = false;

            int targetRow;
            int targetCol;

            if (GetSquareFromPoint(e.Location, out targetRow, out targetCol))
            {
                int targetEngineSquare = VisualToEngineSquare(targetRow, targetCol);

                if (targetEngineSquare != selectedEngineSquare)
                {
                    if (editorMode)
                    {
                        MakeEditorMove(selectedEngineSquare, targetEngineSquare);
                        moveWasMade = true;
                    }
                    else if (TryMakeSelectedMoveToSquare(targetEngineSquare))
                    {
                        moveWasMade = true;
                    }
                }
            }

            isDragging = false;
            dragPoint = Point.Empty;

            Capture = false;
            Cursor = Cursors.Default;

            if (moveWasMade)
            {
                ClearSelectedPiece();
            }

            Invalidate();
        }

        private void ShowLegalMoveHintsForSquare(int engineSquare)
        {
            ClearMoveHints();
            selectedPieceLegalMoves.Clear();

            Move[] moveBuffer = new Move[500];
            Span<Move> legalMoves = moveBuffer;

            int moveCount = allMoves.GenerateAllLegalMoves(
                engineBoard,
                legalMoves,
                engineBoard.SideToMove
            );

            for (int i = 0; i < moveCount; i++)
            {
                Move move = legalMoves[i];

                if (move.FromSquare == engineSquare)
                {
                    selectedPieceLegalMoves.Add(move);
                    AddMoveHintForLegalMove(move);
                }
            }
        }

        private void SelectPieceAtSquare(int row, int col, int engineSquare, int pieceType)
        {
            ClearMoveHints();
            selectedPieceLegalMoves.Clear();

            selectedRow = row;
            selectedCol = col;
            selectedEngineSquare = engineSquare;
            draggedPiece = GetPieceCodeFromPieceType(pieceType);
            hasSelectedPiece = true;

            ShowLegalMoveHintsForSquare(engineSquare);
        }

        private void ClearSelectedPiece()
        {
            ClearMoveHints();
            selectedPieceLegalMoves.Clear();

            hasSelectedPiece = false;
            isDragging = false;

            selectedRow = -1;
            selectedCol = -1;
            selectedEngineSquare = -1;
            draggedPiece = "";
            dragPoint = Point.Empty;
        }

        private void SetLastMoveHighlight(Move move)
        {
            lastMoveFromSquare = move.FromSquare;
            lastMoveToSquare = move.ToSquare;
        }

        private void CheckGameOverState()
        {
            int state = engineBoard.GetBoardState();

            if (state == -1)
            {
                gameIsOver = false;
                gameOverState = -1;
                gameOverTitle = "";
                gameOverSubtitle = "";
                return;
            }

            gameIsOver = true;
            gameOverState = state;

            HidePromotionDropdown();
            ClearSelectedPiece();
            boardInputLocked = false;
            aiMoveQueued = false;

            if (state == 0)
            {
                gameOverTitle = "WHITE WON";
                gameOverSubtitle = "Checkmate";
            }
            else if (state == 1)
            {
                gameOverTitle = "BLACK WON";
                gameOverSubtitle = "Checkmate";
            }
            else if (state == 2)
            {
                gameOverTitle = "DRAW";
                gameOverSubtitle = "Stalemate";
            }
        }

        private bool CheckThreefoldRepetition()
        {
            if (positionKeyHistory.Count < 3) return false;
            ulong currentKey = engineBoard.ZobristKey;
            int count = 0;
            for (int i = positionKeyHistory.Count - 1; i >= 0; i--)
            {
                if (positionKeyHistory[i] == currentKey) count++;
                if (count >= 3) return true;
            }
            return false;
        }

        private bool TryMakeSelectedMoveToSquare(int targetEngineSquare)
        {
            if (!hasSelectedPiece)
            {
                return false;
            }

            for (int i = 0; i < selectedPieceLegalMoves.Count; i++)
            {
                Move move = selectedPieceLegalMoves[i];

                if (move.FromSquare == selectedEngineSquare && move.ToSquare == targetEngineSquare)
                {
                    if (move.IsPromotion)
                    {
                        ShowPromotionDropdown(move);
                        return true;
                    }

                    MakeHumanMove(move);
                    arrows.Clear();
                    ClearColoredSquares();
                    Invalidate();
                    return true;
                }
            }

            return false;
        }

        private void MakeHumanMove(Move move)
        {
            engineBoard.MakeMove(move);
            SetLastMoveHighlight(move);
            engineEvalCentipawns = engineBoard.GetBoardEval();

            positionKeyHistory.Add(engineBoard.ZobristKey);

            if (CheckThreefoldRepetition())
            {
                gameIsOver = true;
                gameOverState = 2;
                gameOverTitle = "DRAW";
                gameOverSubtitle = "Threefold Repetition";
                HidePromotionDropdown();
                ClearSelectedPiece();
                boardInputLocked = false;
                aiMoveQueued = false;
            }
            else
            {
                CheckGameOverState();
            }

            Invalidate();

            if (!gameIsOver)
            {
                QueueBoardPerspectiveFlip();
                QueueBotMoveIfNeeded();
            }
        }

        private void ShowPromotionDropdown(Move move)
        {
            HidePromotionDropdown();

            pendingPromotionMove = move;
            promotionChoiceOpen = true;
            boardInputLocked = true;

            int engineX;
            int engineY;
            int engineHeight;
            int boardX;
            int boardY;
            int squareSize;

            if (!GetLayoutMetrics(out engineX, out engineY, out engineHeight, out boardX, out boardY, out squareSize))
            {
                promotionChoiceOpen = false;
                boardInputLocked = false;
                return;
            }

            promotionPanel = new Panel();
            promotionPanel.BackColor = Color.White;
            promotionPanel.BorderStyle = BorderStyle.FixedSingle;
            promotionPanel.Width = squareSize;
            promotionPanel.Height = squareSize * 5;

            string queenCode;
            string knightCode;
            string rookCode;
            string bishopCode;

            if (engineBoard.SideToMove == 0)
            {
                queenCode = "wQ";
                knightCode = "wN";
                rookCode = "wR";
                bishopCode = "wB";
            }
            else
            {
                queenCode = "bQ";
                knightCode = "bN";
                rookCode = "bR";
                bishopCode = "bB";
            }

            Button queenButton = CreatePromotionButton(queenCode, 4, 0, squareSize);
            Button knightButton = CreatePromotionButton(knightCode, 1, 1, squareSize);
            Button rookButton = CreatePromotionButton(rookCode, 3, 2, squareSize);
            Button bishopButton = CreatePromotionButton(bishopCode, 2, 3, squareSize);
            Button cancelButton = CreatePromotionCancelButton(4, squareSize);

            promotionPanel.Controls.Add(queenButton);
            promotionPanel.Controls.Add(knightButton);
            promotionPanel.Controls.Add(rookButton);
            promotionPanel.Controls.Add(bishopButton);
            promotionPanel.Controls.Add(cancelButton);

            Controls.Add(promotionPanel);
            PositionPromotionDropdown();

            promotionPanel.BringToFront();
            Invalidate();
        }

        private void PositionPromotionDropdown()
        {
            if (promotionPanel == null)
            {
                return;
            }

            int engineX;
            int engineY;
            int engineHeight;
            int boardX;
            int boardY;
            int squareSize;

            if (!GetLayoutMetrics(out engineX, out engineY, out engineHeight, out boardX, out boardY, out squareSize))
            {
                return;
            }

            int row;
            int col;
            EngineSquareToVisual(pendingPromotionMove.ToSquare, out row, out col);

            int boardSize = squareSize * 8;
            int panelWidth = squareSize;
            int panelHeight = squareSize * 5;

            int x = boardX + col * squareSize;
            int y;

            // If target square is near the top, open downward.
            // If target square is near the bottom, open upward.
            if (row <= 3)
            {
                y = boardY + row * squareSize;
            }
            else
            {
                y = boardY + row * squareSize - (panelHeight - squareSize);
            }

            if (x < boardX)
            {
                x = boardX;
            }

            if (x + panelWidth > boardX + boardSize)
            {
                x = boardX + boardSize - panelWidth;
            }

            if (y < boardY)
            {
                y = boardY;
            }

            if (y + panelHeight > boardY + boardSize)
            {
                y = boardY + boardSize - panelHeight;
            }

            promotionPanel.Width = panelWidth;
            promotionPanel.Height = panelHeight;
            promotionPanel.Location = new Point(x, y);
        }

        private void PromotionOptionButton_Click(object sender, EventArgs e)
        {
            if (!(sender is Button button))
            {
                return;
            }

            int promotedPieceType = (int)button.Tag;

            if (promotedPieceType == -1)
            {
                HidePromotionDropdown();
                Invalidate();
                return;
            }

            Move move = pendingPromotionMove;
            move.PromotedPieceType = promotedPieceType;
            move.IsPromotion = true;

            HidePromotionDropdown();
            ClearSelectedPiece();

            MakeHumanMove(move);
        }

        private void HidePromotionDropdown()
        {
            promotionChoiceOpen = false;
            boardInputLocked = false;

            if (promotionPanel == null)
            {
                return;
            }

            Controls.Remove(promotionPanel);
            promotionPanel.Dispose();
            promotionPanel = null;
        }
        private bool IsHumanTurn()
        {
            if (aiVsAiEnabled)
            {
                return false;
            }

            if (!aiEnabled)
            {
                return true;
            }

            return engineBoard.SideToMove == humanColor;
        }

        public void MakeEditorMove(int fromSquare, int toSquare)
        {
            int pieceType = GetPieceTypeAtSquare(fromSquare);
            if (pieceType == -1) return;

            engineBoard.Pieces[pieceType] &= ~(1UL << fromSquare);

            for (int i = 0; i < 12; i++)
                engineBoard.Pieces[i] &= ~(1UL << toSquare);

            int targetPieceType = pieceType;
            if (pieceType == 0 && toSquare >= 56)
                targetPieceType = 4;
            else if (pieceType == 6 && toSquare <= 7)
                targetPieceType = 10;

            engineBoard.Pieces[targetPieceType] |= 1UL << toSquare;
            engineBoard.ComputeInitialOccupancy();
            engineBoard.ZobristKey = engineBoard.GenerateKey();
            boardPerspective = engineBoard.SideToMove;
            lastMoveFromSquare = fromSquare;
            lastMoveToSquare = toSquare;
            engineEvalCentipawns = engineBoard.GetBoardEval();
            CheckGameOverState();
            OnBoardChanged?.Invoke();
            Invalidate();
        }

        private void ShowPieceContextMenu(Point location, int targetSquare)
        {
            ContextMenuStrip menu = new ContextMenuStrip();
            menu.BackColor = Color.FromArgb(42, 42, 42);
            menu.ForeColor = Color.White;
            menu.Font = new Font("Arial", 9);

            string[] pieceCodes = { "wP", "wN", "wB", "wR", "wQ", "wK", "bP", "bN", "bB", "bR", "bQ", "bK" };
            string[] pieceNames = { "White Pawn", "White Knight", "White Bishop", "White Rook", "White Queen", "White King",
                                    "Black Pawn", "Black Knight", "Black Bishop", "Black Rook", "Black Queen", "Black King" };
            int[] pieceTypes = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 };

            for (int i = 0; i < pieceCodes.Length; i++)
            {
                int pt = pieceTypes[i];
                int ts = targetSquare;
                ToolStripMenuItem item = new ToolStripMenuItem(pieceNames[i]);

                if (pieceImages.ContainsKey(pieceCodes[i]))
                {
                    int imgSize = 16;
                    item.Image = new Bitmap(pieceImages[pieceCodes[i]], imgSize, imgSize);
                }

                item.Click += (s, e) =>
                {
                    for (int j = 0; j < 12; j++)
                        engineBoard.Pieces[j] &= ~(1UL << ts);
                    engineBoard.Pieces[pt] |= 1UL << ts;
                    engineBoard.ComputeInitialOccupancy();
                    engineBoard.ZobristKey = engineBoard.GenerateKey();
                    lastMoveFromSquare = -1;
                    lastMoveToSquare = -1;
                    OnBoardChanged?.Invoke();
                    Invalidate();
                };
                menu.Items.Add(item);
            }

            menu.Show(this, location);
        }

        public string GetFen()
        {
            System.Text.StringBuilder fen = new System.Text.StringBuilder();
            for (int rank = 7; rank >= 0; rank--)
            {
                int emptyCount = 0;
                for (int file = 0; file < 8; file++)
                {
                    int square = rank * 8 + file;
                    int pieceType = GetPieceTypeAtSquare(square);
                    if (pieceType == -1)
                    {
                        emptyCount++;
                    }
                    else
                    {
                        if (emptyCount > 0) { fen.Append(emptyCount); emptyCount = 0; }
                        fen.Append(PieceTypeToFenChar(pieceType));
                    }
                }
                if (emptyCount > 0) fen.Append(emptyCount);
                if (rank > 0) fen.Append('/');
            }

            fen.Append(' ');
            fen.Append(engineBoard.SideToMove == 0 ? 'w' : 'b');

            fen.Append(' ');
            string castling = "";
            if ((engineBoard.CastlingRights & 1) != 0) castling += "K";
            if ((engineBoard.CastlingRights & 2) != 0) castling += "Q";
            if ((engineBoard.CastlingRights & 4) != 0) castling += "k";
            if ((engineBoard.CastlingRights & 8) != 0) castling += "q";
            fen.Append(castling.Length > 0 ? castling : "-");

            fen.Append(' ');
            if (engineBoard.EnPassantSquare >= 0)
            {
                int epFile = engineBoard.EnPassantSquare % 8;
                int epRank = engineBoard.EnPassantSquare / 8;
                fen.Append((char)('a' + epFile));
                fen.Append((char)('1' + epRank));
            }
            else
            {
                fen.Append('-');
            }

            fen.Append(' ');
            fen.Append(engineBoard.HalfMoveClock);
            fen.Append(" 1");

            return fen.ToString();
        }

        private char PieceTypeToFenChar(int pieceType)
        {
            char[] chars = { 'P', 'N', 'B', 'R', 'Q', 'K', 'p', 'n', 'b', 'r', 'q', 'k' };
            return chars[pieceType];
        }

        private void QueueBotMoveIfNeeded()
        {
            if (!aiEnabled)
            {
                return;
            }

            if (gameIsOver)
            {
                return;
            }

            if (aiMoveQueued)
            {
                return;
            }

            if (!aiVsAiEnabled && engineBoard.SideToMove != aiColor)
            {
                return;
            }

            aiMoveQueued = true;

            int delayMs = 0;

            if (aiVsAiEnabled && lastMoveFromSquare != -1)
            {
                delayMs = aiVsAiMoveDelayMs;
            }

            if (delayMs <= 0)
            {
                _ = MakeBotMoveIfNeededAsync();
                return;
            }

            System.Windows.Forms.Timer aiTimer = new System.Windows.Forms.Timer();
            aiTimer.Interval = delayMs;

            aiTimer.Tick += delegate
            {
                aiTimer.Stop();
                aiTimer.Dispose();

                _ = MakeBotMoveIfNeededAsync();
            };

            aiTimer.Start();
        }
        private bool HasLegalMoves()
        {
            Move[] moveBuffer = new Move[500];
            Span<Move> legalMoves = moveBuffer;

            int moveCount = allMoves.GenerateAllLegalMoves(
                engineBoard,
                legalMoves,
                engineBoard.SideToMove
            );

            return moveCount > 0;
        }


        private async Task MakeBotMoveIfNeededAsync()
        {
            try
            {
                if (!aiEnabled || gameIsOver || (!aiVsAiEnabled && engineBoard.SideToMove != aiColor))
                {
                    aiMoveQueued = false;
                    return;
                }

                if (!HasLegalMoves())
                {
                    aiMoveQueued = false;
                    gameIsOver = true;
                    CheckGameOverState();
                    Invalidate();
                    return;
                }

                int d = 7;
                if (engineBoard.GameType == 1)
                {
                    d = 8;
                }
                else if (engineBoard.GameType == 2)
                {
                    d = 10;
                }

                boardInputLocked = true;

                Board aiSandboxBoard = engineBoard.Clone();

                Move botMove = await Task.Run(() => Bot.Think(aiSandboxBoard, d, 0));

                if (!IsValidBotMove(botMove, engineBoard))
                {
                    gameIsOver = true;
                    CheckGameOverState();
                    boardInputLocked = false;
                    aiMoveQueued = false;
                    Invalidate();
                    return;
                }

                engineBoard.MakeMove(botMove);
                SetLastMoveHighlight(botMove);
                engineEvalCentipawns = engineBoard.GetBoardEval();

                positionKeyHistory.Add(engineBoard.ZobristKey);

                if (CheckThreefoldRepetition())
                {
                    gameIsOver = true;
                    gameOverState = 2;
                    gameOverTitle = "DRAW";
                    gameOverSubtitle = "Threefold Repetition";
                    HidePromotionDropdown();
                    ClearSelectedPiece();
                    boardInputLocked = false;
                    aiMoveQueued = false;
                }
                else
                {
                    CheckGameOverState();
                }

                if (!gameIsOver)
                {
                    UpdateBoardPerspectiveForTurn();
                }

                ClearSelectedPiece();
                Invalidate();

                boardInputLocked = false;
                aiMoveQueued = false;

                if (aiVsAiEnabled && !gameIsOver)
                {
                    QueueBotMoveIfNeeded();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ERROR] AI move failed: {ex.Message}");
                boardInputLocked = false;
                aiMoveQueued = false;
                Invalidate();
            }
        }

        private static bool IsValidBotMove(Move move, Board board)
        {
            if (move.FromSquare < 0 || move.FromSquare > 63 ||
                move.ToSquare < 0 || move.ToSquare > 63 ||
                move.PieceType < 0 || move.PieceType > 11)
                return false;

            ulong mask = 1UL << move.FromSquare;
            return (board.Pieces[move.PieceType] & mask) != 0;
        }
        private void AddMoveHintForLegalMove(Move move)
        {
            int row;
            int col;

            EngineSquareToVisual(move.ToSquare, out row, out col);

            if (!IsInsideBoard(row, col))
            {
                return;
            }

            if (move.IsCapture)
            {
                captureCircles[row, col] = true;
            }
            else
            {
                moveDots[row, col] = true;
            }
        }

        private void ClearMoveHints()
        {
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    moveDots[row, col] = false;
                    captureCircles[row, col] = false;
                }
            }
        }

        private int VisualToEngineSquare(int row, int col)
        {
            if (boardPerspective == 0)
            {
                return (7 - row) * 8 + col;
            }

            return row * 8 + (7 - col);
        }

        private void EngineSquareToVisual(int square, out int row, out int col)
        {
            int rank = square / 8;
            int file = square % 8;

            if (boardPerspective == 0)
            {
                row = 7 - rank;
                col = file;
            }
            else
            {
                row = rank;
                col = 7 - file;
            }
        }

        private int GetPieceTypeAtSquare(int square)
        {
            ulong mask = 1UL << square;

            for (int pieceType = 0; pieceType < 12; pieceType++)
            {
                if ((engineBoard.Pieces[pieceType] & mask) != 0)
                {
                    return pieceType;
                }
            }

            return -1;
        }

        private int GetColorFromPieceType(int pieceType)
        {
            if (pieceType >= 0 && pieceType <= 5)
            {
                return 0;
            }

            return 1;
        }

        private string GetPieceCodeFromPieceType(int pieceType)
        {
            if (pieceType == 0) return "wP";
            if (pieceType == 1) return "wN";
            if (pieceType == 2) return "wB";
            if (pieceType == 3) return "wR";
            if (pieceType == 4) return "wQ";
            if (pieceType == 5) return "wK";

            if (pieceType == 6) return "bP";
            if (pieceType == 7) return "bN";
            if (pieceType == 8) return "bB";
            if (pieceType == 9) return "bR";
            if (pieceType == 10) return "bQ";
            if (pieceType == 11) return "bK";

            return "";
        }

        private bool IsInsideBoard(int row, int col)
        {
            return row >= 0 && row < 8 && col >= 0 && col < 8;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (Image img in pieceImages.Values)
                {
                    img.Dispose();
                }

                pieceImages.Clear();
            }

            base.Dispose(disposing);
        }

        private void LoadPieceImages()
        {
            string piecesFolder = FindPiecesFolder();

            if (piecesFolder == null)
            {
                return;
            }

            string[] codes = new string[]
            {
                "wP", "wR", "wN", "wB", "wQ", "wK",
                "bP", "bR", "bN", "bB", "bQ", "bK"
            };

            for (int i = 0; i < codes.Length; i++)
            {
                string code = codes[i];
                string filePath = Path.Combine(piecesFolder, code + ".png");

                if (File.Exists(filePath))
                {
                    using (Image temp = Image.FromFile(filePath))
                    {
                        pieceImages[code] = new Bitmap(temp);
                    }
                }
            }
        }

        private string FindPiecesFolder()
        {
            DirectoryInfo dir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);

            while (dir != null)
            {
                string candidate = Path.Combine(dir.FullName, "Assets", "Pieces");

                if (Directory.Exists(candidate))
                {
                    return candidate;
                }

                dir = dir.Parent;
            }

            return null;
        }

        private bool GetLayoutMetrics(
            out int engineX,
            out int engineY,
            out int engineHeight,
            out int boardX,
            out int boardY,
            out int squareSize)
        {
            engineX = 0;
            engineY = 0;
            engineHeight = 0;
            boardX = 0;
            boardY = 0;
            squareSize = 0;

            int reservedEngineWidth = 0;

            if (showEngineBar)
            {
                reservedEngineWidth = engineBarWidth + engineBarGap;
            }

            int availableWidth = ClientSize.Width - outerPadding * 2 - reservedEngineWidth;
            int availableHeight = ClientSize.Height - outerPadding * 2;

            int boardSize = Math.Min(availableWidth, availableHeight);

            if (boardSize <= 0)
            {
                return false;
            }

            boardSize = boardSize - (boardSize % 8);

            if (boardSize <= 0)
            {
                return false;
            }

            squareSize = boardSize / 8;

            int totalWidth = reservedEngineWidth + boardSize;

            engineX = (ClientSize.Width - totalWidth) / 2;
            boardX = engineX + reservedEngineWidth;

            boardY = (ClientSize.Height - boardSize) / 2;
            engineY = boardY;
            engineHeight = boardSize;

            return true;
        }

        private bool GetSquareFromPoint(Point point, out int row, out int col)
        {
            row = -1;
            col = -1;

            int engineX;
            int engineY;
            int engineHeight;
            int boardX;
            int boardY;
            int squareSize;

            if (!GetLayoutMetrics(out engineX, out engineY, out engineHeight, out boardX, out boardY, out squareSize))
            {
                return false;
            }

            int boardSize = squareSize * 8;

            if (point.X < boardX || point.X >= boardX + boardSize)
            {
                return false;
            }

            if (point.Y < boardY || point.Y >= boardY + boardSize)
            {
                return false;
            }

            col = (point.X - boardX) / squareSize;
            row = (point.Y - boardY) / squareSize;

            return row >= 0 && row < 8 && col >= 0 && col < 8;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;
            g.Clear(background);

            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;

            int engineX;
            int engineY;
            int engineHeight;
            int boardX;
            int boardY;
            int squareSize;

            if (!GetLayoutMetrics(out engineX, out engineY, out engineHeight, out boardX, out boardY, out squareSize))
            {
                return;
            }

            if (promotionChoiceOpen)
            {
                PositionPromotionDropdown();
            }

            if (showEngineBar)
            {
                DrawEngineBar(g, engineX, engineY, engineHeight);
            }

            DrawBoard(g, boardX, boardY, squareSize);
            DrawLastMoveHighlight(g, boardX, boardY, squareSize);
            DrawColoredSquares(g, boardX, boardY, squareSize);
            DrawSelection(g, boardX, boardY, squareSize);
            DrawCoordinates(g, boardX, boardY, squareSize);
            DrawMoveHints(g, boardX, boardY, squareSize);
            DrawArrows(g, boardX, boardY, squareSize);
            DrawPieces(g, boardX, boardY, squareSize);
            DrawDraggedPiece(g, squareSize);
            DrawGameOverScreen(g, boardX, boardY, squareSize);

        }

        private void DrawEngineBar(Graphics g, int engineX, int engineY, int engineHeight)
        {
            int clampedEval = engineEvalCentipawns;

            if (clampedEval > 1000)
            {
                clampedEval = 1000;
            }

            if (clampedEval < -1000)
            {
                clampedEval = -1000;
            }

            double whitePercentDouble;

            if (clampedEval >= 1000)
            {
                whitePercentDouble = 100.0;
            }
            else if (clampedEval <= -1000)
            {
                whitePercentDouble = 0.0;
            }
            else
            {
                double pawns = clampedEval / 100.0;
                whitePercentDouble = 50.0 + pawns * 5.0;
            }

            int whiteHeight = (int)(engineHeight * whitePercentDouble / 100.0);
            int blackHeight = engineHeight - whiteHeight;

            Rectangle blackRect = new Rectangle(engineX, engineY, engineBarWidth, blackHeight);
            Rectangle whiteRect = new Rectangle(engineX, engineY + blackHeight, engineBarWidth, whiteHeight);

            using (Brush blackBrush = new SolidBrush(Color.FromArgb(25, 25, 25)))
            using (Brush whiteBrush = new SolidBrush(Color.FromArgb(235, 235, 235)))
            using (Pen borderPen = new Pen(Color.FromArgb(70, 70, 70), 2))
            {
                g.FillRectangle(blackBrush, blackRect);
                g.FillRectangle(whiteBrush, whiteRect);
                g.DrawRectangle(borderPen, engineX, engineY, engineBarWidth, engineHeight);
            }

            DrawEngineEvalText(g, engineX, engineY, engineHeight, clampedEval);
        }

        private void DrawEngineEvalText(Graphics g, int engineX, int engineY, int engineHeight, int evalCentipawns)
        {
            double pawns = evalCentipawns / 100.0;

            string text;

            if (evalCentipawns > 0)
            {
                text = "+" + pawns.ToString("0.00");
            }
            else
            {
                text = pawns.ToString("0.00");
            }

            using (Font font = new Font("Arial", 8, FontStyle.Bold))
            using (Brush textBrush = new SolidBrush(Color.FromArgb(220, 220, 220)))
            {
                SizeF textSize = g.MeasureString(text, font);

                float x = engineX + (engineBarWidth - textSize.Width) / 2;
                float y = engineY + engineHeight + 6;

                g.DrawString(text, font, textBrush, x, y);
            }
        }

        private void DrawGameOverScreen(Graphics g, int boardX, int boardY, int squareSize)
        {
            if (!gameIsOver)
            {
                return;
            }

            int boardSize = squareSize * 8;

            Rectangle overlayRect = new Rectangle(
                boardX,
                boardY,
                boardSize,
                boardSize
            );

            using (Brush overlayBrush = new SolidBrush(Color.FromArgb(175, 0, 0, 0)))
            {
                g.FillRectangle(overlayBrush, overlayRect);
            }

            int boxWidth = Math.Min(420, boardSize - 80);
            int boxHeight = 190;

            Rectangle boxRect = new Rectangle(
                boardX + (boardSize - boxWidth) / 2,
                boardY + (boardSize - boxHeight) / 2,
                boxWidth,
                boxHeight
            );

            using (GraphicsPath path = RoundedRect(boxRect, 22))
            using (Brush boxBrush = new SolidBrush(Color.FromArgb(245, 245, 245)))
            using (Pen borderPen = new Pen(Color.FromArgb(40, 40, 40), 3))
            {
                g.FillPath(boxBrush, path);
                g.DrawPath(borderPen, path);
            }

            using (Font titleFont = new Font("Arial", 30, FontStyle.Bold))
            using (Font subtitleFont = new Font("Arial", 16, FontStyle.Bold))
            using (Brush titleBrush = new SolidBrush(Color.FromArgb(25, 25, 25)))
            using (Brush subtitleBrush = new SolidBrush(Color.FromArgb(90, 90, 90)))
            {
                StringFormat center = new StringFormat();
                center.Alignment = StringAlignment.Center;
                center.LineAlignment = StringAlignment.Center;

                Rectangle titleRect = new Rectangle(
                    boxRect.X,
                    boxRect.Y + 30,
                    boxRect.Width,
                    55
                );

                Rectangle subtitleRect = new Rectangle(
                    boxRect.X,
                    boxRect.Y + 95,
                    boxRect.Width,
                    40
                );

                g.DrawString(gameOverTitle, titleFont, titleBrush, titleRect, center);
                g.DrawString(gameOverSubtitle, subtitleFont, subtitleBrush, subtitleRect, center);
            }
        }

        private GraphicsPath RoundedRect(Rectangle bounds, int radius)
        {
            int diameter = radius * 2;

            GraphicsPath path = new GraphicsPath();

            path.AddArc(bounds.X, bounds.Y, diameter, diameter, 180, 90);
            path.AddArc(bounds.Right - diameter, bounds.Y, diameter, diameter, 270, 90);
            path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(bounds.X, bounds.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();

            return path;
        }

        private void DrawColoredSquares(Graphics g, int boardX, int boardY, int squareSize)
        {
            Color lightHighlight = ColorTranslator.FromHtml("#EB7D6A");
            Color darkHighlight = ColorTranslator.FromHtml("#D36C50");

            for (int square = 0; square < 64; square++)
            {
                if (!coloredSquares[square])
                {
                    continue;
                }

                int row;
                int col;

                EngineSquareToVisual(square, out row, out col);

                bool isLight = (row + col) % 2 == 0;
                Color color = isLight ? lightHighlight : darkHighlight;

                using (Brush brush = new SolidBrush(color))
                {
                    g.FillRectangle(
                        brush,
                        boardX + col * squareSize,
                        boardY + row * squareSize,
                        squareSize,
                        squareSize
                    );
                }
            }
        }

        private void DrawMoveHints(Graphics g, int boardX, int boardY, int squareSize)
        {
            using (Brush dotBrush = new SolidBrush(Color.FromArgb(110, 30, 30, 30)))
            using (Pen circlePen = new Pen(Color.FromArgb(130, 30, 30, 30), Math.Max(3, squareSize / 18)))
            {
                for (int row = 0; row < 8; row++)
                {
                    for (int col = 0; col < 8; col++)
                    {
                        int squareX = boardX + col * squareSize;
                        int squareY = boardY + row * squareSize;

                        if (moveDots[row, col])
                        {
                            int dotSize = squareSize / 4;

                            Rectangle dotRect = new Rectangle(
                                squareX + (squareSize - dotSize) / 2,
                                squareY + (squareSize - dotSize) / 2,
                                dotSize,
                                dotSize
                            );

                            g.FillEllipse(dotBrush, dotRect);
                        }

                        if (captureCircles[row, col])
                        {
                            int padding = squareSize / 10;

                            Rectangle circleRect = new Rectangle(
                                squareX + padding,
                                squareY + padding,
                                squareSize - padding * 2,
                                squareSize - padding * 2
                            );

                            g.DrawEllipse(circlePen, circleRect);
                        }
                    }
                }
            }
        }

        private void DrawArrows(Graphics g, int boardX, int boardY, int squareSize)
        {
            for (int i = 0; i < arrows.Count; i++)
            {
                DrawSingleArrow(g, boardX, boardY, squareSize, arrows[i], false);
            }

            if (isDrawingArrow && arrowCurrentRow != -1 && arrowCurrentCol != -1)
            {
                bool sameSquare = arrowStartRow == arrowCurrentRow && arrowStartCol == arrowCurrentCol;

                if (!sameSquare)
                {
                    int startSquare = VisualToEngineSquare(arrowStartRow, arrowStartCol);
                    int currentSquare = VisualToEngineSquare(arrowCurrentRow, arrowCurrentCol);

                    BoardArrow previewArrow = new BoardArrow(startSquare, currentSquare);

                    DrawSingleArrow(g, boardX, boardY, squareSize, previewArrow, true);
                }
            }
        }

        private void DrawSingleArrow(Graphics g, int boardX, int boardY, int squareSize, BoardArrow arrow, bool preview)
        {
            int startRow;
            int startCol;
            int endRow;
            int endCol;

            EngineSquareToVisual(arrow.StartSquare, out startRow, out startCol);
            EngineSquareToVisual(arrow.EndSquare, out endRow, out endCol);

            int rowDistance = Math.Abs(endRow - startRow);
            int colDistance = Math.Abs(endCol - startCol);

            bool isKnightArrow =
                (rowDistance == 2 && colDistance == 1) ||
                (rowDistance == 1 && colDistance == 2);

            BoardVisualArrow visualArrow = new BoardVisualArrow(startRow, startCol, endRow, endCol);

            if (isKnightArrow)
            {
                DrawKnightArrow(g, boardX, boardY, squareSize, visualArrow, preview);
            }
            else
            {
                DrawStraightArrow(g, boardX, boardY, squareSize, visualArrow, preview);
            }
        }

        private void DrawStraightArrow(Graphics g, int boardX, int boardY, int squareSize, BoardVisualArrow arrow, bool preview)
        {
            List<PointF> points = new List<PointF>();

            points.Add(GetSquareCenter(boardX, boardY, squareSize, arrow.StartRow, arrow.StartCol));
            points.Add(GetSquareCenter(boardX, boardY, squareSize, arrow.EndRow, arrow.EndCol));

            DrawChessComArrow(g, squareSize, points, preview);
        }

        private void DrawKnightArrow(Graphics g, int boardX, int boardY, int squareSize, BoardVisualArrow arrow, bool preview)
        {
            int rowDiff = arrow.EndRow - arrow.StartRow;
            int colDiff = arrow.EndCol - arrow.StartCol;

            int rowDistance = Math.Abs(rowDiff);
            int colDistance = Math.Abs(colDiff);

            int rowDirection = 0;
            int colDirection = 0;

            if (rowDiff > 0) rowDirection = 1;
            if (rowDiff < 0) rowDirection = -1;

            if (colDiff > 0) colDirection = 1;
            if (colDiff < 0) colDirection = -1;

            List<PointF> points = new List<PointF>();

            PointF start = GetSquareCenter(boardX, boardY, squareSize, arrow.StartRow, arrow.StartCol);
            PointF end = GetSquareCenter(boardX, boardY, squareSize, arrow.EndRow, arrow.EndCol);

            points.Add(start);

            if (rowDistance == 2 && colDistance == 1)
            {
                
                PointF corner = GetSquareCenter(
                    boardX,
                    boardY,
                    squareSize,
                    arrow.StartRow + rowDirection * 2,
                    arrow.StartCol
                );

                points.Add(corner);
            }
            else if (rowDistance == 1 && colDistance == 2)
            {
                
                PointF corner = GetSquareCenter(
                    boardX,
                    boardY,
                    squareSize,
                    arrow.StartRow,
                    arrow.StartCol + colDirection * 2
                );

                points.Add(corner);
            }
            else
            {
                DrawStraightArrow(g, boardX, boardY, squareSize, arrow, preview);
                return;
            }

            points.Add(end);

            DrawChessComArrow(g, squareSize, points, preview);
        }

        private void DrawChessComArrow(Graphics g, int squareSize, List<PointF> points, bool preview)
        {
            if (points == null || points.Count < 2)
            {
                return;
            }

            PointF tip = points[points.Count - 1];
            PointF beforeTip = points[points.Count - 2];

            float dx = tip.X - beforeTip.X;
            float dy = tip.Y - beforeTip.Y;

            float segmentLength = (float)Math.Sqrt(dx * dx + dy * dy);

            if (segmentLength < 1)
            {
                return;
            }

            float ux = dx / segmentLength;
            float uy = dy / segmentLength;

            float px = -uy;
            float py = ux;

            int alpha;

            if (preview)
            {
                alpha = 135;
            }
            else
            {
                alpha = 185;
            }

            Color arrowColor = Color.FromArgb(alpha, 245, 178, 38);

            float lineWidth = (float)Math.Max(12.0, squareSize * 0.23);
            float headLength = (float)(squareSize * 0.34);
            float headWidth = lineWidth * 2.45f;

            if (headLength > segmentLength * 0.70f)
            {
                headLength = segmentLength * 0.70f;
            }

            PointF headBase = new PointF(
                tip.X - ux * headLength,
                tip.Y - uy * headLength
            );

            List<PointF> bodyPoints = new List<PointF>();

            for (int i = 0; i < points.Count - 1; i++)
            {
                bodyPoints.Add(points[i]);
            }

            bodyPoints.Add(headBase);

            using (Pen bodyPen = new Pen(arrowColor, lineWidth))
            using (SolidBrush headBrush = new SolidBrush(arrowColor))
            {
                bodyPen.StartCap = LineCap.Flat;
                bodyPen.EndCap = LineCap.Flat;
                bodyPen.LineJoin = LineJoin.Miter;
                bodyPen.MiterLimit = 2.0f;

                if (bodyPoints.Count >= 2)
                {
                    using (GraphicsPath bodyPath = new GraphicsPath())
                    {
                        bodyPath.AddLines(bodyPoints.ToArray());
                        g.DrawPath(bodyPen, bodyPath);
                    }
                }

                PointF leftHead = new PointF(
                    headBase.X + px * headWidth / 2f,
                    headBase.Y + py * headWidth / 2f
                );

                PointF rightHead = new PointF(
                    headBase.X - px * headWidth / 2f,
                    headBase.Y - py * headWidth / 2f
                );

                PointF[] headPoints = new PointF[]
                {
            tip,
            leftHead,
            rightHead
                };

                g.FillPolygon(headBrush, headPoints);
            }
        }

        private PointF GetSquareCenter(int boardX, int boardY, int squareSize, int row, int col)
        {
            return new PointF(
                boardX + col * squareSize + squareSize / 2f,
                boardY + row * squareSize + squareSize / 2f
            );
        }

        private void DrawBoard(Graphics g, int boardX, int boardY, int squareSize)
        {
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    bool isLight = (row + col) % 2 == 0;
                    Color color = isLight ? lightSquare : darkSquare;

                    using (Brush brush = new SolidBrush(color))
                    {
                        g.FillRectangle(
                            brush,
                            boardX + col * squareSize,
                            boardY + row * squareSize,
                            squareSize,
                            squareSize
                        );
                    }
                }
            }
        }
        private void DrawLastMoveHighlight(Graphics g, int boardX, int boardY, int squareSize)
        {
            if (lastMoveFromSquare == -1 || lastMoveToSquare == -1)
            {
                return;
            }

            DrawSquareHighlight(g, boardX, boardY, squareSize, lastMoveFromSquare);
            DrawSquareHighlight(g, boardX, boardY, squareSize, lastMoveToSquare);
        }

        private void DrawSquareHighlight(Graphics g, int boardX, int boardY, int squareSize, int engineSquare)
        {
            int row;
            int col;

            EngineSquareToVisual(engineSquare, out row, out col);

            if (!IsInsideBoard(row, col))
            {
                return;
            }

            Color color = GetHighlightColorForVisualSquare(row, col);

            using (Brush brush = new SolidBrush(color))
            {
                g.FillRectangle(
                    brush,
                    boardX + col * squareSize,
                    boardY + row * squareSize,
                    squareSize,
                    squareSize
                );
            }
        }

        private Color GetHighlightColorForVisualSquare(int row, int col)
        {
            bool isLightSquare = (row + col) % 2 == 0;

            if (isLightSquare)
            {
                return lightSelectionColor;
            }

            return darkSelectionColor;
        }


        private void DrawSelection(Graphics g, int boardX, int boardY, int squareSize)
        {
            if (!hasSelectedPiece)
            {
                return;
            }

            if (selectedEngineSquare < 0)
            {
                return;
            }

            int row;
            int col;

            EngineSquareToVisual(selectedEngineSquare, out row, out col);

            using (Brush brush = new SolidBrush(GetHighlightColorForVisualSquare(row, col)))
            {
                g.FillRectangle(
                    brush,
                    boardX + col * squareSize,
                    boardY + row * squareSize,
                    squareSize,
                    squareSize
                );
            }
        }

        private void DrawCoordinates(Graphics g, int boardX, int boardY, int squareSize)
        {
            string files = "abcdefgh";

            using (Font font = new Font("Arial", Math.Max(10, squareSize / 6), FontStyle.Bold))
            {
                for (int row = 0; row < 8; row++)
                {
                    for (int col = 0; col < 8; col++)
                    {
                        bool isLight = (row + col) % 2 == 0;
                        Color coordColor = isLight ? darkSquare : lightSquare;

                        using (Brush brush = new SolidBrush(coordColor))
                        {
                            if (col == 0)
                            {
                                string rank;

                                if (boardPerspective == 0)
                                {
                                    rank = (8 - row).ToString();
                                }
                                else
                                {
                                    rank = (row + 1).ToString();
                                }

                                g.DrawString(
                                    rank,
                                    font,
                                    brush,
                                    boardX + col * squareSize + 4,
                                    boardY + row * squareSize + 2
                                );
                            }

                            if (row == 7)
                            {
                                string file;

                                if (boardPerspective == 0)
                                {
                                    file = files[col].ToString();
                                }
                                else
                                {
                                    file = files[7 - col].ToString();
                                }

                                SizeF textSize = g.MeasureString(file, font);

                                g.DrawString(
                                    file,
                                    font,
                                    brush,
                                    boardX + col * squareSize + squareSize - textSize.Width - 4,
                                    boardY + row * squareSize + squareSize - textSize.Height + 2
                                );
                            }
                        }
                    }
                }
            }
        }
        private void DrawPieces(Graphics g, int boardX, int boardY, int squareSize)
        {
            for (int pieceType = 0; pieceType < 12; pieceType++)
            {
                string code = GetPieceCodeFromPieceType(pieceType);

                if (code.Length == 0)
                {
                    continue;
                }

                ulong pieces = engineBoard.Pieces[pieceType];

                for (int square = 0; square < 64; square++)
                {
                    ulong mask = 1UL << square;

                    if ((pieces & mask) == 0)
                    {
                        continue;
                    }

                    if (isDragging && square == selectedEngineSquare)
                    {
                        continue;
                    }

                    int row;
                    int col;

                    EngineSquareToVisual(square, out row, out col);

                    DrawPiece(g, code, boardX, boardY, row, col, squareSize);
                }
            }
        }

        private void DrawPiece(Graphics g, string code, int boardX, int boardY, int row, int col, int squareSize)
        {
            if (!pieceImages.ContainsKey(code))
            {
                DrawMissingPieceDebugText(g, code, boardX, boardY, row, col, squareSize);
                return;
            }

            Image pieceImage = pieceImages[code];

            int pieceSize = squareSize * pieceScalePercent / 100;
            int pieceOffset = (squareSize - pieceSize) / 2;

            Rectangle dest = new Rectangle(
                boardX + col * squareSize + pieceOffset,
                boardY + row * squareSize + pieceOffset,
                pieceSize,
                pieceSize
            );

            g.DrawImage(pieceImage, dest);
        }

        private void DrawDraggedPiece(Graphics g, int squareSize)
        {
            if (!isDragging)
            {
                return;
            }

            if (draggedPiece == null || draggedPiece.Length == 0)
            {
                return;
            }

            if (!pieceImages.ContainsKey(draggedPiece))
            {
                return;
            }

            Image pieceImage = pieceImages[draggedPiece];

            int pieceSize = squareSize * pieceScalePercent / 100;

            Rectangle dest = new Rectangle(
                dragPoint.X - pieceSize / 2,
                dragPoint.Y - pieceSize / 2,
                pieceSize,
                pieceSize
            );

            g.DrawImage(pieceImage, dest);
        }

        private void DrawMissingPieceDebugText(Graphics g, string code, int boardX, int boardY, int row, int col, int squareSize)
        {
            using (Font font = new Font("Arial", Math.Max(10, squareSize / 5), FontStyle.Bold))
            using (Brush brush = new SolidBrush(Color.Red))
            {
                SizeF textSize = g.MeasureString(code, font);

                float x = boardX + col * squareSize + (squareSize - textSize.Width) / 2;
                float y = boardY + row * squareSize + (squareSize - textSize.Height) / 2;

                g.DrawString(code, font, brush, x, y);
            }
        }
    }
}