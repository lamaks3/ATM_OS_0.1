using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System;
using System.Threading.Tasks;

namespace ATM_OS
{
    public partial class MainMenuView : UserControl
    {
        private string _cardUid;
        private CardHolderRepository _repository;
        
        public enum OperationType
        {
            deposit,withdraw,pinChange
        }
        
        public event Action OnExit;
        public event Action<string, OperationType> OnTransactionRequested; 
        public event Action<string> OnViewBalance; 
        public event Action<string> OnChangePin; 

        public MainMenuView()
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
            OnTransactionRequested?.Invoke(_cardUid, OperationType.deposit);
        }

        private void WithdrawButton_Click(object sender, RoutedEventArgs e)
        {
            OnTransactionRequested?.Invoke(_cardUid, OperationType.withdraw);
        }
        
        private void ChangePinButton_Click(object sender, RoutedEventArgs e)
        {
            OnChangePin?.Invoke(_cardUid);
        }

        private void BalanceButton_Click(object sender, RoutedEventArgs e)
        {
            OnViewBalance?.Invoke(_cardUid);
        }
        
        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            OnExit?.Invoke();
        }
        
    }
}