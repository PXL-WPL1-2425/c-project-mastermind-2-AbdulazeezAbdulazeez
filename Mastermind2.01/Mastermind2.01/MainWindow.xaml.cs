using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Mastermind2._01
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
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
            MessageBox.Show("Too late mate! Je verliest deze beurt.", "te laat", MessageBoxButton.OK, MessageBoxImage.Warning);
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

            ClearBorders();

            bool isCorrect = true;

            for (int i = 0; i < guesses.Count; i++)
            {
                if (guesses[i] == Random[i])
                {
                    GetLabelForIndex(i).BorderBrush = Brushes.DarkRed;
                    GetLabelForIndex(i).BorderThickness = new Thickness(2);
                }
                else if (Random.Contains(guesses[i]))
                {
                    GetLabelForIndex(i).BorderBrush = Brushes.Wheat;
                    GetLabelForIndex(i).BorderThickness = new Thickness(2);
                    isCorrect = false;
                }
                else
                {
                    isCorrect = false;
                }
            }

            if (isCorrect)
            {
                EndGame(true); 
            }
        }

        private void ClearBorders()
        {
            Label1.BorderBrush = Brushes.Transparent;
            Label2.BorderBrush = Brushes.Transparent;
            Label3.BorderBrush = Brushes.Transparent;
            Label4.BorderBrush = Brushes.Transparent;
        }

        private Label GetLabelForIndex(int index)
        {
            switch (index)
            {
                case 0: return Label1;
                case 1: return Label2;
                case 2: return Label3;
                case 3: return Label4;
                default: return null;
            }
        }

        private void UpdateTitle()
        {
            string solution = string.Join(", ", Random);
            Title = $"MasterMind - Poging {attempts}/{maxAttempts} | Oplossing: {solution}";
        }

        private void ToggleDebug()
        {
            isDebugMode = !isDebugMode;
            DebugTextBox.Visibility = isDebugMode ? Visibility.Visible : Visibility.Hidden;
        }

        private void EndGame(bool hasWon)
        {
            StopCountdown(); 

            if (hasWon)
            {
                MessageBox.Show("Gefeliciteerd! Je hebt de code gekraakt!", "Gewonnen", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                string solution = string.Join(", ", Random);
                MessageBox.Show($"Helaas, je hebt verloren! De juiste code was: {solution}", "Verloren", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            ResetGame();
        }

        private void ResetGame()
        {
            attempts = 1; 
            RandomKleur();
            UpdateTitle();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.F12)
            {
                ToggleDebug();
            }
        }
    }
}
