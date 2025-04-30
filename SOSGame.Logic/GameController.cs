using System;
using System.Threading;
using SOSGame.Logic;
using SOSGame.Logic.Players;

namespace SOSGame.Controller
{
    public class GameController
    {
        // Triggered after every placement (human or AI)
        public event Action<int, int, char>? OnMoveMade;

        private readonly BaseGame gameLogic;
        public BaseGame GameLogic => gameLogic;

        public int GridSize { get; }
        public string GameMode { get; }
        public string PlayerMode { get; private set; } = "";
        public char CurrentPlayer { get; private set; } = 'R';   // 'R' = Red, 'B' = Blue

        private readonly IGamePlayer redPlayer;
        private readonly IGamePlayer bluePlayer;
        private readonly Action<string>? bannerCallback;

        // Is the side whose turn it is now controlled by the CPU?
        public bool CurrentPlayerIsCpu
        {
            get
            {
                var current = gameLogic.IsPlayerOneTurn() ? redPlayer : bluePlayer;
                return current is ComputerPlayer;
            }
        }

        //  What letter will the side whose turn it is now place?
        public char CurrentPlayerLetter
        {
            get
            {
                var current = gameLogic.IsPlayerOneTurn() ? redPlayer : bluePlayer;
                return current.DefaultLetter;
            }
        }

        // Human vs Human convenience ctor
        public GameController(int gridSize, string gameMode)
            : this(gridSize, gameMode, false, false, 'S', 'O', null) { }

        // Full constructor: choose CPU/human for each side, assign letters, & supply banner callback.
        public GameController(
            int gridSize,
            string gameMode,
            bool redIsCpu,
            bool blueIsCpu,
            char redLetter,
            char blueLetter,
            Action<string>? bannerCallback = null)
        {
            if (gridSize < 3 || gridSize > 12)
                throw new ArgumentException("Board size must be between 3 and 12.");

            GridSize = gridSize;
            GameMode = gameMode;
            this.bannerCallback = bannerCallback;

            // create board logic and pipe banner messages up
            gameLogic = gameMode switch
            {
                "Simple" => new SimpleGame(gridSize, msg => bannerCallback?.Invoke(msg)),
                "General" => new GeneralGame(gridSize, msg => bannerCallback?.Invoke(msg)),
                _ => throw new ArgumentException("Invalid game mode")
            };

            // player mode label
            PlayerMode = redIsCpu && blueIsCpu
                       ? "Computer vs Computer"
                       : (redIsCpu || blueIsCpu)
                         ? "Human vs Computer"
                         : "Human vs Human";

            // instantiate player strategies
            redPlayer = redIsCpu ? new ComputerPlayer(redLetter) : new HumanPlayer(redLetter);
            bluePlayer = blueIsCpu ? new ComputerPlayer(blueLetter) : new HumanPlayer(blueLetter);
        }

        // Human move entry point. Raises OnMoveMade, switches turn, and auto‑plays any AI turns.
        public bool MakeMove(int row, int col, char letter)
        {
            if (!gameLogic.MakeMove(row, col, letter))
                return false;

            OnMoveMade?.Invoke(row, col, letter);        // notify UI

            if (!gameLogic.IsGameOver())
            {
                SwitchTurn();
                TryAutoPlayNext();
            }
            return true;
        }

        // Runs consecutive AI turns until next human or game end, pausing between moves.

        public async Task TryAutoPlayNext()
        {
            int guard = 0;
            while (!gameLogic.IsGameOver())
            {
                IGamePlayer current = gameLogic.IsPlayerOneTurn() ? redPlayer : bluePlayer;
                if (current is HumanPlayer)
                    break;

                var (r, c, l) = current.GetNextMoveWithLetter(gameLogic);

                if (!gameLogic.MakeMove(r, c, l))
                    break;

                OnMoveMade?.Invoke(r, c, l);

                await Task.Delay(TimeSpan.FromSeconds(1));

                SwitchTurn();

                // safety
                if (++guard > gameLogic.GetBoard().Length)
                    break;
            }
        }




        private void SwitchTurn() =>
            CurrentPlayer = (CurrentPlayer == 'R') ? 'B' : 'R';

        public int GetPlayerOneScore() => gameLogic.GetPlayerOneScore();
        public int GetPlayerTwoScore() => gameLogic.GetPlayerTwoScore();
        public bool IsPlayerOneTurn() => gameLogic.IsPlayerOneTurn();
        public bool IsGameOver() => gameLogic.IsGameOver();
    }
}