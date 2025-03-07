using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using System;
using SOSGame.Logic;
using SOSGame.Controller;

namespace SOSGame.GUI
{
    public class GridSystem : Grid
    {
        private int gridSize = 3; // Default grid size
        private bool isPlayerOneTurn = true; 
        private MainWindow? parentWindow; // Reference to MainWindow for selected letter
        
        // Default constructor required for initialization
        public GridSystem() 
        {
            InitializeGrid();
        }

        // Associates the grid with the main window
        public void SetParent(MainWindow parent)
        {
            parentWindow = parent;
            System.Diagnostics.Debug.WriteLine("Parent window set."); // Debugging
        }

        // Sets grid size and ensures it's within allowed limits
        public void SetGridSize(int newSize)
        {
            if (newSize < 3 || newSize > 12)
            {
                throw new ArgumentException("ERR: The grid size must be between 3 and 12.");
            }

            gridSize = newSize;
            this.Children.Clear();
            this.ColumnDefinitions.Clear();
            this.RowDefinitions.Clear();
            InitializeGrid();

            System.Diagnostics.Debug.WriteLine($"Grid initialized with size: {gridSize}x{gridSize}"); // Debugging
        }
        public int GetGridSize()
        {
            return gridSize;
        }

        // Initializes the grid UI elements
        private void InitializeGrid()
        {
            this.Width = 400;
            this.Height = 400;

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

                    int r = row, c = col;
                    cellButton.Click += (sender, args) => OnCellClicked(cellButton, r, c);

                    Grid.SetRow(cellButton, row);
                    Grid.SetColumn(cellButton, col);
                    this.Children.Add(cellButton);
                }
            }

            System.Diagnostics.Debug.WriteLine("Grid buttons initialized."); // Debugging
        }

        // Handles cell click event
        private void OnCellClicked(Button button, int row, int col)
        {
            System.Diagnostics.Debug.WriteLine($" Cell Clicked: Row {row}, Col {col}");

            if (parentWindow == null)
            {
                System.Diagnostics.Debug.WriteLine(" ERROR: MainWindow reference not set.");
                return;
            }

            if (button.Content != null && button.Content.ToString() != "")
            {
                System.Diagnostics.Debug.WriteLine(" ERROR: Cell is already occupied.");
                return;
            }

            char? selectedLetter = parentWindow.GetSelectedLetter();
            System.Diagnostics.Debug.WriteLine($" Selected Letter: {selectedLetter}");

            if (selectedLetter == null)
            {
                System.Diagnostics.Debug.WriteLine(" ERROR: No letter selected.");
                return;
            }

            // Place the letter in the grid
            button.Content = selectedLetter.Value;
            button.Foreground = Brushes.Black;

            System.Diagnostics.Debug.WriteLine($"Move placed at Row {row}, Col {col}: {selectedLetter.Value}");

            isPlayerOneTurn = !isPlayerOneTurn;

            // Uncheck checkboxes to force re-selection for next move
            parentWindow.ClearLetterSelection();
        }


    }
}
