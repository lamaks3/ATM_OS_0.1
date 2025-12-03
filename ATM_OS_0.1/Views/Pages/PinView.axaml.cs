using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace ATM_OS
{
    public partial class PinView : UserControl
    {
        private string _cardUID;
        private CardHolderRepository _repository;
        private const int PIN_LENGTH = 4;
        private int pinAttemps = 1;
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
            
            var titleText = this.FindControl<TextBlock>("TitleText");
            titleText.Text = "Enter PIN code";
            
            var keyboard = this.FindControl<NumericKeyboard>("Keyboard");
            keyboard.Reset();
            keyboard.SetMaxLength(PIN_LENGTH); 
            keyboard.enablePinMode();
            
            ClearError();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void Keyboard_OnValueChanged(string value)
        {
            ClearError();
        }

        private void Keyboard_OnValueConfirmed(string value)
        {
            // Проверяем PIN код
            if (string.IsNullOrEmpty(value) || value.Length != PIN_LENGTH)
            {
                ShowError("PIN code must be exactly 4 digits");
                return;
            }

            if (_repository.VerifyPin(_cardUID, value))
            {
                OnPinVerified?.Invoke(_cardUID);
            }
            else
            {
                var keyboard = this.FindControl<NumericKeyboard>("Keyboard");
                keyboard.Clean();
                if (pinAttemps == 3) OnBackToMain?.Invoke();
                ShowError("Incorrect PIN code (" + pinAttemps + "/3)");
                pinAttemps += 1;
            }
        }

        private void Keyboard_OnClearPressed()
        {
            ClearError();
        }

        private void Keyboard_OnBackPressed()
        {
            ClearError();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            OnBackToMain?.Invoke();
        }

        private void ShowError(string message)
        {
            var errorText = this.FindControl<TextBlock>("ErrorText");
            errorText.Text = message;
        }

        private void ClearError()
        {
            var errorText = this.FindControl<TextBlock>("ErrorText");
            errorText.Text = "";
        }
    }
}