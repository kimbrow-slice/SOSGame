using SOSGame.Logic;

namespace SOSGame.Controller
{
    public class GameController
    {
        private GameLogic gameLogic; // Manage instance of core game logic

        public int GridSize { get; private set; }
        public string GameMode { get; private set; } = "";
        public string PlayerMode { get; private set; } = "";
        public char CurrentPlayer { get; private set; } = 'R'; // Indicate the current player R for Red, B for Blue

        // Constructor only handles game logic
        public GameController(int gridSize)
        {
            if (gridSize < 3 || gridSize > 12)
                throw new ArgumentException("Board size must be between 3 and 12.");

            this.GridSize = gridSize;
            this.gameLogic = new GameLogic(gridSize);
        }

        // Handles move logic
        public bool MakeMove(int row, int col, char letter)
        {
            bool moveSuccess = gameLogic.MakeMove(row, col, letter);
            if (moveSuccess)
            {
                SwitchTurn();
            }
            return moveSuccess;
        }

        // Switches turn between players
        private void SwitchTurn()
        {
            CurrentPlayer = (CurrentPlayer == 'R') ? 'B' : 'R';
        }

        // Gets player scores
        public int GetPlayerOneScore() => gameLogic.GetPlayerOneScore();
        public int GetPlayerTwoScore() => gameLogic.GetPlayerTwoScore();
    }
}
