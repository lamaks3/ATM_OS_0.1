using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System.Collections.Generic;

namespace ATM_OS
{
    public partial class DepositView : UserControl
    {
        private string _cardUid;
        private string _currency;
        private AtmOperations _atmOperations;
        private int _totalDepositAmount = 0;
        private Dictionary<int, int> _depositedBanknotes = new Dictionary<int, int>();
        
        public event Action<string, HomeView.OperationType, string, string> OnAmountConfirmed;
        public event Action<string> OnBackToOperations;

        public DepositView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public void Initialize(string cardUid)
        {
            _cardUid = cardUid;
            _atmOperations = new AtmOperations();
            _currency = _atmOperations.GetCurrency(cardUid);
            
            _depositedBanknotes[1] = 0;
            _depositedBanknotes[5] = 0;
            _depositedBanknotes[10] = 0;
            _depositedBanknotes[20] = 0;
            _depositedBanknotes[50] = 0;
            _depositedBanknotes[100] = 0;
            _depositedBanknotes[200] = 0;
            _depositedBanknotes[500] = 0;
    
            UpdateUI();
            
            var btn1 = this.FindControl<Button>("Btn1");
            var count1Text = this.FindControl<TextBlock>("Count1Text");
            var btn200 = this.FindControl<Button>("Btn200");
            var count200Text = this.FindControl<TextBlock>("Count200Text");
            var btn500 = this.FindControl<Button>("Btn500");
            var count500Text = this.FindControl<TextBlock>("Count500Text");
    
            if (_currency == "USD")
            {
                btn1.IsVisible = true;
                count1Text.IsVisible = true;
            }
            if (_currency == "EUR")
            {
                btn200.IsVisible = true;
                count200Text.IsVisible = true;
            }
            else if (_currency == "BYN")
            {
                btn200.IsVisible = true;
                count200Text.IsVisible = true;
                btn500.IsVisible = true;
                count500Text.IsVisible = true;
            }
        }

        private void UpdateUI()
        {
            var totalText = this.FindControl<TextBlock>("TotalText");
            totalText.Text = $"{_totalDepositAmount} {_currency}";
            
            UpdateBanknoteCounters();
        }

        private void UpdateBanknoteCounters()
        {
            var count1Text = this.FindControl<TextBlock>("Count1Text");
            var count5Text = this.FindControl<TextBlock>("Count5Text");
            var count10Text = this.FindControl<TextBlock>("Count10Text");
            var count20Text = this.FindControl<TextBlock>("Count20Text");
            var count50Text = this.FindControl<TextBlock>("Count50Text");
            var count100Text = this.FindControl<TextBlock>("Count100Text");
            var count200Text = this.FindControl<TextBlock>("Count200Text");
            var count500Text = this.FindControl<TextBlock>("Count500Text");
            
            count1Text.Text = $"×{_depositedBanknotes[1]}";
            count5Text.Text = $"×{_depositedBanknotes[5]}";
            count10Text.Text = $"×{_depositedBanknotes[10]}";
            count20Text.Text = $"×{_depositedBanknotes[20]}";
            count50Text.Text = $"×{_depositedBanknotes[50]}";
            count100Text.Text = $"×{_depositedBanknotes[100]}";
            count200Text.Text = $"×{_depositedBanknotes[200]}";
            count500Text.Text = $"×{_depositedBanknotes[500]}";
        }

        private void AmountButton_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            if (int.TryParse(button.Tag?.ToString(), out int denomination))
            {
                AddBanknote(denomination);
            }
        }

        private void AddBanknote(int denomination)
        {
            _depositedBanknotes[denomination] += 1;
            _totalDepositAmount += denomination;
            UpdateUI();
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            if (_totalDepositAmount == 0)
                return;
            
            CashHandler.Currency currencyEnum = _currency switch
            {
                "BYN" => CashHandler.Currency.BYN,
                "USD" => CashHandler.Currency.USD,
                "EUR" => CashHandler.Currency.EUR,
                _ => CashHandler.Currency.BYN
            };
            
            foreach (var kvp in _depositedBanknotes)
            {
                if (kvp.Value > 0)
                {
                    CashHandler.AddBanknotes(currencyEnum, kvp.Key, kvp.Value);
                }
            }
            
            _atmOperations.TryProceedTransaction(_cardUid, _totalDepositAmount, HomeView.OperationType.Deposit);

            string totalDeposit = _totalDepositAmount.ToString();
            
            OnAmountConfirmed?.Invoke(_cardUid, HomeView.OperationType.Deposit, totalDeposit, _currency);
            
            ResetDeposit();
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            ResetDeposit();
        }

        private void ResetDeposit()
        {
            _totalDepositAmount = 0;
            _depositedBanknotes[1] = 0;
            _depositedBanknotes[5] = 0;
            _depositedBanknotes[10] = 0;
            _depositedBanknotes[20] = 0;
            _depositedBanknotes[50] = 0;
            _depositedBanknotes[100] = 0;
            _depositedBanknotes[200] = 0;
            _depositedBanknotes[500] = 0;
            UpdateUI();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            OnBackToOperations?.Invoke(_cardUid);
        }
    }
}