using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Mastermind
{
    public partial class MainWindow : Window
    {
        private DispatcherTimer timer = new DispatcherTimer();
        private List<string> kleuren = new List<string> { "Red", "Yellow", "Orange", "White", "Green", "Blue" };
        private List<string> Random = new List<string>();
        private int attempts = 1;
        private const int maxAttempts = 10;
        private int countdownSeconds = 0;
        private const int maxTime = 10;
        private bool gameEnded = false;

        public MainWindow()
        {
            InitializeComponent();
            RandomKleur();
            ComboBoxes();
            timer.Tick += Timer_Tick;
            timer.Interval = new TimeSpan(0, 0, 1);
            StartCountdown();
        }

        private void RandomKleur()
        {
            var random = new Random();
            Random.Clear();

            for (int i = 0; i < 4; i++)
            {
                Random.Add(kleuren[random.Next(kleuren.Count)]);
            }

            DebugTextBox.Text = $"Geheime code: {string.Join(", ", Random)}";
            StartCountdown();
            UpdateTitle();
        }

        private void ComboBoxes()
        {
            ComboBox1.ItemsSource = kleuren;
            ComboBox2.ItemsSource = kleuren;
            ComboBox3.ItemsSource = kleuren;
            ComboBox4.ItemsSource = kleuren;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;

            if (comboBox.SelectedItem != null)
            {
                string selectedColor = comboBox.SelectedItem.ToString();
                Brush brushColor = (Brush)new BrushConverter().ConvertFromString(selectedColor);

                switch (comboBox.Name)
                {
                    case "ComboBox1":
                        Label1.Background = brushColor;
                        break;
                    case "ComboBox2":
                        Label2.Background = brushColor;
                        break;
                    case "ComboBox3":
                        Label3.Background = brushColor;
                        break;
                    case "ComboBox4":
                        Label4.Background = brushColor;
                        break;
                }
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            countdownSeconds++;
            timerLabel.Content = $"Tijd: {countdownSeconds}s";

            if (countdownSeconds >= maxTime)
            {
                StopCountdown();
                LoseTurn();
            }
        }

        private void StartCountdown()
        {
            countdownSeconds = 0;
            timer.Start();
        }

        private void StopCountdown()
        {
            timer.Stop();
        }

        private void LoseTurn()
        {
            MessageBox.Show("Te laat! Je verliest deze beurt.", "Te laat", MessageBoxButton.OK, MessageBoxImage.Warning);
            attempts++;

            if (attempts > maxAttempts)
            {
                EndGame(false);
            }
            else
            {
                UpdateTitle();
                StartCountdown();
            }
        }

        private void CheckCodeButton_Click(object sender, RoutedEventArgs e)
        {
            string guess1 = ComboBox1.SelectedItem?.ToString();
            string guess2 = ComboBox2.SelectedItem?.ToString();
            string guess3 = ComboBox3.SelectedItem?.ToString();
            string guess4 = ComboBox4.SelectedItem?.ToString();

            int score = CheckGuesses(guess1, guess2, guess3, guess4);

            ScoreLabel.Content = $"Score: {score}";

            StopCountdown();

            if (score == 0)
            {
                EndGame(true);
            }
            else if (attempts < maxAttempts)
            {
                attempts++;
                StartCountdown();
                UpdateTitle();
            }
            else
            {
                EndGame(false);
            }
        }

        private int CheckGuesses(string guess1, string guess2, string guess3, string guess4)
        {
            List<string> guesses = new List<string> { guess1, guess2, guess3, guess4 };

            StackPanel feedbackPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(5)
            };

            int correctPositions = 0;
            int correctColors = 0;
            int wrongColors = 0;

            List<string> secretCode = new List<string>(Random);

            for (int i = 0; i < guesses.Count; i++)
            {
                if (guesses[i] == secretCode[i])
                {
                    correctPositions++;
                    secretCode[i] = null;
                }
            }

            for (int i = 0; i < guesses.Count; i++)
            {
                if (guesses[i] != null && secretCode.Contains(guesses[i]))
                {
                    correctColors++;
                    secretCode[secretCode.IndexOf(guesses[i])] = null;
                }
            }

            wrongColors = guesses.Count - (correctPositions + correctColors);

            int score = (wrongColors * 2) + (correctColors * 1);

            for (int i = 0; i < guesses.Count; i++)
            {
                Border feedbackBorder = new Border
                {
                    Width = 20,
                    Height = 20,
                    Margin = new Thickness(5),
                    BorderBrush = Brushes.Red,
                    BorderThickness = new Thickness(1)
                };

                if (guesses[i] == Random[i])
                {
                    feedbackBorder.Background = (Brush)new BrushConverter().ConvertFromString(guesses[i]);
                }
                else
                {
                    feedbackBorder.Background = Brushes.White;
                }

                feedbackPanel.Children.Add(feedbackBorder);
            }

            PreviousGuessesPanel.Children.Add(feedbackPanel);
            return score;
        }

        private void EndGame(bool isWin)
        {
            if (gameEnded)
            {
                return;
            }

            gameEnded = true;

            string message = isWin ? "Gefeliciteerd! Je hebt gewonnen!" : $"Helaas, je hebt verloren. De geheime code was: {string.Join(", ", Random)}";
            MessageBoxResult result = MessageBox.Show(message + "\nWil je opnieuw spelen?", "Einde Spel", MessageBoxButton.YesNo, MessageBoxImage.Information);

            if (result == MessageBoxResult.Yes)
            {
                ResetGame();
            }
            else
            {
                Close();
            }
        }

        private void ResetGame()
        {
            gameEnded = false;
            attempts = 1;
            countdownSeconds = 0;
            RandomKleur();
            ScoreLabel.Content = "Score: 0";
            PreviousGuessesPanel.Children.Clear();
            UpdateTitle();
            StartCountdown();
        }

        private void UpdateTitle()
        {
            this.Title = $"Poging {attempts}/{maxAttempts} | Tijd: {countdownSeconds}s";
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (!gameEnded)
            {
                MessageBoxResult result = MessageBox.Show("Je probeert het spel vroegtijdig te beëindigen. Weet je zeker dat je de applicatie wilt afsluiten?", "Beëindigen", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    base.OnClosing(e);
                }
                else
                {
                    e.Cancel = true;
                }
            }
            else
            {
                base.OnClosing(e);
            }
        }
    }
}
