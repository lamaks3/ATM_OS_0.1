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
        private AtmOperations _atmOperations;
        
        public enum OperationType
        {
            Deposit = 0,
            Withdraw = 1,
            PinChange = 2
        }
        
        public event Action OnExit;
        public event Action<string,OperationType> OnTransactionRequested; 
        public event Action<string> OnViewBalance; 
        public event Action<string> OnChangePin; 
        public event Action<string> OnDepositRequested; 
        public event Action<string> OnExchangeCurrency;

        public HomeView()
        {
            InitializeComponent();
        }

        public void Initialize(string cardUid)
        {
            _cardUid = cardUid;
            _atmOperations = new AtmOperations();
            LoadUserName();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void LoadUserName()
        {
            var userNameText = this.FindControl<TextBlock>("UserNameText");
            var userName = _atmOperations.GetUserName(_cardUid);
            userNameText.Text = $"Welcome, {userName}!";
        }

        private void DepositButton_Click(object sender, RoutedEventArgs e)
        {
            OnDepositRequested?.Invoke(_cardUid);
        }

        private void WithdrawButton_Click(object sender, RoutedEventArgs e)
        {
            OnTransactionRequested?.Invoke(_cardUid,OperationType.Withdraw);
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