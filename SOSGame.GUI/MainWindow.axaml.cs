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
        // UI control fields loaded from XAML.
        private GridSystem? gridControl;
        private RadioButton? simpleGameRadio;
        private RadioButton? generalGameRadio;
        private RadioButton? playerVsPlayerRadio;
        private RadioButton? playerVsAIRadio;
        private RadioButton? vulnerableProgramState;
        private TextBlock? victoryBanner;
        private TextBox? gridSizeInput;
        private TextBlock? scoreboardText;
        private Button? startGameButton;
        private Button? newGameButton;
        private CheckBox? selectSCheckBox;
        private CheckBox? selectOCheckBox;
        private CheckBox? saveScoreGameCheckBox;

        public MainWindow()
        {
            AvaloniaXamlLoader.Load(this);
            InitializeComponent();

            // Initialize VictoryBanner control.
            victoryBanner = this.FindControl<TextBlock>("VictoryBanner");

            gridControl = this.FindControl<GridSystem>("GameGrid");
            if (gridControl != null)
            {
                // Set the parent reference for grid events.
                gridControl.SetParent(this);
                // Wire up callbacks so that UI updates are handled here.
                gridControl.OnScoreUpdated = (p1, p2) => UpdateScoreboard(p1, p2);
                gridControl.OnTurnChanged = (isP1) => UpdatePlayerTurnDisplay(isP1);
                gridControl.OnLetterCleared = () => ClearLetterSelection();
                // Instead of using a decoupled banner callback, we pass our own ShowVictoryBanner method.
                gridControl.OnBannerDisplayed = (msg) => ShowVictoryBanner(msg);
            }

            startGameButton = this.FindControl<Button>("StartGameButton");
            newGameButton = this.FindControl<Button>("NewGameButton");
            selectSCheckBox = this.FindControl<CheckBox>("SelectSCheckBox");
            selectOCheckBox = this.FindControl<CheckBox>("SelectOCheckBox");

            // Check for letter selection checkboxes and log result for debugging.
            if (selectSCheckBox == null || selectOCheckBox == null)
            {
                System.Diagnostics.Debug.WriteLine("CheckBoxes Not Found");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Found CheckBox!");
            }

            // Initialize scoreboard display.
            scoreboardText = this.FindControl<TextBlock>("ScoreBoard");
            if (scoreboardText != null)
            {
                scoreboardText.Text = "Red Player: [ 0 | 0 ] : Blue Player";
            }

            // Find additional UI controls.
            saveScoreGameCheckBox = this.FindControl<CheckBox>("SaveScoreCheckBox");
            simpleGameRadio = this.FindControl<RadioButton>("SimpleGameRadio");
            generalGameRadio = this.FindControl<RadioButton>("GeneralGameRadio");
            playerVsPlayerRadio = this.FindControl<RadioButton>("PlayerVsPlayerRadio");
            playerVsAIRadio = this.FindControl<RadioButton>("PlayerVsAIRadio");
            vulnerableProgramState = this.FindControl<RadioButton>("VulnerableProgramState");
            gridSizeInput = this.FindControl<TextBox>("GridSizeInput");

            // Start Game button click event.
            if (startGameButton != null)
            {
                startGameButton.Content = "Start Game";
                startGameButton.Click += (sender, e) => StartGame();
            }
            // New Game button click event.
            if (newGameButton != null)
            {
                newGameButton.Content = "New Game";
                newGameButton.Click += (sender, e) => NewGame();
            }

            // Attach async event handler for saving game score if checkbox is toggled.
            if (saveScoreGameCheckBox != null)
            {
                saveScoreGameCheckBox.IsCheckedChanged += async (sender, e) =>
                {
                    if (saveScoreGameCheckBox.IsChecked ?? false)
                    {
                        await ShowSaveFileDialog();
                    }
                };
            }
        }

        // Starts the game by validating the user selection, setting the grid size, and resetting the victory banner.
        private void StartGame()
        {
            System.Diagnostics.Debug.WriteLine("StartGame() Triggered");

            // Validate that board size, game mode, and player mode are selected.
            if (!IsSelectionValid())
            {
                ShowErrorDialog("Please select a board size, game mode (simple/general), and player mode.");
                return;
            }

            // Parse grid size input and validate.
            if (!int.TryParse(gridSizeInput?.Text, out int gridSize) || gridSize < 3 || gridSize > 12)
            {
                ShowErrorDialog("Invalid board size input. Please enter a number between 3 and 12.");
                gridSize = 3;
            }

            // Ensure the grid control is available.
            if (gridControl == null)
            {
                ShowErrorDialog("ERROR: Grid control not found.");
                return;
            }

            System.Diagnostics.Debug.WriteLine($"Initializing Grid with size {gridSize}");
            // Set the grid size and initialize game board.
            gridControl.SetGridSize(gridSize);
            ResetBanner();
        }

        // Resets the current game state to start a new game.
        private void NewGame()
        {
            System.Diagnostics.Debug.WriteLine("NewGame() Triggered");

            // Ensure the grid control is available.
            if (gridControl == null)
            {
                ShowErrorDialog("ERROR: Grid control not found.");
                return;
            }

            // Reset UI elements related to game end and player turn.
            ResetBanner();
            ClearLetterSelection();

            // Parse grid size input and validate.
            if (!int.TryParse(gridSizeInput?.Text, out int gridSize) || gridSize < 3 || gridSize > 12)
            {
                ShowErrorDialog("Invalid board size input. Please enter a number between 3 and 12.");
                gridSize = 3;
            }

            // Reset the grid and create a new game instance.
            gridControl.SetGridSize(gridSize);

            // Reset the scoreboard to initial state.
            UpdateScoreboard(0, 0);
        }

        // Retrieves the letter selected by the user from the checkboxes and validates that only one is selected.
        public char? GetSelectedLetter()
        {
            if (selectSCheckBox == null || selectOCheckBox == null)
            {
                ShowErrorDialog("ERROR: Letter selection checkboxes not found!");
                return null;
            }

            bool selectedS = selectSCheckBox.IsChecked ?? false;
            bool selectedO = selectOCheckBox.IsChecked ?? false;

            System.Diagnostics.Debug.WriteLine($"Letter Selection -> S: {selectedS}, O: {selectedO}");

            // Ensure that both or neither are selected.
            if (selectedS && selectedO)
            {
                ShowErrorDialog("ERROR: Both 'S' and 'O' are selected. Please select only one.");
                return null;
            }

            if (!selectedS && !selectedO)
            {
                ShowErrorDialog("ERROR: No letter selected! Please select 'S' or 'O'.");
                return null;
            }

            // Return the selected letter.
            return selectedS ? 'S' : 'O';
        }

        // Clears the user's letter selection by unchecking both checkboxes.
        public void ClearLetterSelection()
        {
            if (selectSCheckBox != null) selectSCheckBox.IsChecked = false;
            if (selectOCheckBox != null) selectOCheckBox.IsChecked = false;
            System.Diagnostics.Debug.WriteLine("Letter selection cleared.");
        }

        // Displays the victory banner with the provided message, then hides the turn display during victory state.
        public void ShowVictoryBanner(string message)
        {
            var bannerContainer = this.FindControl<Border>("VictoryBannerContainer");
            var bannerText = this.FindControl<TextBlock>("VictoryBannerText");
            var turnDisplay = this.FindControl<TextBlock>("playerTurnDisplay");

            if (bannerContainer != null && bannerText != null && turnDisplay != null)
            {
                bannerText.Text = message;
                bannerContainer.IsVisible = true;
                turnDisplay.IsVisible = false;
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Banner controls not found.");
            }
        }

        // Resets the victory banner and shows the player turn display.
        public void ResetBanner()
        {
            var bannerContainer = this.FindControl<Border>("VictoryBannerContainer");
            var turnDisplay = this.FindControl<TextBlock>("playerTurnDisplay");

            if (bannerContainer != null && turnDisplay != null)
            {
                bannerContainer.IsVisible = false;
                turnDisplay.IsVisible = true;
            }
        }

        // Indicates whether the general game mode is selected via radio button.
        public bool IsGeneralGameModeSelected
        {
            get => generalGameRadio != null && generalGameRadio.IsChecked == true;
        }

        // Updates the scoreboard text with the provided scores.
        public void UpdateScoreboard(int p1Score, int p2Score)
        {
            if (scoreboardText != null)
            {
                scoreboardText.Text = $"Red Player: [ {p1Score} | {p2Score} ] : Blue Player";
            }
        }

        // Updates the display for the current player's turn.
        public void UpdatePlayerTurnDisplay(bool isPlayerOne)
        {
            var turnDisplay = this.FindControl<TextBlock>("playerTurnDisplay");
            if (turnDisplay != null)
            {
                turnDisplay.Text = isPlayerOne ? "Player 1's Turn (Red)" : "Player 2's Turn (Blue)";
                turnDisplay.Foreground = isPlayerOne ? Brushes.Red : Brushes.Blue;
            }
        }

        // Validates that all necessary selections (game mode, player mode, and grid size) are made.
        private bool IsSelectionValid()
        {
            bool gameModeSelected = (simpleGameRadio?.IsChecked == true) || (generalGameRadio?.IsChecked == true);
            bool playerModeSelected = (playerVsPlayerRadio?.IsChecked == true) ||
                                      (playerVsAIRadio?.IsChecked == true) ||
                                      (vulnerableProgramState?.IsChecked == true);
            bool gridSizeValid = int.TryParse(gridSizeInput?.Text, out int gridSize) && gridSize >= 3;
            return gameModeSelected && playerModeSelected && gridSizeValid;
        }

        // Provides a popup dialog with the error message.
        public async void ShowErrorDialog(string message)
        {
            var dialog = new Window
            {
                Title = "Error",
                Width = 600,
                Height = 200,
                Background = Brushes.White,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
            };

            var stackPanel = new StackPanel
            {
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                Spacing = 10
            };

            var messageText = new TextBlock
            {
                Text = message,
                Foreground = Brushes.Black,
                TextAlignment = Avalonia.Media.TextAlignment.Center,
                Margin = new Thickness(10)
            };

            var okButton = new Button
            {
                Content = "OK",
                Width = 80,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center
            };
            okButton.Click += (_, __) => dialog.Close();

            stackPanel.Children.Add(messageText);
            stackPanel.Children.Add(okButton);
            dialog.Content = stackPanel;

            await dialog.ShowDialog(this);
        }

        // Displays a save file dialog to allow the user to save the game score as a CSV file.
        private async Task ShowSaveFileDialog()
        {
            if (StorageProvider != null)
            {
                var file = await StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
                {
                    Title = "Save Game Score",
                    SuggestedFileName = "sos_game_score.csv",
                    FileTypeChoices = new[]
                    {
                        new FilePickerFileType("CSV File")
                        {
                            Patterns = new[] { "*.csv" },
                            MimeTypes = new[] { "text/csv" }
                        }
                    }
                });

                if (file != null)
                {
                    string filePath = file.Path.LocalPath;
                    Console.WriteLine($"File selected: {filePath}");
                    await File.WriteAllTextAsync(filePath, "Player,Score,Color\n");
                }
            }
            else
            {
                ShowErrorDialog("StorageProvider is unavailable.");
            }
        }
    }
}
