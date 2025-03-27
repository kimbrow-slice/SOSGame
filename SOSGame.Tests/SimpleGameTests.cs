using Xunit;
using SOSGame.Logic;

namespace SOSGame.Tests
{
    public class SimpleGameTests
    {
        [Fact]
        public void MakeMove_FormsVerticalSOS_ReturnsTrue()
        {
            var game = new SimpleGame(5);
            game.MakeMove(0, 2, 'S');
            game.MakeMove(1, 2, 'O');
            var result = game.MakeMove(2, 2, 'S'); // Should form SOS vertically
            Assert.True(result);
        }

        [Fact]
        public void MakeMove_FormsHorizontalSOS_ReturnsTrue()
        {
            var game = new SimpleGame(5);
            game.MakeMove(2, 0, 'S');
            game.MakeMove(2, 1, 'O');
            var result = game.MakeMove(2, 2, 'S'); // Should form SOS horizontally
            Assert.True(result);
        }

        [Fact]
        public void MakeMove_FormsDiagonalSOS_ReturnsTrue()
        {
            var game = new SimpleGame(5);
            game.MakeMove(0, 0, 'S');
            game.MakeMove(1, 1, 'O');
            var result = game.MakeMove(2, 2, 'S'); // Should form SOS diagonally
            Assert.True(result);
        }

        [Fact]
        public void MakeMove_InvalidLetter_ReturnsFalse()
        {
            var game = new SimpleGame(5);
            var result = game.MakeMove(0, 0, 'X'); // Invalid letter
            Assert.False(result);
        }

        [Fact]
        public void MakeMove_OutsideBounds_ReturnsFalse()
        {
            var game = new SimpleGame(5);
            var result = game.MakeMove(-1, 0, 'S'); // Out of bounds
            Assert.False(result);
        }

        [Fact]
        public void MakeMove_AlreadyOccupied_ReturnsFalse()
        {
            var game = new SimpleGame(5);
            game.MakeMove(0, 0, 'S');
            var result = game.MakeMove(0, 0, 'O'); // Already occupied
            Assert.False(result);
        }
    }
}
