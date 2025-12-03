using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace ATM_OS
{
    public partial class TransactionView : UserControl
    {
        
        private string _cardUid;
        private CardHolderRepository _repository;
        private const int MaxAmount = 10000;
        private HomeView.OperationType _operationType;
        public event Action<string, HomeView.OperationType, int> OnAmountConfirmed;
        public event Action<string> OnBackToOperations;

        public TransactionView()
        {
            InitializeComponent();
        }

        public void Initialize(string cardUID, HomeView.OperationType operationType)
        {
            _cardUid = cardUID;
            _operationType = operationType;
            _repository = new CardHolderRepository();
            string title = "";

            if (HomeView.OperationType.Withdraw == _operationType)
            {
                title += "Withdraw";
            }else if (HomeView.OperationType.Deposit == _operationType)
            {
                title += "Deposit";
            }else if (HomeView.OperationType.PinChange == _operationType)
            {
                title += "PinChange";
            }
                
            
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
            
            if (amount > MaxAmount)
            {
                ShowError($"Maximum amount is {MaxAmount}");
                return;
            }
            
            if (_operationType == HomeView.OperationType.Withdraw)
            {
                double currentBalance = _repository.GetBalance(_cardUid);
                if (amount > currentBalance)
                {
                    ShowError("Insufficient funds");
                    return;
                }
            }
            
            OnAmountConfirmed?.Invoke(_cardUid, _operationType, amount);
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
            OnBackToOperations?.Invoke(_cardUid);
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