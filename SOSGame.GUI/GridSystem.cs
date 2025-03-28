using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using System;
using SOSGame.Logic;

namespace SOSGame.GUI
{
    public class GridSystem : Grid
    {
        private int gridSize = 3;
        private bool isPlayerOneTurn = true;
        private MainWindow? parentWindow; // Reference to the main window for accessing UI elements.
        private BaseGame? game; // Holds the game logic instance (can be either SimpleGame or GeneralGame).

        // Overlay canvas for drawing SOS lines.
        private Canvas overlayCanvas = new Canvas
        {
            IsHitTestVisible = false // ensures clicks go through to the buttons
        };

        // Public callback properties for decoupling
        public Action<int, int>? OnScoreUpdated;
        public Action<bool>? OnTurnChanged;
        public Action? OnLetterCleared;
        public Action<string>? OnBannerDisplayed;

        // Initializes the grid UI when a GridSystem instance is created.
        public GridSystem()
        {
            InitializeGrid();
        }

        // Sets the parent window to enable communication with other UI components.
        public void SetParent(MainWindow parent)
        {
            parentWindow = parent;
        }

        // Sets the grid size, validates it, resets the board, and instantiates the appropriate game mode.
        public void SetGridSize(int newSize)
        {
            if (newSize < 3 || newSize > 12)
                throw new ArgumentException("ERR: The grid size must be between \n 3 and 12.");

            gridSize = newSize;
            ResetBoard(); // Clears and reinitializes the grid and overlay.

            if (parentWindow != null)
            {
                // Choose the game mode based on the parent window's selection.
                if (parentWindow.IsGeneralGameModeSelected)
                {
                    // Use the callback for banner display.
                    game = new GeneralGame(gridSize, msg => OnBannerDisplayed?.Invoke(msg));
                }
                else
                {
                    game = new SimpleGame(gridSize, msg => OnBannerDisplayed?.Invoke(msg));
                }
            }
            else
            {
                // Default to SimpleGame if no parent window is set.
                game = new SimpleGame(gridSize);
            }
        }

        // Returns the current grid size.
        public int GetGridSize() => gridSize;

        // Clear all UI elements and SOS lines, then reinitialize the grid for start/new game.
        public void ResetBoard()
        {
            // Remove all existing UI elements and definitions.
            this.Children.Clear();
            this.ColumnDefinitions.Clear();
            this.RowDefinitions.Clear();
            overlayCanvas.Children.Clear();
            InitializeGrid(); // Rebuild the grid and overlay canvas.
        }

        // Base of UI Construction creates rows, columns, and buttons for each cell, and attaches event handlers.
        private void InitializeGrid()
        {
            this.Width = 400;
            this.Height = 400;

            // Create grid structure based on gridSize.
            for (int i = 0; i < gridSize; i++)
            {
                this.ColumnDefinitions.Add(new ColumnDefinition());
                this.RowDefinitions.Add(new RowDefinition());
            }

            for (int row = 0; row < gridSize; row++)
            {
                for (int col = 0; col < gridSize; col++)
                {
                    var cellButton = new Button
                    {
                        Content = "",
                        Background = Brushes.White,
                        Foreground = Brushes.Black,
                        BorderBrush = Brushes.Black,
                        BorderThickness = new Thickness(1),
                        FontSize = 24,
                        FontWeight = FontWeight.Bold,
                        HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch,
                        VerticalAlignment = Avalonia.Layout.VerticalAlignment.Stretch
                    };

                    // Capture row and column values and attach the click event handler.
                    int r = row, c = col;
                    cellButton.Click += (sender, args) => OnCellClicked(cellButton, r, c);

                    Grid.SetRow(cellButton, row);
                    Grid.SetColumn(cellButton, col);
                    this.Children.Add(cellButton);
                }
            }

            // Add the overlay canvas to display SOS lines on top of the grid.
            Grid.SetRowSpan(overlayCanvas, gridSize);
            Grid.SetColumnSpan(overlayCanvas, gridSize);
            this.Children.Add(overlayCanvas);
        }

        // Handles a cell button click by verifying game state, processes the move, updates the UI, and redraws SOS lines.
        private void OnCellClicked(Button button, int row, int col)
        {
            if (parentWindow == null || game == null)
                return;

            // In SimpleGame mode, ignore clicks if the game is currently frozen (post-win state).
            if (game is SOSGame.Logic.SimpleGame sg && sg.IsFrozen)
                return;

            // Ignore the click if the cell already contains a letter.
            if (!string.IsNullOrEmpty(button.Content?.ToString()))
                return;

            // Retrieve the letter selected by the player from the parent window.
            char? selectedLetter = parentWindow.GetSelectedLetter();
            if (selectedLetter == null)
                return;

            // Proceed only if the move is valid.
            bool moveSuccess = game.MakeMove(row, col, selectedLetter.Value);
            if (!moveSuccess)
                return;

            button.Content = selectedLetter.Value;
            button.Foreground = Brushes.Black;

            // Replace direct calls to parentWindow with callbacks.
            OnScoreUpdated?.Invoke(game.GetPlayerOneScore(), game.GetPlayerTwoScore());
            isPlayerOneTurn = game.IsPlayerOneTurn();
            OnTurnChanged?.Invoke(isPlayerOneTurn);
            OnLetterCleared?.Invoke();

            // Update overlay canvas to display any new SOS lines.
            UpdateOverlayCanvas(game);
        }

        // Draws lines connecting cells that form valid SOS sequences using data from the business logic.
        private void UpdateOverlayCanvas(BaseGame baseGame)
        {
            overlayCanvas.Children.Clear();

            // Determine each cell's width and height based on the current grid dimensions.
            double cellWidth = this.Bounds.Width / gridSize;
            double cellHeight = this.Bounds.Height / gridSize;

            // Retrieve each SOS line from the game logic and draw it.
            foreach (var line in baseGame.GetSOSLines())
            {
                double startX = (line.StartCol + 0.5) * cellWidth;
                double startY = (line.StartRow + 0.5) * cellHeight;
                double endX = (line.EndCol + 0.5) * cellWidth;
                double endY = (line.EndRow + 0.5) * cellHeight;

                var shapeLine = new Line
                {
                    StartPoint = new Point(startX, startY),
                    EndPoint = new Point(endX, endY),
                    StrokeThickness = 4,
                    Stroke = line.PlayerColor.Equals("Red", StringComparison.OrdinalIgnoreCase)
                                ? Brushes.Red : Brushes.Blue
                };

                overlayCanvas.Children.Add(shapeLine);
            }
        }
    }
}
