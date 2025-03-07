using Xunit;
using SOSGame.Controller;
using System;

namespace SOSGame.Tests
{
    public class GameControllerTests
    {
        [Fact]
        public void Constructor_InvalidGridSize_ThrowsArgumentException()
        {
            // Arrange
            int invalidGridSize = 2; // less than the minimum allowed size (3)

            // Act & Assert
            Assert.Throws<ArgumentException>(() => new GameController(invalidGridSize));
        }

        [Fact]
        public void TestTurnSwitching()
        {
            // Arrange
            GameController gameController = new GameController(5);
            char initialPlayer = gameController.CurrentPlayer;

            // Act
            bool moveMade = gameController.MakeMove(0, 0, 'S'); // Assuming the move at (0,0) with letter 'S' is valid

            // Assert
            Assert.True(moveMade);
            Assert.NotEqual(initialPlayer, gameController.CurrentPlayer);
        }
    }
}