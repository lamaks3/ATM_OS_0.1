using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System;

namespace ATM_OS
{
    public partial class OperationsView : UserControl
    {
        private string _cardUID;
        private CardHolderRepository _repository;

        // Событие для навигации
        public event Action OnExit;

        public OperationsView()
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
            var balanceText = this.FindControl<TextBlock>("BalanceText");
            
            var holder = _repository.GetCardHolderByUid(_cardUID);
            if (holder != null)
            {
                userNameText.Text = $"Welcome, {holder.HolderName}";
                balanceText.Text = $"Balance: {_repository.GetBalance(_cardUID)} {_repository.GetCurrency(_cardUID)}";
            }
        }

        private void DepositButton_Click(object sender, RoutedEventArgs e)
        {
            ShowMessage("Deposit function - to be implemented");
        }

        private void WithdrawButton_Click(object sender, RoutedEventArgs e)
        {
            ShowMessage("Withdraw function - to be implemented");
        }

        private void BalanceButton_Click(object sender, RoutedEventArgs e)
        {
            LoadUserInfo();
            ShowMessage($"Current balance: {_repository.GetBalance(_cardUID)} {_repository.GetCurrency(_cardUID)}");
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            OnExit?.Invoke();
        }

        private async void ShowMessage(string message)
        {
            var dialog = new Window
            {
                Title = "Information",
                Width = 300,
                Height = 150,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            var stackPanel = new StackPanel();
            stackPanel.Children.Add(new TextBlock { 
                Text = message, 
                Margin = new Thickness(20), 
                TextWrapping = Avalonia.Media.TextWrapping.Wrap 
            });

            var okButton = new Button { 
                Content = "OK", 
                Margin = new Thickness(20), 
                Width = 80, 
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center 
            };
            okButton.Click += (s, e) => dialog.Close();
            
            stackPanel.Children.Add(okButton);
            dialog.Content = stackPanel;

            await dialog.ShowDialog((Window)this.VisualRoot);
        }
    }
}