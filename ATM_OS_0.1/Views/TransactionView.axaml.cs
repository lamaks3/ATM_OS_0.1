using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace ATM_OS
{
    public partial class TransactionView : UserControl
    {
        private string _cardUID;
        private string _operationType;
        private CardHolderRepository _repository;
        private const int MAX_AMOUNT = 10000;
        
        public event Action<string, int> OnAmountConfirmed;
        public event Action OnBackToOperations;

        public TransactionView()
        {
            InitializeComponent();
        }

        public void Initialize(string cardUID, string operationType, string title)
        {
            _cardUID = cardUID;
            _operationType = operationType;
            _repository = new CardHolderRepository();
            
            var titleText = this.FindControl<TextBlock>("TitleText");
            titleText.Text = title;
            
            var keyboard = this.FindControl<NumericKeyboard>("Keyboard");
            keyboard.Reset();
            keyboard.SetMaxLength(5); 
            
            ClearError();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void Keyboard_OnValueChanged(string value)
        {
            // Теперь здесь только очистка ошибки, отображение в клавиатуре
            ClearError();
        }

        private void Keyboard_OnValueConfirmed(string value)
        {
            int amount = int.Parse(value);
            
            if (amount == 0)
            {
                ShowError("Amount cannot be zero");
                return;
            }
            
            if (amount > MAX_AMOUNT)
            {
                ShowError($"Maximum amount is {MAX_AMOUNT}");
                return;
            }
            
            if (_operationType == "Withdraw")
            {
                double currentBalance = _repository.GetBalance(_cardUID);
                if (amount > currentBalance)
                {
                    ShowError("Insufficient funds");
                    return;
                }
            }
            
            OnAmountConfirmed?.Invoke(_cardUID, amount);
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
            OnBackToOperations?.Invoke();
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