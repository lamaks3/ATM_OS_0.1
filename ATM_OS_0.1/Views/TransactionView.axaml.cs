using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System;

namespace ATM_OS
{
    public partial class TransactionView : UserControl
    {
        private string _cardUID;
        private string _operationType;
        private CardHolderRepository _repository;
        private string _amount = "0";
        private const int MAX_AMOUNT = 10000; // Максимальная сумма

        // События
        public event Action<string, int> OnAmountConfirmed; // cardUID, amount
        public event Action OnBackToOperations;

        public TransactionView()
        {
            InitializeComponent();
            
            // Настраиваем обработку клавиатуры
            this.Focusable = true;
            this.AddHandler(KeyDownEvent, OnKeyDown, RoutingStrategies.Tunnel);
        }

        public void Initialize(string cardUID, string operationType, string title)
        {
            _cardUID = cardUID;
            _operationType = operationType;
            _repository = new CardHolderRepository();
            _amount = "0";
            
            var titleText = this.FindControl<TextBlock>("TitleText");
            if (titleText != null)
                titleText.Text = title;
            
            UpdateAmountDisplay();
            this.Focus();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            // Обработка цифр
            if (e.Key >= Key.D0 && e.Key <= Key.D9)
            {
                AddDigit((char)('0' + (e.Key - Key.D0)));
                e.Handled = true;
            }
            else if (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)
            {
                AddDigit((char)('0' + (e.Key - Key.NumPad0)));
                e.Handled = true;
            }
            else
            {
                switch (e.Key)
                {
                    case Key.Back:
                        RemoveLastDigit();
                        e.Handled = true;
                        break;
                        
                    case Key.Enter:
                        ConfirmAmount();
                        e.Handled = true;
                        break;
                        
                    case Key.Escape:
                        OnBackToOperations?.Invoke();
                        e.Handled = true;
                        break;
                }
            }
        }

        private void AddDigit(char digit)
        {
            // Проверяем, не превысит ли новая сумма максимальное значение
            string newAmount = _amount == "0" ? digit.ToString() : _amount + digit;
            
            if (int.TryParse(newAmount, out int amountValue))
            {
                if (amountValue <= MAX_AMOUNT)
                {
                    _amount = newAmount;
                    UpdateAmountDisplay();
                    
                    // Скрываем ошибку если она была показана
                    var errorText = this.FindControl<TextBlock>("ErrorText");
                    if (errorText != null)
                        errorText.IsVisible = false;
                }
                else
                {
                    ShowError($"Maximum amount is {MAX_AMOUNT:N0}");
                }
            }
        }

        private void RemoveLastDigit()
        {
            if (_amount.Length > 1)
                _amount = _amount.Substring(0, _amount.Length - 1);
            else
                _amount = "0";
            
            UpdateAmountDisplay();
            
            // Скрываем ошибку при изменении суммы
            var errorText = this.FindControl<TextBlock>("ErrorText");
            if (errorText != null)
                errorText.IsVisible = false;
        }

        private void UpdateAmountDisplay()
        {
            var amountText = this.FindControl<TextBlock>("AmountText");
            if (amountText != null)
            {
                if (int.TryParse(_amount, out int amount))
                {
                    amountText.Text = $"{amount:N0}";
                }
                else
                {
                    amountText.Text = "0";
                }
            }
        }

        // Обработчики виртуальной клавиатуры
        private void NumberButton_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            AddDigit(button.Content.ToString()[0]);
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            _amount = "0";
            UpdateAmountDisplay();
            
            var errorText = this.FindControl<TextBlock>("ErrorText");
            if (errorText != null)
                errorText.IsVisible = false;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            ConfirmAmount();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            OnBackToOperations?.Invoke();
        }

        private void ConfirmAmount()
        {
            if (int.TryParse(_amount, out int amount) && amount > 0)
            {
                // Проверяем максимальную сумму
                if (amount > MAX_AMOUNT)
                {
                    ShowError($"Maximum transaction amount is {MAX_AMOUNT:N0}");
                    return;
                }

                // Проверяем достаточно ли средств для снятия
                if (_operationType == "Withdraw")
                {
                    double currentBalance = _repository.GetBalance(_cardUID);
                    if (amount > currentBalance)
                    {
                        ShowError($"Insufficient funds. Available: {currentBalance:N0}");
                        return;
                    }
                }
                
                OnAmountConfirmed?.Invoke(_cardUID, amount);
            }
            else
            {
                ShowError("Please enter a valid amount");
            }
        }

        private void ShowError(string message)
        {
            var errorText = this.FindControl<TextBlock>("ErrorText");
            if (errorText != null)
            {
                errorText.Text = message;
                errorText.IsVisible = true;
            }
        }

        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnAttachedToVisualTree(e);
            this.Focus();
        }
    }
}