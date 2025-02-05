using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using System;
using System.IO;
using System.Threading.Tasks;

namespace SOSGame.GUI
{
    public partial class MainWindow : Window
    {
        // Main UI elements
        private Border? gameBoard;
        private GridControl? gridControl;
        private TextBlock? titleText;
        private RadioButton? playerVsAiRadio;
        private RadioButton? twoPlayerRadio;
        private TextBox? gridSizeInput;
        private CheckBox? saveFileCheckBox;

        // Initalizing the MainWindow and UI elements

        public MainWindow()
        {
            AvaloniaXamlLoader.Load(this);
            InitializeComponent();

            // UI elements based from class names
            gameBoard = this.FindControl<Border>("GameBoard");
            gridControl = this.FindControl<GridControl>("GameGrid");
            titleText = this.FindControl<TextBlock>("TitleText");

            // Player options based from class names
            playerVsAiRadio = this.FindControl<RadioButton>("PlayerVsAIRadio");
            twoPlayerRadio = this.FindControl<RadioButton>("AiVsAiRadio");

            // User inputs based on class names
            gridSizeInput = this.FindControl<TextBox>("GridSizeInput");
            saveFileCheckBox = this.FindControl<CheckBox>("SaveScoreCheckBox");

            // Attaching an event handler for updating grid size when enter is hit
            if (gridSizeInput != null)
            {
                gridSizeInput.KeyDown += async (sender, e) =>
                {
                    if (e.Key == Key.Enter)
                    {
                        await ApplyGridSize();
                    }
                };
            }

            // Attaching an event handler for the checkbox to "Save Game Score"
            if (saveFileCheckBox != null)
            {
                saveFileCheckBox.IsCheckedChanged += async (sender, e) =>
                {
                    if (saveFileCheckBox.IsChecked ?? false)
                    {
                        await ShowSaveFileDialog();
                    }
                };
            }
        }


        // Trying to update grid sizebased on our users input
  
        private async Task ApplyGridSize()
        {
            if (gridSizeInput == null || gridControl == null)
                return;

            try
            {   
                if (int.TryParse(gridSizeInput.Text, out int newSize))
                {   
                    // Input validation checking for postive integers above 3, if it breaks our validation check, prompt the user with an error message describing why it occured
                    if (newSize < 4)
                    {
                        throw new ArgumentException("ERR: The grid size MUST be greater than 3.");
                    }
                    // Update the size of the grid system
                    gridControl.SetGridSize(newSize);
                }
                else
                {
                    throw new ArgumentException("ERR: Invalid input! Please enter a positive integer greater than 3.");
                }
            }
            catch (ArgumentException ex)
            {
                await ShowErrorDialog(ex.Message);
            }
        }

        // Provide the user the error message in a popup dialog box
        private async Task ShowErrorDialog(string message)
        {
            var dialog = new Window
            {
                Title = "Error",
                Width = 400,
                Height = 150,
                Background = Brushes.White
            };

            var stackPanel = new StackPanel
            {
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                Spacing = 10
            };

            // Adding an error message text block
            var messageText = new TextBlock
            {
                Text = message,
                Foreground = Brushes.Black,
                TextAlignment = Avalonia.Media.TextAlignment.Center,
                Margin = new Thickness(10)
            };

            // Creating an "OK" button to close out of error dialogue box
            var okButton = new Button
            {
                Content = "OK",
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                Background = Brushes.LightGray,
                Foreground = Brushes.Black
            };

            okButton.Click += (sender, args) => dialog.Close();

            // Add UI elements to our error and dialog box
            stackPanel.Children.Add(messageText);
            stackPanel.Children.Add(okButton);
            dialog.Content = stackPanel;

            await dialog.ShowDialog(this);
        }

        // Display save file dialog box for user to decide where to save the file to
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

                    // To Do: Future implementation to be able to write the game data to a csv file.
                    await File.WriteAllTextAsync(filePath, "Player,Score,Color\n");
                }
            }
            else
            {
                await ShowErrorDialog("StorageProvider is unavailable.");
            }
        }
    }
}
