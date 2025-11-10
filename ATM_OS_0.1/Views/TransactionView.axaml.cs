using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System;

namespace ATM_OS
{
    public partial class TransactionView : UserControl
    {
        private string _cardUID;
        private string _operationType;
        private CardHolderRepository _repository;
        private string _amount = "0";
        private const int MAX_AMOUNT = 10000;
        
        public event Action<string, int> OnAmountConfirmed;
        public event Action OnBackToOperations;

        public TransactionView()
        {
            InitializeComponent();
            
            this.Focusable = true;
            this.AddHandler(KeyDownEvent, OnKeyDown, RoutingStrategies.Tunnel);
        }

        public void Initialize(string cardUID, string operationType, string title)
        {
            _cardUID = cardUID;
            _operationType = operationType;
            _repository = new CardHolderRepository();
            
            var titleText = this.FindControl<TextBlock>("TitleText");
            titleText.Text = title;
            
            UpdateAmountDisplay();
            this.Focus();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key >= Key.D0 && e.Key <= Key.D9)
            {
                AddDigit((char)('0' + (e.Key - Key.D0)));
                e.Handled = true;
            }
            else
            {
                switch (e.Key)
                {
                    case Key.Back:
                        RemoveLastDigit();
                        e.Handled = true;
                        break;
                        
                    case Key.Enter:
                        ConfirmAmount();
                        e.Handled = true;
                        break;
                }
            }
        }

        private void AddDigit(char digit)
        {
            string newAmount = _amount == "0" ? digit.ToString() : _amount + digit;
            int amountValue = int.Parse(newAmount);
            
            if (amountValue <= MAX_AMOUNT)
            {
                _amount = newAmount;
                UpdateAmountDisplay();
            }
            else
            {
                ShowError($"Maximum amount is {MAX_AMOUNT}");
            }
        }

        private void RemoveLastDigit()
        {
            if (_amount.Length > 1)
                _amount = _amount.Substring(0, _amount.Length - 1);
            else
                _amount = "0";
            
            UpdateAmountDisplay();
        }

        private void UpdateAmountDisplay()
        {
            var amountText = this.FindControl<TextBlock>("AmountText");
            amountText.Text = $"{int.Parse(_amount)}";
        }
        
        private void NumberButton_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            AddDigit(button.Content.ToString()[0]);
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            _amount = "0";
            UpdateAmountDisplay();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            ConfirmAmount();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            OnBackToOperations?.Invoke();
        }

        private void ConfirmAmount()
        {
            int amount = int.Parse(_amount);
            
            if (_operationType == "Withdraw")
            {
                double currentBalance = _repository.GetBalance(_cardUID);
                if (amount > currentBalance)
                {
                    ShowError($"Insufficient funds. Available: {currentBalance}");
                    return;
                }
            }
            
            OnAmountConfirmed?.Invoke(_cardUID, amount);
        }

        private void ShowError(string message)
        {
            var errorText = this.FindControl<TextBlock>("ErrorText");
            errorText.Text = message;
            errorText.IsVisible = true;
        }
        
    }
}