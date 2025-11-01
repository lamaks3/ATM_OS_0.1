using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System;
using System.Threading.Tasks;

namespace ATM_OS
{
    public partial class BalanceView : UserControl
    {
        private string _cardUID;
        private CardHolderRepository _repository;

        // События для навигации
        public event Action OnBackToMainMenu;
        public event Action<string> OnPrintReceipt; // cardUID

        public BalanceView()
        {
            InitializeComponent();
        }

        public void Initialize(string cardUID)
        {
            _cardUID = cardUID;
            _repository = new CardHolderRepository();
            LoadBalanceInfo();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void LoadBalanceInfo()
        {
            var holder = _repository.GetCardHolderByUid(_cardUID);
            if (holder != null)
            {
                var holderNameText = this.FindControl<TextBlock>("HolderNameText");
                var accountNumberText = this.FindControl<TextBlock>("AccountNumberText");
                var balanceAmountText = this.FindControl<TextBlock>("BalanceAmountText");
                var currencyText = this.FindControl<TextBlock>("CurrencyText");
                var paymentSystemText = this.FindControl<TextBlock>("PaymentSystemText");
                var expiryDateText = this.FindControl<TextBlock>("ExpiryDateText");

                if (holderNameText != null)
                    holderNameText.Text = holder.HolderName;
                
                if (accountNumberText != null)
                    accountNumberText.Text = FormatAccountNumber(holder.NumberOfAccount);
                
                if (balanceAmountText != null)
                    balanceAmountText.Text = $"{_repository.GetBalance(_cardUID):N2}";
                
                if (currencyText != null)
                    currencyText.Text = holder.Currency;
                
                if (paymentSystemText != null)
                    paymentSystemText.Text = $"{holder.PaymentSystem} • Card UID: {holder.CardUid}";
                
                if (expiryDateText != null)
                    expiryDateText.Text = $"Valid thru: {holder.ExpireDate}";
            }
        }

        private string FormatAccountNumber(string accountNumber)
        {
            if (string.IsNullOrEmpty(accountNumber) || accountNumber.Length < 4)
                return accountNumber;
            
            // Форматируем номер счета для лучшего отображения
            return $"**** **** **** {accountNumber.Substring(accountNumber.Length - 4)}";
        }

        private void PrintReceiptButton_Click(object sender, RoutedEventArgs e)
        {
            ShowMessage("Receipt printing feature will be implemented soon");
            OnPrintReceipt?.Invoke(_cardUID);
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            OnBackToMainMenu?.Invoke();
        }

        private async void ShowMessage(string message)
        {
            var messageText = this.FindControl<TextBlock>("MessageText");
            if (messageText != null)
            {
                messageText.Text = message;
                messageText.IsVisible = true;
                
                // Автоматически скрываем сообщение через 3 секунды
                await Task.Delay(3000);
                messageText.IsVisible = false;
            }
        }

        // Метод для обновления баланса (если нужно обновить данные)
        public void RefreshBalance()
        {
            LoadBalanceInfo();
        }
    }
}