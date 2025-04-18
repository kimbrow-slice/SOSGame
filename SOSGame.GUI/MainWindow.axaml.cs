using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using System;
using System.IO;
using System.Threading.Tasks;
using SOSGame.Logic;
using SOSGame.Controller;
using Avalonia.Platform.Storage;

namespace SOSGame.GUI
{
    public partial class MainWindow : Window
    {
        // Expose controller to GridSystem
        private GameController? gameController;
        public GameController GameController => gameController!;

        public MainWindow()
        {
            AvaloniaXamlLoader.Load(this);
            InitializeComponent();

            //  Hook current grid callbacks
            GameGrid.SetParent(this);
            GameGrid.OnScoreUpdated = (p1, p2) => UpdateScoreboard(p1, p2);
            GameGrid.OnTurnChanged = isP1 => UpdatePlayerTurnDisplay(isP1);
            GameGrid.OnLetterCleared = () => ClearLetterSelection();
            GameGrid.OnBannerDisplayed = msg => ShowVictoryBanner(msg);

            //  Button handlers 
            StartGameButton.Click += (_, __) => StartGame();
            NewGameButton.Click += (_, __) => NewGame();

            //  Save file checkbox 
            SaveScoreCheckBox.IsCheckedChanged += async (_, __) =>
            {
                if (SaveScoreCheckBox.IsChecked == true)
                    await ShowSaveFileDialog();
            };

            // Initial scoreboard
            ScoreBoard.Text = "Red Player: [ 0 | 0 ] : Blue Player";

            // Enable/disable logic for Start/New
            WireUpEnableLogic();
        }

        // Detect whether General mode is selected
        public bool IsGeneralGameModeSelected =>
            GeneralGameRadio.IsChecked == true;

        private void WireUpEnableLogic()
        {
            // Panels always visible
            redCpuOptionsPanel.IsVisible = true;
            blueCpuOptionsPanel.IsVisible = true;

            void refresh()
            {
                bool valid = IsSelectionValid();
                StartGameButton.IsEnabled = valid;
                NewGameButton.IsEnabled = valid;
            }

            // Player vs Player
            playerVsPlayerRadio.IsCheckedChanged += (_, __) =>
            {
                redHumanRadio.IsChecked = true;
                blueHumanRadio.IsChecked = true;
                redCpuRadio.IsChecked = false;
                blueCpuRadio.IsChecked = false;
                refresh();
            };

            // Player vs AI  
            playerVsAIRadio.IsCheckedChanged += (_, __) =>
            {
                // clear previous selections
                redHumanRadio.IsChecked = false;
                redCpuRadio.IsChecked = false;
                blueHumanRadio.IsChecked = false;
                blueCpuRadio.IsChecked = false;
                refresh();
            };

            // CPU vs CPU
            cpuVsCpuRadio.IsCheckedChanged += (_, __) =>
            {
                redCpuRadio.IsChecked = true;
                blueCpuRadio.IsChecked = true;
                redHumanRadio.IsChecked = false;
                blueHumanRadio.IsChecked = false;
                refresh();
            };

            redHumanRadio.IsCheckedChanged += (_, __) => refresh();
            redCpuRadio.IsCheckedChanged += (_, __) => refresh();
            blueHumanRadio.IsCheckedChanged += (_, __) => refresh();
            blueCpuRadio.IsCheckedChanged += (_, __) => refresh();
        }


        private void StartGame()
        {
            ResetBanner();
            ClearLetterSelection();

            if (!int.TryParse(GridSizeInput.Text, out int gridSize)
                || gridSize < 3 || gridSize > 12)
            {
                ShowErrorDialog(
                  "Invalid board size input. Please enter a number between 3 and 12.");
                gridSize = 3;
            }

            GameGrid.SetGridSize(gridSize);

            bool redIsCpu = redCpuRadio.IsChecked == true;
            bool blueIsCpu = blueCpuRadio.IsChecked == true;

            // if CPU vs CPU mode selected
            if (cpuVsCpuRadio.IsChecked == true)
            {
                redIsCpu = true;
                blueIsCpu = true;
            }

            gameController = new GameController(
                gridSize,
                IsGeneralGameModeSelected ? "General" : "Simple",
                redIsCpu, blueIsCpu,
                'S',
                IsGeneralGameModeSelected ? 'S' : 'O',
                msg => ShowVictoryBanner(msg)
            );

            gameController.OnMoveMade += (r, c, l) =>
            {
                GameGrid.SetCellContent(r, c, l);
                GameGrid.RefreshOverlay();
                UpdateScoreboard(
                    gameController.GetPlayerOneScore(),
                    gameController.GetPlayerTwoScore());
                UpdatePlayerTurnDisplay(gameController.IsPlayerOneTurn());
            };

            GameGrid.SetGameLogic(gameController.GameLogic);

            // If CPU goes first, take turn
            if (((redIsCpu && gameController.CurrentPlayer == 'R') ||
                 (blueIsCpu && gameController.CurrentPlayer == 'B'))
                && !gameController.IsGameOver())
            {
                gameController.TryAutoPlayNext();
            }

            ResetBanner();
            UpdateScoreboard(0, 0);
            UpdatePlayerTurnDisplay(gameController.IsPlayerOneTurn());
        }

        private void NewGame() => StartGame();

        public char? GetSelectedLetter()
        {
            // If current side is CPU, bypass human selection
            if (gameController != null)
            {
                if (gameController.CurrentPlayer == 'R' && redCpuRadio.IsChecked == true)
                    return 'S';
                if (gameController.CurrentPlayer == 'B' && blueCpuRadio.IsChecked == true)
                    return 'O';
            }

            bool s = SelectSCheckBox.IsChecked == true;
            bool o = SelectOCheckBox.IsChecked == true;
            if (s == o)
            {
                ShowErrorDialog("Please select exactly one of S or O.");
                return null;
            }
            return s ? 'S' : 'O';
        }

        public void ClearLetterSelection()
        {
            SelectSCheckBox.IsChecked = false;
            SelectOCheckBox.IsChecked = false;
        }

        public void ShowVictoryBanner(string message)
        {
            VictoryBannerText.Text = message;
            VictoryBannerContainer.IsVisible = true;
            playerTurnDisplay.IsVisible = false;
        }

        public void ResetBanner()
        {
            VictoryBannerContainer.IsVisible = false;
            playerTurnDisplay.IsVisible = true;
        }

        public void UpdateScoreboard(int p1, int p2)
        {
            ScoreBoard.Text = $"Red Player: [ {p1} | {p2} ] : Blue Player";
        }

        public void UpdatePlayerTurnDisplay(bool isP1)
        {
            playerTurnDisplay.Text = isP1
                ? "Player 1's Turn (Red)"
                : "Player 2's Turn (Blue)";
            playerTurnDisplay.Foreground = isP1
                ? Brushes.Red : Brushes.Blue;
        }

        private bool IsSelectionValid()
        {
            bool modeOK = SimpleGameRadio.IsChecked == true || GeneralGameRadio.IsChecked == true;

            bool sideOK =
                 (playerVsPlayerRadio.IsChecked == true
                    && redHumanRadio.IsChecked == true
                    && blueHumanRadio.IsChecked == true)
              || (playerVsAIRadio.IsChecked == true
                    && ((redHumanRadio.IsChecked == true && blueCpuRadio.IsChecked == true)
                     || (redCpuRadio.IsChecked == true && blueHumanRadio.IsChecked == true)))
              || (cpuVsCpuRadio.IsChecked == true
                    && redCpuRadio.IsChecked == true
                    && blueCpuRadio.IsChecked == true);

            bool sizeOK = int.TryParse(GridSizeInput.Text, out int sz) && sz >= 3;

            return modeOK && sideOK && sizeOK;
        }



        public async void ShowErrorDialog(string message)
        {
            var dlg = new Window
            {
                Title = "Error",
                Width = 400,
                Height = 200,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            var txt = new TextBlock { Text = message, Margin = new Thickness(10) };
            var btn = new Button
            {
                Content = "OK",
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center
            };
            btn.Click += (_, __) => dlg.Close();
            dlg.Content = new StackPanel { Children = { txt, btn }, Spacing = 10 };
            await dlg.ShowDialog(this);
        }

        private async Task ShowSaveFileDialog()
        {
            if (StorageProvider != null)
            {
                var file = await StorageProvider
                    .SaveFilePickerAsync(new FilePickerSaveOptions
                    {
                        Title = "Save Game Score",
                        SuggestedFileName = "sos_game_score.csv",
                        FileTypeChoices = new[]
                    {
                        new FilePickerFileType("CSV")
                        {
                            Patterns  = new[] { "*.csv" },
                            MimeTypes = new[] { "text/csv" }
                        }
                    }
                    });
                if (file != null)
                    await File.WriteAllTextAsync(file.Path.LocalPath,
                                                "Player,Score,Color\n");
            }
        }
    }
}
