using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System;
using System.Threading.Tasks;

namespace ATM_OS
{
    public partial class PinWindow : Window
    {
        private string _cardUID;
        private int _attempts = 0;
        private const int MaxAttempts = 3;
        private CardHolderRepository _repository;
        
        private TextBox _pinTextBox;
        private TextBlock _errorText;

        public PinWindow(string cardUID)
        {
            _cardUID = cardUID;
            _repository = new CardHolderRepository();
            
            InitializeComponent();
            
            // Находим элементы после инициализации
            _pinTextBox = this.FindControl<TextBox>("PinTextBox");
            _errorText = this.FindControl<TextBlock>("ErrorText");
            
            if (_pinTextBox == null)
            {
                throw new Exception("PinTextBox not found in XAML");
            }
            
            // Устанавливаем фокус на поле PIN
            _pinTextBox.Focus();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void NumberButton_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            if (_pinTextBox.Text.Length < 4)
            {
                _pinTextBox.Text += button.Content.ToString();
            }
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            _pinTextBox.Text = "";
            _errorText.IsVisible = false;
        }

        private async void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (_pinTextBox.Text.Length != 4)
            {
                ShowError("PIN must be 4 digits");
                return;
            }

            // Проверяем PIN
            bool isValid = _repository.checkPIN(_cardUID, _pinTextBox.Text);
            
            if (isValid)
            {
                // PIN верный - переходим к основному меню операций
                var operationsWindow = new OperationsWindow(_cardUID);
                operationsWindow.Show();
                this.Close();
            }
            else
            {
                _attempts++;
                if (_attempts >= MaxAttempts)
                {
                    ShowError("Too many failed attempts. Card blocked.");
                    await Task.Delay(3000);
                    // Возвращаемся к главному меню
                    var mainWindow = new MainWindow();
                    mainWindow.Show();
                    this.Close();
                }
                else
                {
                    ShowError($"Invalid PIN. Attempts left: {MaxAttempts - _attempts}");
                    _pinTextBox.Text = "";
                }
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            // Возвращаемся к главному меню
            var mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        private void ShowError(string message)
        {
            _errorText.Text = message;
            _errorText.IsVisible = true;
        }
    }
}