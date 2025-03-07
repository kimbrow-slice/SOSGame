using Xunit;
using SOSGame.Logic;

namespace SOSGame.Tests
{
    public class GameLogicTests
    {
        // Test if GameLogic initializes properly
        [Fact]
        public void GameLogic_Initializes_Correctly()
        {
            var game = new GameLogic(5);
            Assert.NotNull(game);
        }

        // Test if making a valid move updates the grid correctly
        [Fact]
        public void MakeMove_Updates_Grid()
        {
            var game = new GameLogic(5);
            Assert.True(game.MakeMove(2, 2, 'S'));
        }

        // Test handling of negative coordinates
        [Fact]
        public void MakeMove_ReturnsFalse_WhenCoordinatesNegative()
        {
            var game = new GameLogic(5);
            Assert.False(game.MakeMove(-1, -1, 'S'));
        }

        // Test handling of coordinates outside of grid boundaries
        [Theory]
        [InlineData(5, 0)]
        [InlineData(0, 5)]
        [InlineData(6, 6)]
        public void MakeMove_ReturnsFalse_WhenCoordinatesOutsideGrid(int row, int col)
        {
            var game = new GameLogic(5);
            Assert.False(game.MakeMove(row, col, 'O'));
        }

        // Test handling of invalid letters
        [Theory]
        [InlineData('X')]
        [InlineData('A')]
        [InlineData('1')]
        public void MakeMove_ReturnsFalse_WhenInvalidLetterProvided(char invalidLetter)
        {
            var game = new GameLogic(5);
            Assert.False(game.MakeMove(0, 0, invalidLetter));
        }

        // Test making a move to an already occupied cell
        [Fact]
        public void MakeMove_ReturnsFalse_WhenCellOccupied()
        {
            var game = new GameLogic(3);
            game.MakeMove(0, 0, 'S');
            Assert.False(game.MakeMove(0, 0, 'O'));
        }

        // Test if turns switch after making a move without SOS formation
        [Fact]
        public void MakeMove_SwitchesTurn_WhenNoSOSFormed()
        {
            var game = new GameLogic(3);
            bool initialTurn = game.IsPlayerOneTurn();
            game.MakeMove(0, 0, 'S');
            Assert.NotEqual(initialTurn, game.IsPlayerOneTurn());
        }

        // Direct test of the SwitchTurn method
        [Fact]
        public void SwitchTurn_CorrectlyTogglesTurn()
        {
            var game = new GameLogic(3);
            var initialTurn = game.IsPlayerOneTurn();
            game.SwitchTurn();
            Assert.NotEqual(initialTurn, game.IsPlayerOneTurn());
        }
    }
}