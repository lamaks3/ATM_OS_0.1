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

        
        public event Action OnBackToMainMenu;

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

                holderNameText.Text = holder.HolderName;
                
                accountNumberText.Text = FormatAccountNumber(holder.NumberOfAccount);
                
                balanceAmountText.Text = $"{_repository.GetBalance(_cardUID):N2}";
                
                currencyText.Text = holder.Currency;
                
                paymentSystemText.Text = $"{holder.PaymentSystem} • Card UID: {holder.CardUid}";
                
                expiryDateText.Text = $"Valid thru: {holder.ExpireDate}";
            }
        }

        private string FormatAccountNumber(string accountNumber)
        {
            if (string.IsNullOrEmpty(accountNumber) || accountNumber.Length < 4)
                return accountNumber;
            
            return $"**** **** **** {accountNumber.Substring(accountNumber.Length - 4)}";
        }
        
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            OnBackToMainMenu?.Invoke();
        }
    }
}