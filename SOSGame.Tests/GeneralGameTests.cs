using System;

namespace SOSGame.Logic
{
    public class GeneralGame : BaseGame
    {
        // Callback for displaying banner messages.
        private readonly Action<string>? displayBanner;

        public GeneralGame(int size, Action<string>? displayBannerCallback = null)
            : base(size)
        {
            displayBanner = displayBannerCallback;
        }

        public override bool MakeMove(int row, int col, char letter)
        {
            if (!IsValidMove(row, col, letter))
                return false;

            PlaceLetter(row, col, letter); // Tracks totalMoves and sets board

            int newSOS = CountNewSOS(row, col);
            string moveMessage = "";

            if (newSOS > 0)
            {
                if (isPlayerOneTurn)
                    playerOneScore += newSOS;
                else
                    playerTwoScore += newSOS;

                moveMessage = $"Player {(isPlayerOneTurn ? 1 : 2)} formed {newSOS} SOS!";
                // Current player keeps turn if they formed an SOS
            }
            else
            {
                // Switch turn if no SOS
                SwitchTurn();
            }

            // Only declare game over when the board is full
            if (IsGameOver())
            {
                string winner = (playerOneScore > playerTwoScore) ? "Player 1 (Red)" :
                                (playerTwoScore > playerOneScore) ? "Player 2 (Blue)" : "Tie";
                string gameOverMessage = $"Game Over! {winner} wins!";
                string finalMessage = moveMessage.Length > 0 ? $"{moveMessage} {gameOverMessage}" : gameOverMessage;
                displayBanner?.Invoke(finalMessage);
            }

            return true;
        }

        // Use 8 directions (up, down, left, right, and the 4 diagonals)/
        private int CountNewSOS(int row, int col)
        {
            int totalSOS = 0;

            int[,] directions = new int[,]
            {
                { -1,  0 }, {  1,  0 },
                {  0, -1 }, {  0,  1 },
                { -1, -1 }, {  1,  1 },
                { -1,  1 }, {  1, -1 }
            };

            for (int i = 0; i < directions.GetLength(0); i++)
            {
                int dr = directions[i, 0];
                int dc = directions[i, 1];

                // Pattern 1: new cell is the middle 'O'
                totalSOS += CheckTriplet(row - dr, col - dc, row, col, row + dr, col + dc);

                // Pattern 2: new cell is the first 'S'
                totalSOS += CheckTriplet(row, col, row + dr, col + dc, row + 2 * dr, col + 2 * dc);

                // Pattern 3: new cell is the last 'S'
                totalSOS += CheckTriplet(row - 2 * dr, col - 2 * dc, row - dr, col - dc, row, col);
            }

            return totalSOS;
        }

        private int CheckTriplet(int r1, int c1, int r2, int c2, int r3, int c3)
        {
            return base.TryCreateSOSLine(r1, c1, r2, c2, r3, c3);
        }
    }
}
