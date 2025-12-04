using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace ATM_OS
{
    public partial class ChangePinView : KeyboardViewBase
    {
        private const int PinLength = 4;
        private string _tempPin;
        
        public event Action<string> OnBackToMain;
        public event Action OnShowPartingView;

        public ChangePinView()
        {
            InitializeComponent();
        }

        public void Initialize(string cardUid)
        {
            CommonInitialize(cardUid, "Pin change", PinLength, true);
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
        
        private void Keyboard_OnValueConfirmed(string value)
        {
            if (value?.Length != PinLength)
            {
                ShowError("PIN must be 4 digits");
                return;
            }

            if (_tempPin == null)
            {
                _tempPin = value;
                var titleText = this.FindControl<TextBlock>("TitleText");
                titleText.Text = "Change Pin";
                var keyboard = this.FindControl<NumericKeyboard>("Keyboard");
                keyboard.Reset();
                ClearError();
            }
            else
            {
                if (_tempPin == value)
                {
                    _atmService.ChangePin(_cardUid, value);
                    OnShowPartingView?.Invoke();
                }
                else
                {
                    var keyboard = this.FindControl<NumericKeyboard>("Keyboard");
                    keyboard.Clean();
                    ShowError("PIN codes don't match. Try again");
                }
            }
        }
        
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            OnBackToMain?.Invoke(_cardUid);
        }
    }
}