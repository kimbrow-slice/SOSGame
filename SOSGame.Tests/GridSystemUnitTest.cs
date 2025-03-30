using Xunit;
using SOSGame.GUI;
using Avalonia.Controls;
using System;
using System.Linq;

namespace SOSGame.Tests
{
    public class GridSystemUnitTests
    {
        [Fact]
        public void Grid_Initializes_With_Default_Size()
        {
            var gridSystem = new GridSystem();
            int initialSize = gridSystem.GetGridSize();
            Assert.Equal(3, initialSize);
        }

        [Fact]
        public void Grid_Updates_Size_When_SetGridSize_Is_Called_With_Valid_Size()
        {
            var gridSystem = new GridSystem();
            gridSystem.SetGridSize(newSize: 5);
            int actualSize = gridSystem.GetGridSize();
            Assert.Equal(5, actualSize);
        }

        [Theory]
        [InlineData(2)]
        [InlineData(13)]
        [InlineData(-1)]
        [InlineData(0)]
        public void SetGridSize_Should_Throw_Exception_When_Invalid_Size_Is_Given(int invalidSize)
        {
            var gridSystem = new GridSystem();
            var ex = Assert.Throws<ArgumentException>(() => gridSystem.SetGridSize(invalidSize));

            Assert.Equal("ERR: The grid size must be between \n 3 and 12.", ex.Message);
        }

        [Fact]
        public void InitializeGrid_Should_Create_Correct_Number_Of_Buttons()
        {
            var gridSystem = new GridSystem();
            gridSystem.SetGridSize(3);
            // Count only Buttons, ignoring the overlay canvas.
            int actualButtonCount = gridSystem.Children.OfType<Button>().Count();
            int expectedButtonCount = 3 * 3; // 3x3 grid
            Assert.Equal(expectedButtonCount, actualButtonCount);
        }

        [Fact]
        public void InitializeGrid_Should_Update_Buttons_When_Size_Changes()
        {
            var gridSystem = new GridSystem();
            gridSystem.SetGridSize(4);
            int actualButtonCount = gridSystem.Children.OfType<Button>().Count();
            int expectedButtonCount = 4 * 4; // 4x4 grid
            Assert.Equal(expectedButtonCount, actualButtonCount);
        }
    }
}
