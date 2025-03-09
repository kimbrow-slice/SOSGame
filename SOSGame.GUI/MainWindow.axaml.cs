using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using System;
using SOSGame.Logic;
using SOSGame.Controller;



namespace SOSGame.GUI
{
    public partial class MainWindow : Window
    {
        private GridSystem? gridControl; // Updated from GridControl to GridSystem
        private RadioButton? simpleGameRadio;
        private RadioButton? generalGameRadio;
        private RadioButton? playerVsPlayerRadio;
        private RadioButton? playerVsAIRadio;
        private RadioButton? vulnerableProgramState;
        private TextBox? gridSizeInput;
        private Button? startGameButton;
        private CheckBox? selectSCheckBox;
        private CheckBox? selectOCheckBox;

        public MainWindow()
        {
            AvaloniaXamlLoader.Load(this);
            InitializeComponent();

            gridControl = this.FindControl<GridSystem>("GameGrid");
            if (gridControl != null)
            {
                gridControl.SetParent(this);
            }


            startGameButton = this.FindControl<Button>("StartGameButton");
            selectSCheckBox = this.FindControl<CheckBox>("SelectSCheckBox");
            selectOCheckBox = this.FindControl<CheckBox>("SelectOCheckBox");

            // Ensure checkboxes toggle correctly
            if (selectSCheckBox == null || selectOCheckBox == null)
            {
                System.Diagnostics.Debug.WriteLine("CheckBoxes Not Found");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Found CheckBox!");
            }
            simpleGameRadio = this.FindControl<RadioButton>("SimpleGameRadio");
            generalGameRadio = this.FindControl<RadioButton>("GeneralGameRadio");
            playerVsPlayerRadio = this.FindControl<RadioButton>("PlayerVsPlayerRadio");
            playerVsAIRadio = this.FindControl<RadioButton>("PlayerVsAIRadio");
            vulnerableProgramState = this.FindControl<RadioButton>("VulnerableProgramState");
            gridSizeInput = this.FindControl<TextBox>("GridSizeInput");

            if (startGameButton != null)
            {
                {
                    StartGameButton.Content = "Start Game";
                    startGameButton.Click += (sender, e) => StartGame();
                }
            }
        }


        private void StartGame()
        {
            System.Diagnostics.Debug.WriteLine(" StartGame() Triggered");

            if (!IsSelectionValid())
            {
                System.Diagnostics.Debug.WriteLine("To begin the game, you must first select the board size\nGame Mode (simple/general) \n and Player Mode Selection");
                return;
            }

            if (!int.TryParse(gridSizeInput?.Text, out int gridSize) || gridSize < 3 || gridSize > 12)
            {
                System.Diagnostics.Debug.WriteLine("Invalid board size input. Defaulting to 3.");
                gridSize = 3;
            }

            if (gridControl == null)
            {
                System.Diagnostics.Debug.WriteLine("ERROR: GridControl is NULL");
                return;
            }

            System.Diagnostics.Debug.WriteLine($"Initializing Grid with size {gridSize}");
            gridControl.SetGridSize(gridSize);
        }

        public char? GetSelectedLetter()
        {
            if (selectSCheckBox == null || selectOCheckBox == null)
            {
                System.Diagnostics.Debug.WriteLine("ERROR: Checkboxes not found!");
                return null;
            }

            bool selectedS = selectSCheckBox.IsChecked ?? false;
            bool selectedO = selectOCheckBox.IsChecked ?? false;

            System.Diagnostics.Debug.WriteLine($" Letter Selection -> S: {selectedS}, O: {selectedO}");

            if (selectedS && selectedO)
            {
                System.Diagnostics.Debug.WriteLine("ERROR: Both 'S' and 'O' are selected. Only one should be.");
                return null; // Ensure only one letter is selected
            }

            if (!selectedS && !selectedO)
            {
                System.Diagnostics.Debug.WriteLine(" ERROR: No letter selected!");
                return null;
            }

            return selectedS ? 'S' : 'O';
        }


        public void ClearLetterSelection()
        {
            if (selectSCheckBox != null) selectSCheckBox.IsChecked = false;
            if (selectOCheckBox != null) selectOCheckBox.IsChecked = false;

            System.Diagnostics.Debug.WriteLine(" Letter selection cleared.");
        }


        private bool IsSelectionValid()
        {
            bool gameModeSelected = (simpleGameRadio?.IsChecked == true) || (generalGameRadio?.IsChecked == true);
            bool playerModeSelected = (playerVsPlayerRadio?.IsChecked == true) ||
                                      (playerVsAIRadio?.IsChecked == true) ||
                                      (vulnerableProgramState?.IsChecked == true);
            bool gridSizeValid = int.TryParse(gridSizeInput?.Text, out int gridSize) && gridSize >= 3;

            return gameModeSelected && playerModeSelected && gridSizeValid;
        }
    }
}
