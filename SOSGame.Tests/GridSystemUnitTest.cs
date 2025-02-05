using Xunit;
using Avalonia.Controls;
using SOSGame.GUI;

namespace SOSGame.Tests
{
    public class GridSystemUnitTest
    {
        // Test to validate that the grid starts as an 8x8 format
        [Fact]
        public void Grid_Initializes_With_Default_Size()
        {
            var grid = new GridControl();
            int initialSize = GetGridSize(grid);

            Assert.Equal(8, initialSize);
        }
        // Test to validate whether or not the grid size is updated when the user inputs a new value
        [Fact]
        public void Grid_Updates_When_SetGridSize_Is_Called()
        {
            var grid = new GridControl();
            int newSize = 5;

            grid.SetGridSize(newSize);
            int updatedSize = GetGridSize(grid);

            Assert.Equal(newSize, updatedSize);
        }

        private int GetGridSize(GridControl grid)
        {
            return grid.RowDefinitions.Count;
        }
    }
}

