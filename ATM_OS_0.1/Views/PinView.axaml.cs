using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using System;
using System.Threading.Tasks;

namespace ATM_OS
{
    public partial class PinView : UserControl
    {
        private string _cardUid;
        private int _attempts = 0;
        private const int MaxAttempts = 3;
        private CardHolderRepository _repository;
        private string _pinCode = "";
        
        public event Action<string> OnPinVerified;
        public event Action OnBackToMain;

        public PinView()
        {
            InitializeComponent();
            
            this.Focusable = true;
            this.AddHandler(KeyDownEvent, OnKeyDown, RoutingStrategies.Tunnel);
        }

        public void Initialize(string cardUid)
        {
            _cardUid = cardUid;
            _repository = new CardHolderRepository();
            _pinCode = "";
            _attempts = 0;
            
            this.Focus();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
        
        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key >= Key.D0 && e.Key <= Key.D9)
            {
                char digit = (char)('0' + (e.Key - Key.D0));
                AddDigit(digit);
                e.Handled = true;
            }
            else
            {
                switch (e.Key)
                {
                    case Key.Back:
                        RemoveLastDigit();
                        e.Handled = true;
                        break;
                
                    case Key.Enter:
                        SubmitPin();
                        e.Handled = true;
                        break;
                
                    case Key.Escape:
                        OnBackToMain?.Invoke();
                        e.Handled = true;
                        break;
                }
            }
        }

        private void AddDigit(char digit)
        {
            if (_pinCode.Length < 4)
            {
                _pinCode += digit;
                UpdatePinDisplay();
            }
        }

        private void RemoveLastDigit()
        {
            if (_pinCode.Length > 0)
            {
                _pinCode = _pinCode.Substring(0, _pinCode.Length - 1);
                UpdatePinDisplay();
            }
        }

        private void UpdatePinDisplay()
        {
            var pinTextBox = this.FindControl<TextBox>("PinTextBox");
            pinTextBox.Text = new string('*', _pinCode.Length);
        }
        
        private void NumberButton_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            AddDigit(button.Content.ToString()[0]);
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            _pinCode = "";
            UpdatePinDisplay();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            SubmitPin();
        }

        private async void SubmitPin()
        {
            var errorText = this.FindControl<TextBlock>("ErrorText");

            if (_pinCode.Length != 4)
            {
                ShowError("PIN must be 4 digits");
                return;
            }

            bool isValid = _repository.VerifyPIN(_cardUid, _pinCode);
            
            if (isValid)
            {
                OnPinVerified?.Invoke(_cardUid);
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
            errorText.Text = message;
            errorText.IsVisible = true;
         
        }
    }
}