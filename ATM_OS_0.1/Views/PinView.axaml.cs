using Avalonia;
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
            
            // Устанавливаем фокус на весь UserControl для приема клавиатурного ввода
            this.Focusable = true;
            this.AddHandler(KeyDownEvent, OnKeyDown, RoutingStrategies.Tunnel);
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
            
            // Устанавливаем фокус на контрол для приема клавиш
            this.Focus();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        // Обработчик клавиатуры
        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.D0:
                case Key.NumPad0:
                    AddDigit('0');
                    e.Handled = true;
                    break;
                    
                case Key.D1:
                case Key.NumPad1:
                    AddDigit('1');
                    e.Handled = true;
                    break;
                    
                case Key.D2:
                case Key.NumPad2:
                    AddDigit('2');
                    e.Handled = true;
                    break;
                    
                case Key.D3:
                case Key.NumPad3:
                    AddDigit('3');
                    e.Handled = true;
                    break;
                    
                case Key.D4:
                case Key.NumPad4:
                    AddDigit('4');
                    e.Handled = true;
                    break;
                    
                case Key.D5:
                case Key.NumPad5:
                    AddDigit('5');
                    e.Handled = true;
                    break;
                    
                case Key.D6:
                case Key.NumPad6:
                    AddDigit('6');
                    e.Handled = true;
                    break;
                    
                case Key.D7:
                case Key.NumPad7:
                    AddDigit('7');
                    e.Handled = true;
                    break;
                    
                case Key.D8:
                case Key.NumPad8:
                    AddDigit('8');
                    e.Handled = true;
                    break;
                    
                case Key.D9:
                case Key.NumPad9:
                    AddDigit('9');
                    e.Handled = true;
                    break;
                    
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
                
                // Скрываем ошибку при изменении PIN
                var errorText = this.FindControl<TextBlock>("ErrorText");
                if (errorText != null)
                    errorText.IsVisible = false;
            }
        }

        private void UpdatePinDisplay()
        {
            var pinTextBox = this.FindControl<TextBox>("PinTextBox");
            if (pinTextBox != null)
            {
                pinTextBox.Text = new string('*', _pinCode.Length);
            }
        }

        // Обработчики для виртуальной клавиатуры
        private void NumberButton_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            AddDigit(button.Content.ToString()[0]);
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            _pinCode = "";
            UpdatePinDisplay();
            
            var errorText = this.FindControl<TextBlock>("ErrorText");
            if (errorText != null)
                errorText.IsVisible = false;
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

        // При активации контрола снова устанавливаем фокус
        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnAttachedToVisualTree(e);
            this.Focus();
        }
    }
}