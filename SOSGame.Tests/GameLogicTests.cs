using Xunit;
using SOSGame.Logic;

namespace SOSGame.Tests
{
    public class GameLogicTests
    {
        // Test if game will initialize properly
        [Fact]
        public void GameLogic_Initializes_Correctly()
        {
            var game = new GameLogic(5);
            Assert.NotNull(game);
        }
        // Test if making a move actually updates our grid
        [Fact]
        public void MakeMove_Updates_Grid()
        {
            var game = new GameLogic(5);
            bool result = game.MakeMove(2, 2, 'S');

            Assert.True(result);
        }
    }
}
