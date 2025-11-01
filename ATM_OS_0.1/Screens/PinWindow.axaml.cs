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
        private string _pinCode = "";

        public PinWindow(string cardUID)
        {
            _cardUID = cardUID;
            _repository = new CardHolderRepository();
            
            InitializeComponent();
            
            // Отложенная установка фокуса
            this.Opened += OnWindowOpened;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void OnWindowOpened(object sender, EventArgs e)
        {
            // Устанавливаем фокус после полной загрузки окна
            var pinTextBox = this.FindControl<TextBox>("PinTextBox");
            pinTextBox?.Focus();
        }

        private void UpdatePinDisplay()
        {
            var pinTextBox = this.FindControl<TextBox>("PinTextBox");
            if (pinTextBox != null)
            {
                pinTextBox.Text = new string('*', _pinCode.Length);
            }
        }

        private void NumberButton_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            
            if (_pinCode.Length < 4)
            {
                _pinCode += button.Content.ToString();
                UpdatePinDisplay();
            }
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            _pinCode = "";
            UpdatePinDisplay();
            
            var errorText = this.FindControl<TextBlock>("ErrorText");
            if (errorText != null)
                errorText.IsVisible = false;
        }

        private async void OkButton_Click(object sender, RoutedEventArgs e)
        {
            var errorText = this.FindControl<TextBlock>("ErrorText");

            if (_pinCode.Length != 4)
            {
                ShowError("PIN must be 4 digits");
                return;
            }

            // Проверяем PIN
            bool isValid = _repository.checkPIN(_cardUID, _pinCode);
            
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
                    _pinCode = "";
                    UpdatePinDisplay();
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
            var errorText = this.FindControl<TextBlock>("ErrorText");
            if (errorText != null)
            {
                errorText.Text = message;
                errorText.IsVisible = true;
            }
        }
    }
}