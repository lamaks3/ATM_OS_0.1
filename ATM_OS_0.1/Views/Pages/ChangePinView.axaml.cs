using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace ATM_OS
{
    public partial class ChangePinView : UserControl
    {
        private string _cardUID;
        private CardHolderRepository _repository;
        private const int PIN_LENGTH = 4;
        private string tempPin;
        
        public event Action OnBackToMain;
        public event Action OnShowPartingView;

        public ChangePinView()
        {
            InitializeComponent();
        }

        public void Initialize(string cardUID)
        {
            _cardUID = cardUID;
            _repository = new CardHolderRepository();
            
            var titleText = this.FindControl<TextBlock>("TitleText");
            titleText.Text = "Enter new PIN code";
            
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
            if (value?.Length != PIN_LENGTH)
            {
                ShowError("PIN must be 4 digits");
                return;
            }

            if (tempPin == null)
            {
                tempPin = value;
                this.FindControl<TextBlock>("TitleText").Text = "Confirm PIN code";
                this.FindControl<NumericKeyboard>("Keyboard").Reset();
                ClearError();
            }
            else
            {
                if (tempPin == value)
                {
                    _repository.ChangePin(_cardUID, value);
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