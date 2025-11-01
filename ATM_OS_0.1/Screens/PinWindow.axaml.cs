using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System;
using System.Threading.Tasks;

namespace ATM_OS
{
    public partial class PinView : UserControl
    {
        private string _cardUID;
        private int _attempts = 0;
        private const int MaxAttempts = 3;
        private CardHolderRepository _repository;
        private string _pinCode = "";

        // Событие для навигации
        public event Action<string> OnPinVerified;
        public event Action OnBackToMain;

        public PinView()
        {
            InitializeComponent();
        }

        public void Initialize(string cardUID)
        {
            _cardUID = cardUID;
            _repository = new CardHolderRepository();
            _pinCode = "";
            _attempts = 0;
            UpdatePinDisplay();
            
            var errorText = this.FindControl<TextBlock>("ErrorText");
            if (errorText != null)
                errorText.IsVisible = false;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
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

            bool isValid = _repository.checkPIN(_cardUID, _pinCode);
            
            if (isValid)
            {
                OnPinVerified?.Invoke(_cardUID);
            }
            else
            {
                _attempts++;
                if (_attempts >= MaxAttempts)
                {
                    ShowError("Too many failed attempts. Card blocked.");
                    await Task.Delay(3000);
                    OnBackToMain?.Invoke();
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
            OnBackToMain?.Invoke();
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