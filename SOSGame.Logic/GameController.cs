using SOSGame.Logic;
using System;

namespace SOSGame.Controller
{
    public class GameController
    {
        private BaseGame gameLogic;

        public int GridSize { get; private set; }
        public string GameMode { get; private set; } = "";
        public string PlayerMode { get; private set; } = "";
        public char CurrentPlayer { get; private set; } = 'R'; // R = Red, B = Blue

        public GameController(int gridSize, string gameMode)
        {
            if (gridSize < 3 || gridSize > 12)
                throw new ArgumentException("Board size must be between 3 and 12.");

            GridSize = gridSize;
            GameMode = gameMode;

            // Initialize correct game mode
            if (gameMode == "Simple")
                gameLogic = new SimpleGame(gridSize);
            else if (gameMode == "General")
                gameLogic = new GeneralGame(gridSize);
            else
                throw new ArgumentException("Invalid game mode");
        }

        public bool MakeMove(int row, int col, char letter)
        {
            bool success = gameLogic.MakeMove(row, col, letter);
            if (success)
            {
                if (!gameLogic.IsGameOver())
                    SwitchTurn();
            }
            return success;
        }

        private void SwitchTurn()
        {
            CurrentPlayer = (CurrentPlayer == 'R') ? 'B' : 'R';
        }

        public int GetPlayerOneScore() => gameLogic.GetPlayerOneScore();
        public int GetPlayerTwoScore() => gameLogic.GetPlayerTwoScore();
        public bool IsPlayerOneTurn() => gameLogic.IsPlayerOneTurn();
    }
}
