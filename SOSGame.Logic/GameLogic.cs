namespace SOSGame.Logic
{
    public class GameLogic
    {
        private int gridSize;
        private char[,] grid;

        public GameLogic(int size = 8)
        {
            gridSize = size;
            grid = new char[gridSize, gridSize];
        }

        public bool MakeMove(int row, int col, char letter)
        {
            if (row < 0 || col < 0 || row >= gridSize || col >= gridSize || grid[row, col] != '\0')
                return false;

            grid[row, col] = letter;
            return true;
        }
    }
}
