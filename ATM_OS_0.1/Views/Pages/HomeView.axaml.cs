using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System;
using System.Threading.Tasks;

namespace ATM_OS
{
    public partial class HomeView : UserControl
    {
        private string _cardUid;
        private CardHolderRepository _repository;
        
        public enum OperationType
        {
            Deposit = 0,
            Withdraw = 2,
            PinChange = 3
        }
        
        public event Action OnExit;
        public event Action<string, OperationType> OnTransactionRequested; 
        public event Action<string> OnViewBalance; 
        public event Action<string> OnChangePin; 
        public event Action<string> OnExchangeCurrency;

        public HomeView()
        {
            InitializeComponent();
        }

        public void Initialize(string cardUID)
        {
            _cardUid = cardUID;
            _repository = new CardHolderRepository();
            LoadUserInfo();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void LoadUserInfo()
        {
            var userNameText = this.FindControl<TextBlock>("UserNameText");
            var holder = _repository.GetCardHolderByUid(_cardUid);
            userNameText.Text = $"Welcome, {holder.HolderName.Split(" ")[0]}";
        }

        private void DepositButton_Click(object sender, RoutedEventArgs e)
        {
            OnTransactionRequested?.Invoke(_cardUid, OperationType.Deposit);
        }

        private void WithdrawButton_Click(object sender, RoutedEventArgs e)
        {
            OnTransactionRequested?.Invoke(_cardUid, OperationType.Withdraw);
        }
        
        private void ChangePinButton_Click(object sender, RoutedEventArgs e)
        {
            OnChangePin?.Invoke(_cardUid);
        }

        private void BalanceButton_Click(object sender, RoutedEventArgs e)
        {
            OnViewBalance?.Invoke(_cardUid);
        }
        
        private void ExchangeCurrencyButton_Click(object sender, RoutedEventArgs e)
        {
            OnExchangeCurrency?.Invoke(_cardUid);
        }
        
        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            OnExit?.Invoke();
        }
        
    }
}