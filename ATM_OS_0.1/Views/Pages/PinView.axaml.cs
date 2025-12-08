using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace ATM_OS
{
    public partial class PinView : KeyboardViewBase
    {
        private int pinAttemps = 1;
        public event Action<string> OnPinVerified;
        public event Action OnBackToMain;

        public PinView()
        {
            InitializeComponent();
        }

        public void Initialize(string cardUID)
        {
            CommonInitialize(cardUID, "Enter PinCode", true);
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
        
        private void Keyboard_OnValueConfirmed(string value)
        {
            if (string.IsNullOrEmpty(value) || value.Length != AtmConfiguration.PinLength)
            {
                ShowError("PIN code must be exactly 4 digits");
                return;
            }

            if (AtmOperations.VerifyPin(_cardUid, value))
            {
                OnPinVerified?.Invoke(_cardUid);
            }
            else
            {
                var keyboard = this.FindControl<NumericKeyboard>("Keyboard");
                keyboard.Clean();
                if (pinAttemps == AtmConfiguration.MaxPinAttempts) OnBackToMain?.Invoke();
                ShowError("Incorrect PIN code (" + pinAttemps + "/3)");
                pinAttemps += 1;
            }
        }
        
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            OnBackToMain?.Invoke();
        }
    }
}