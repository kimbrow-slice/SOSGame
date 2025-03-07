namespace SOSGame.Logic
{
    public class GameLogic
    {
        private int gridSize;
        private char[,]? grid;
        private int playerOneScore;
        private int playerTwoScore;
        private bool isPlayerOneTurn = true;

        public GameLogic(int gridSize)
        {
            this.gridSize = gridSize;
            grid = new char[gridSize, gridSize];
        }

        public bool MakeMove(int row, int col, char letter)
        {
            // Validate input to ensure only 'S' or 'O' is allowed
            if (row < 0 || col < 0 || row >= gridSize || col >= gridSize || grid[row, col] != '\0')
                return false; // Invalid move

            if (letter != 'S' && letter != 'O')
                return false; // Prevent invalid character input

            // Place the letter on the board
            grid[row, col] = letter;
            int sosCount = CheckForSOS(row, col);

            // If no SOS was formed, switch turns
            if (sosCount == 0)
            {
                SwitchTurn();
            }
            return true;
        }

       // Placeholder for Win COnditions
        private int CheckForSOS(int row, int col)
        {
            int sosCount = 0;

            // Count all possible SOS formations
            if (CheckHorizontal(row, col)) sosCount++;
            if (CheckVertical(row, col)) sosCount++;
            if (CheckDiagonal(row, col)) sosCount++;

            // If an SOS was formed, update score
            if (sosCount > 0)
            {
                if (isPlayerOneTurn)
                    playerOneScore += sosCount;
                else
                    playerTwoScore += sosCount;
            }

            return sosCount;
        }

        // Placeholder for Win COnditions
        private bool CheckHorizontal(int row, int col)
        {
            return (col >= 1 && col < gridSize - 1 &&
                    grid[row, col - 1] == 'S' && grid[row, col] == 'O' && grid[row, col + 1] == 'S');
        }

        // Placeholder for Win COnditions
        private bool CheckVertical(int row, int col)
        {
            return (row >= 1 && row < gridSize - 1 &&
                    grid[row - 1, col] == 'S' && grid[row, col] == 'O' && grid[row + 1, col] == 'S');
        }

        // Placeholder for Win COnditions
        private bool CheckDiagonal(int row, int col)
        {
            return (row >= 1 && row < gridSize - 1 && col >= 1 && col < gridSize - 1 &&
                    ((grid[row - 1, col - 1] == 'S' && grid[row, col] == 'O' && grid[row + 1, col + 1] == 'S') ||
                     (grid[row - 1, col + 1] == 'S' && grid[row, col] == 'O' && grid[row + 1, col - 1] == 'S')));
        }

        public int GetPlayerOneScore() => playerOneScore;
        public int GetPlayerTwoScore() => playerTwoScore;
        public bool IsPlayerOneTurn() => isPlayerOneTurn;

        // SwitchTurn is now properly handled
       public void SwitchTurn()
{
    isPlayerOneTurn = !isPlayerOneTurn;
            System.Diagnostics.Debug.WriteLine($"Turn Switched: Player {(isPlayerOneTurn ? "1 (Blue)" : "2 (Red)")}");
}

    }
}
