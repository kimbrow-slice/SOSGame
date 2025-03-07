using Xunit;
using SOSGame.GUI;
using Avalonia.Controls;
using System;

namespace SOSGame.Tests
{
    public class GridSystemUnitTests
    {
        // Test for Default size
        [Fact]
        public void Grid_Initializes_With_Default_Size()
        {
            var gridSystem = new GridSystem();

            int initialSize = gridSystem.GetGridSize();

            Assert.Equal(3, initialSize);
        }

        // Test for Updating Board Size
        [Fact]
        public void Grid_Updates_Size_When_SetGridSize_Is_Called_With_Valid_Size()
        {
            var gridSystem = new GridSystem();

            gridSystem.SetGridSize(newSize: 5);
            int actualSize = gridSystem.GetGridSize();

            Assert.Equal(expected: 5, actual: actualSize);
        }

        [Theory]
        [InlineData(2)]
        [InlineData(13)]
        [InlineData(-1)]
        [InlineData(0)]

        // Tests for checking for invalid size
        public void SetGridSize_Should_Throw_Exception_When_Invalid_Size_Is_Given(int invalidSize)
        {
            var gridSystem = new GridSystem();

            var ex = Assert.Throws<ArgumentException>(() => gridSystem.SetGridSize(invalidSize));
            Assert.Equal("ERR: The grid size must be between 3 and 12.", ex.Message);
        }
        \
        // Test for checking number of buttons (grid components)
        [Fact]
        public void InitializeGrid_Should_Create_Correct_Number_Of_Buttons()
        {
            var gridSystem = new GridSystem();
            int expectedButtonCount = 3 * 3; // Default grid size is 3x3

            gridSystem.SetGridSize(3);
            int actualButtonCount = gridSystem.Children.Count;

            Assert.Equal(expectedButtonCount, actualButtonCount);
        }

        // Test for updating grid buttons when size is updated (grid components are considered buttons)
        [Fact]
        public void InitializeGrid_Should_Update_Buttons_When_Size_Changes()
        {
            var gridSystem = new GridSystem();
            gridSystem.SetGridSize(4);

            int actualButtonCount = gridSystem.Children.Count;
            int expectedButtonCount = 4 * 4;

            Assert.Equal(expectedButtonCount, actualButtonCount);
        }
    }
}
