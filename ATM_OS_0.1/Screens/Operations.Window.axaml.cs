using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using System;

namespace ATM_OS
{
    public partial class OperationsWindow : Window
    {
        private string _cardUID;
        private CardHolderRepository _repository;

        public OperationsWindow(string cardUID)
        {
            InitializeComponent();
            _cardUID = cardUID;
            _repository = new CardHolderRepository();
            LoadUserInfo();
        }

        private void LoadUserInfo()
        {
            var holder = _repository.GetCardHolderByUid(_cardUID);
            if (holder != null)
            {
                UserNameText.Text = $"Welcome, {holder.HolderName}";
                BalanceText.Text = $"Balance: {_repository.GetBalance(_cardUID)} {_repository.GetCurrency(_cardUID)}";
            }
        }

        private void DepositButton_Click(object sender, RoutedEventArgs e)
        {
            // Здесь можно реализовать диалог для пополнения
            ShowMessage("Deposit function - to be implemented");
        }

        private void WithdrawButton_Click(object sender, RoutedEventArgs e)
        {
            // Здесь можно реализовать диалог для снятия
            ShowMessage("Withdraw function - to be implemented");
        }

        private void BalanceButton_Click(object sender, RoutedEventArgs e)
        {
            LoadUserInfo(); // Обновляем баланс
            ShowMessage($"Current balance: {_repository.GetBalance(_cardUID)} {_repository.GetCurrency(_cardUID)}");
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            // Возвращаемся к главному меню
            var mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        private async void ShowMessage(string message)
        {
            var dialog = new Window
            {
                Title = "Information",
                Width = 300,
                Height = 150,
                Content = new StackPanel
                {
                    Children =
                    {
                        new TextBlock { Text = message, Margin = new Thickness(20), TextWrapping = Avalonia.Media.TextWrapping.Wrap },
                        new Button { Content = "OK", Margin = new Thickness(20), Width = 80, HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center }
                    }
                }
            };

            await dialog.ShowDialog(this);
        }
    }
}