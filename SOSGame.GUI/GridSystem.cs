using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using System;

namespace SOSGame.GUI
{
    public class GridControl : Grid
    {
        private int gridSize = 8;
        private bool isPlayerOneTurn = true;

        public GridControl()
        {
            InitializeGrid();
        }

        // Function to set the grid size and handle exception if player inputs a size too small for the grid size.
        public void SetGridSize(int newSize)
        {
            if (newSize < 4)
            {
                throw new ArgumentException("ERR: The grids size MUST be greater than 3.");
            }

            gridSize = newSize;
            this.Children.Clear();
            this.ColumnDefinitions.Clear();
            this.RowDefinitions.Clear();
            InitializeGrid();
        }

        // Provide initial deminsions for the grid system. Allow the user to update the overall number of boxes in the grid system. As well as basic styling for our grid.
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

                    cellButton.Click += (sender, args) => OnCellClicked(cellButton);

                    Grid.SetRow(cellButton, row);
                    Grid.SetColumn(cellButton, col);
                    this.Children.Add(cellButton);
                }
            }
        }
        // Placeholder to add the ability to allow the user to select wheather they would like to pick letter S or O
        private void OnCellClicked(Button button)
        {
            if (button.Content == null || button.Content.ToString() == "")
            {
                if (isPlayerOneTurn)
                {
                    button.Content = "S";
                    button.Foreground = Brushes.Blue;
                }
                else
                {
                    button.Content = "O";
                    button.Foreground = Brushes.Red;
                }
                isPlayerOneTurn = !isPlayerOneTurn;
            }
        }
    }
}
