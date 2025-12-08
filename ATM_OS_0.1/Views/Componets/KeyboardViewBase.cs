using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace ATM_OS
{
    public abstract class KeyboardViewBase : UserControl
    {
        protected string _cardUid;
        protected AtmOperations AtmOperations;
        
        protected void CommonInitialize(string cardUid, string title, bool enablePinMode = false)
        {
            _cardUid = cardUid;
            AtmOperations = new AtmOperations();
            
            var titleText = this.FindControl<TextBlock>("TitleText");
            titleText.Text = title;
            
            var keyboard = this.FindControl<NumericKeyboard>("Keyboard");
            keyboard.Reset();
            keyboard.SetMaxLength(enablePinMode ? AtmConfiguration.PinLength : AtmConfiguration.TransactionInputLength);
            
            if (enablePinMode)
            {
                keyboard.enablePinMode();
            }
            
            ClearError();
        }

        protected void Keyboard_OnValueChanged(string value)
        {
            ClearError();
        }
        
        protected void Keyboard_OnClearPressed()
        {
            ClearError();
        }

        protected void Keyboard_OnBackPressed()
        {
            ClearError();
        }
        
        protected void ShowError(string message)
        {
            var errorText = this.FindControl<TextBlock>("ErrorText");
            errorText.Text = message;
        }

        protected void ClearError()
        {
            var errorText = this.FindControl<TextBlock>("ErrorText");
            errorText.Text = "";
        }
    }
}