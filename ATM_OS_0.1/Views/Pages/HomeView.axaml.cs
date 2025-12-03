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
        public event Action<OperationType> OnTransactionRequested; 
        public event Action OnViewBalance; 
        public event Action OnChangePin; 
        public event Action OnExchangeCurrency;

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
            var userName = _repository.GetUserName(_cardUid);
            userNameText.Text = $"Welcome, {userName}!";
        }

        private void DepositButton_Click(object sender, RoutedEventArgs e)
        {
            OnTransactionRequested?.Invoke(OperationType.Deposit);
        }

        private void WithdrawButton_Click(object sender, RoutedEventArgs e)
        {
            OnTransactionRequested?.Invoke(OperationType.Withdraw);
        }
        
        private void ChangePinButton_Click(object sender, RoutedEventArgs e)
        {
            OnChangePin?.Invoke();
        }

        private void BalanceButton_Click(object sender, RoutedEventArgs e)
        {
            OnViewBalance?.Invoke();
        }
        
        private void ExchangeCurrencyButton_Click(object sender, RoutedEventArgs e)
        {
            OnExchangeCurrency?.Invoke();
        }
        
        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            OnExit?.Invoke();
        }
        
    }
}