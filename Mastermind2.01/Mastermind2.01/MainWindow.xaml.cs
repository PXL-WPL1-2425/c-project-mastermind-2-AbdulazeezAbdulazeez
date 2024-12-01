using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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
        private bool isDebugMode = false; 
        private int countdownSeconds = 0; 
        private const int maxTime = 10; 

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
            StartCountdown(); // Timer resetten bij het genereren van een nieuwe code
            UpdateTitle(); // Titel bijwerken
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
                StopCountdown(); // Timer stoppen als tijd is verstreken
                LoseTurn(); // Beurt verliezen
            }
        }

        private void StartCountdown()
        {
            countdownSeconds = 0; // Reset de timer
            timer.Start();        // Start de timer
        }

        private void StopCountdown()
        {
            timer.Stop(); // Timer stoppen
        }

        private void LoseTurn()
        {
            MessageBox.Show("Te laat! Je verliest deze beurt.", "Te laat", MessageBoxButton.OK, MessageBoxImage.Warning);
            attempts++;

            if (attempts > maxAttempts)
            {
                EndGame(false); // Spel beëindigen als maximale pogingen is bereikt
            }
            else
            {
                UpdateTitle(); // Titel bijwerken na verlies beurt
                StartCountdown(); // Timer opnieuw starten voor de volgende beurt
            }
        }

        private void CheckCodeButton_Click(object sender, RoutedEventArgs e)
        {
            string guess1 = ComboBox1.SelectedItem?.ToString();
            string guess2 = ComboBox2.SelectedItem?.ToString();
            string guess3 = ComboBox3.SelectedItem?.ToString();
            string guess4 = ComboBox4.SelectedItem?.ToString();

            CheckGuesses(guess1, guess2, guess3, guess4);

            StopCountdown(); 

            if (attempts < maxAttempts)
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

        private void CheckGuesses(string guess1, string guess2, string guess3, string guess4)
        {
            List<string> guesses = new List<string> { guess1, guess2, guess3, guess4 };

            StackPanel feedbackPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal, 
                Margin = new Thickness(5)
            };

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

                // Verwerk de feedback (rood/wit)
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
        }

        private void EndGame(bool isWin)
        {
            string message = isWin ? "Gefeliciteerd! Je hebt gewonnen!" : "Helaas, je hebt verloren.";
            MessageBox.Show(message, "Einde Spel", MessageBoxButton.OK, MessageBoxImage.Information);
            RandomKleur(); 
        }

        private void UpdateTitle()
        {
            string secretCode = string.Join(", ", Random);
            this.Title = $"Poging {attempts}/{maxAttempts} | Tijd: {countdownSeconds}s | Geheime Code: {secretCode}";
        }
    }
}
