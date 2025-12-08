using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using ATMProject;

namespace ATM_OS
{
    public partial class TransactionView : KeyboardViewBase
    {
        private const int MaxAmount = AtmConfiguration.MaxTransactionAmount;
        private HomeView.OperationType _operationType;
        private string _currency;
        
        public event Action<string, HomeView.OperationType, int, string> OnAmountConfirmed;
        public event Action<string> OnBackToOperations;

        public TransactionView()
        {
            InitializeComponent();
        }

        public void Initialize(string cardUid, HomeView.OperationType operationType)
        {
            _operationType = operationType;
            _currency = new AtmOperations().GetCurrency(cardUid);
            
            string title = _operationType == HomeView.OperationType.Withdraw ? "Withdraw" : "Deposit";
            CommonInitialize(cardUid, title);
            
            if (_operationType == HomeView.OperationType.Withdraw)
            {
                UpdateAvailableBanknotesDisplay();
            }
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void UpdateAvailableBanknotesDisplay()
        {
            var banknotesText = this.FindControl<TextBlock>("BanknotesText");
            if (banknotesText == null) return;
            
            CashHandler.Currency currencyEnum = _currency switch
            {
                "BYN" => CashHandler.Currency.BYN,
                "USD" => CashHandler.Currency.USD,
                "EUR" => CashHandler.Currency.EUR,
                _ => CashHandler.Currency.BYN
            };
            
            var banknotes = CashHandler.GetBanknotes(currencyEnum);
            
            var banknoteStrings = new List<string>();
            banknoteStrings.Add("Available banknotes");
            
            foreach (var kvp in banknotes.OrderByDescending(x => x.Key))
            {
                if (kvp.Value > 0)
                {
                    banknoteStrings.Add($"{kvp.Key} {_currency}×{kvp.Value}");
                }
            }
            
            if (banknoteStrings.Count == 1)
            {
                banknotesText.Text = "No banknotes available";
            }
            else
            {
                banknotesText.Text = string.Join(" | ", banknoteStrings);
            }
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
                CashHandler.Currency currencyEnum = _currency switch
                {
                    "BYN" => CashHandler.Currency.BYN,
                    "USD" => CashHandler.Currency.USD,
                    "EUR" => CashHandler.Currency.EUR,
                };
                
                try
                {
                    var banknotesToWithdraw = CashHandler.WithdrawAmountGreedy(currencyEnum, amount);
                    
                    if (!new AtmOperations().TryProceedTransaction(_cardUid, amount, _operationType))
                    {
                        foreach (var kvp in banknotesToWithdraw)
                        {
                            CashHandler.AddBanknotes(currencyEnum, kvp.Key, kvp.Value);
                        }
                        ShowError("Insufficient funds");
                        return;
                    }
                    
                    UpdateAvailableBanknotesDisplay();
                }
                catch
                {
                    ShowError("Cannot withdraw this amount with available banknotes");
                    return;
                }
            }
            else
            {
                if (!new AtmOperations().TryProceedTransaction(_cardUid, amount, _operationType))
                {
                    ShowError("Transaction failed");
                    return;
                }
            }
            
            OnAmountConfirmed?.Invoke(_cardUid, _operationType, amount, _currency);
        }
        
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            OnBackToOperations?.Invoke(_cardUid);
        }
    }
}