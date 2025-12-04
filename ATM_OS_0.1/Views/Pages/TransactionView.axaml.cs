using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace ATM_OS
{
    public partial class TransactionView : KeyboardViewBase
    {
        
        private const int MaxAmount = 10000;
        private HomeView.OperationType _operationType;
        public event Action<string, HomeView.OperationType, int, string> OnAmountConfirmed;
        public event Action<string> OnBackToOperations;

        public TransactionView()
        {
            InitializeComponent();
        }

        public void Initialize(string cardUid, HomeView.OperationType operationType)
        {
            _operationType = operationType;
            string title = "";
            
            if (HomeView.OperationType.Withdraw == _operationType)
            {
                title = "Withdraw";
            }else if (HomeView.OperationType.Deposit == _operationType)
            {
                title = "Deposit";
            }else if (HomeView.OperationType.PinChange == _operationType)
            {
                title = "PinChange";
            }

            CommonInitialize(cardUid, title, 5);
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
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
                if (!_atmService.TryProceedTransaction(_cardUid, amount,_operationType))
                {
                    ShowError("Insufficient funds");
                    return;
                }
            }
            else
            {
                if (!_atmService.TryProceedTransaction(_cardUid, amount,_operationType))
                {
                    ShowError("Uknown error");
                };
            }
            string currency = _atmService.GetCurrency(_cardUid);
            OnAmountConfirmed?.Invoke(_cardUid, _operationType, amount, currency );
        }
        
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            OnBackToOperations?.Invoke(_cardUid);
        }
    }
}