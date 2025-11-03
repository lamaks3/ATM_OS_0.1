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
        private string _cardUID;
        private CardHolderRepository _repository;

        // События для навигации
        public event Action OnExit;
        public event Action<string, string> OnTransactionRequested; // cardUID, operationType
        public event Action<string> OnViewBalance; // cardUID

        public MainMenuView()
        {
            InitializeComponent();
        }

        public void Initialize(string cardUID)
        {
            _cardUID = cardUID;
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
            
            var holder = _repository.GetCardHolderByUid(_cardUID);
            if (holder != null)
            {
                userNameText.Text = $"Welcome, {holder.HolderName.Split(" ")[0]}";
            }
        }

        private void DepositButton_Click(object sender, RoutedEventArgs e)
        {
            OnTransactionRequested?.Invoke(_cardUID, "Deposit");
        }

        private void WithdrawButton_Click(object sender, RoutedEventArgs e)
        {
            OnTransactionRequested?.Invoke(_cardUID, "Withdraw");
        }

        private void BalanceButton_Click(object sender, RoutedEventArgs e)
        {
            OnViewBalance?.Invoke(_cardUID);
        }
        
        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            OnExit?.Invoke();
        }
        
    }
}