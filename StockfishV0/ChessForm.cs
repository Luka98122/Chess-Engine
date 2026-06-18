using ChessEngine;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private Label gameModeLabel;
        private TextBox fenTextBox;
        private Label fenStatusLabel;

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

            Button settingsButton = CreateMenuButton("SETTINGS");
            settingsButton.Click += SettingsButton_Click;

            layout.Controls.Add(title, 0, 1);
            layout.Controls.Add(playButton, 0, 2);
            layout.Controls.Add(settingsButton, 0, 3);

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

            TableLayoutPanel layout = CreateMenuLayout();

            Label title = CreateTitleLabel("Settings", 36);

            Label fenLabel = new Label();
            fenLabel.Text = "Paste FEN position:";
            fenLabel.ForeColor = Color.FromArgb(210, 210, 210);
            fenLabel.Font = new Font("Arial", 14, FontStyle.Bold);
            fenLabel.TextAlign = ContentAlignment.MiddleCenter;
            fenLabel.Dock = DockStyle.Fill;

            fenTextBox = new TextBox();
            fenTextBox.Width = 760;
            fenTextBox.Height = 34;
            fenTextBox.Anchor = AnchorStyles.None;
            fenTextBox.Font = new Font("Consolas", 12, FontStyle.Regular);
            fenTextBox.Text = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

            Button loadFenButton = CreateMenuButton("LOAD FEN");
            loadFenButton.Click += LoadFenButton_Click;

            fenStatusLabel = new Label();
            fenStatusLabel.Text = "";
            fenStatusLabel.ForeColor = Color.FromArgb(210, 210, 210);
            fenStatusLabel.Font = new Font("Arial", 11, FontStyle.Bold);
            fenStatusLabel.TextAlign = ContentAlignment.MiddleCenter;
            fenStatusLabel.Dock = DockStyle.Fill;

            Button backButton = CreateMenuButton("BACK");
            backButton.Click += BackButtonToMain_Click;

            layout.Controls.Add(title, 0, 1);
            layout.Controls.Add(fenLabel, 0, 2);
            layout.Controls.Add(fenTextBox, 0, 3);
            layout.Controls.Add(loadFenButton, 0, 4);
            layout.Controls.Add(fenStatusLabel, 0, 5);
            layout.Controls.Add(backButton, 0, 6);

            settingsPanel.Controls.Add(layout);
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
            if (fenTextBox == null)
            {
                return;
            }

            string error;

            if (!chessBoard.LoadFenPosition(fenTextBox.Text, out error))
            {
                if (fenStatusLabel != null)
                {
                    fenStatusLabel.ForeColor = Color.FromArgb(235, 110, 95);
                    fenStatusLabel.Text = "FEN error: " + error;
                }

                return;
            }

            if (gameModeLabel != null)
            {
                gameModeLabel.Text = "CUSTOM FEN";
            }

            if (fenStatusLabel != null)
            {
                fenStatusLabel.ForeColor = Color.FromArgb(120, 220, 120);
                fenStatusLabel.Text = "Loaded.";
            }

            //ShowGameScreen();
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

            mainMenuPanel.BringToFront();
        }

        private void ShowPlayScreen()
        {
            mainMenuPanel.Visible = false;
            playPanel.Visible = true;
            colorSelectPanel.Visible = false;
            gamePanel.Visible = false;
            settingsPanel.Visible = false;

            playPanel.BringToFront();
        }

        private void ShowColorSelectScreen()
        {
            mainMenuPanel.Visible = false;
            playPanel.Visible = false;
            colorSelectPanel.Visible = true;
            gamePanel.Visible = false;
            settingsPanel.Visible = false;

            colorSelectPanel.BringToFront();
        }

        private void ShowGameScreen()
        {
            mainMenuPanel.Visible = false;
            playPanel.Visible = false;
            colorSelectPanel.Visible = false;
            gamePanel.Visible = true;
            settingsPanel.Visible = false;

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

            settingsPanel.BringToFront();
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

        private bool showEngineBar = true;

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

        public bool LoadFenPosition(string fen, out string error) // ***
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
            if (!IsHumanTurn())
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

            // If a piece is already selected, first check whether this click is a legal target.
            if (hasSelectedPiece)
            {
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

            if (pieceColor != engineBoard.SideToMove)
            {
                ClearSelectedPiece();
                Invalidate();
                return;
            }

            SelectPieceAtSquare(row, col, engineSquare, pieceType);

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
                    if (TryMakeSelectedMoveToSquare(targetEngineSquare))
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

            CheckGameOverState();

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

                int d = 6;
                if (engineBoard.GameType == 1)
                {
                    d = 8;
                }
                else if (engineBoard.GameType == 2)
                {
                    d = 11;
                }

                boardInputLocked = true;

                Board aiSandboxBoard = engineBoard.Clone();

                var sw = Stopwatch.StartNew();
                Move botMove = await Task.Run(() => Bot.Think(aiSandboxBoard, d, 0));
                Debug.WriteLine($"[PERF] Total Think(depth={d}) took {sw.Elapsed.TotalSeconds:F2}s");

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

                CheckGameOverState();

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
                // Two squares vertically, then one square horizontally.
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
                // Two squares horizontally, then one square vertically.
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