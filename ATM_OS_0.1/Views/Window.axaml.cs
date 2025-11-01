using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System;
using System.Threading.Tasks;
using Avalonia.Threading;

namespace ATM_OS
{
    public partial class Window : Avalonia.Controls.Window
    {
        private StartView _startView;
        private PinView _pinView;
        private MainMenuView _mainMenuView;
        private TransactionView _transactionView;
        private BalanceView _balanceView;

        // Добавляем поле для ContentControl
        private ContentControl _mainContent;

        public Window()
        {
            InitializeComponent();
            
            // Находим MainContent после инициализации компонентов
            _mainContent = this.FindControl<ContentControl>("MainContent");
            
            // Используем событие Loaded для инициализации после полной загрузки окна
            this.Loaded += OnWindowLoaded;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void OnWindowLoaded(object sender, EventArgs e)
        {
            // Окно полностью загружено
            InitializeNfcListener();
            ShowStartView();
        }

        private void InitializeNfcListener()
        {
            Task.Run(async () => await CheckForCards());
        }

        private async Task CheckForCards()
        {
            while (true)
            {
                string cardUID = NfcScannerService.GetCardUID();
                
                if (!string.IsNullOrEmpty(cardUID))
                {
                    var repository = new CardHolderRepository();
                    if (repository.CardExists(cardUID))
                    {
                        await Dispatcher.UIThread.InvokeAsync(() =>
                        {
                            ShowPinView(cardUID);
                        });
                    }
                    
                    NfcScannerService.SetCardUID(string.Empty);
                }
                
                await Task.Delay(500);
            }
        }

        private void ShowStartView()
        {
            // Проверяем что _mainContent инициализирован
            if (_mainContent == null)
            {
                _mainContent = this.FindControl<ContentControl>("MainContent");
                if (_mainContent == null)
                {
                    // Если все еще null, создаем временное решение
                    _mainContent = new ContentControl();
                    this.Content = _mainContent;
                }
            }

            _startView = new StartView();
            _mainContent.Content = _startView;
        }

        private void ShowPinView(string cardUID)
        {
            if (_mainContent == null) return;

            _pinView = new PinView();
            _pinView.Initialize(cardUID);
            
            _pinView.OnPinVerified += (uid) => ShowMainMenuView(uid);
            _pinView.OnBackToMain += ShowStartView;
            
            _mainContent.Content = _pinView;
        }

        private void ShowMainMenuView(string cardUID)
        {
            if (_mainContent == null) return;

            _mainMenuView = new MainMenuView();
            _mainMenuView.Initialize(cardUID);
            
            _mainMenuView.OnExit += ShowStartView;
            _mainMenuView.OnTransactionRequested += (uid, operationType) => ShowTransactionView(uid, operationType);
            _mainMenuView.OnViewBalance += (uid) => ShowBalanceView(uid);
            
            _mainContent.Content = _mainMenuView;
        }

        private void ShowTransactionView(string cardUID, string operationType)
        {
            if (_mainContent == null) return;

            _transactionView = new TransactionView();
            
            string title = operationType == "Deposit" ? "Enter deposit amount" : "Enter withdrawal amount";
            _transactionView.Initialize(cardUID, operationType, title);
            
            _transactionView.OnAmountConfirmed += (uid, amount) => ProcessTransaction(uid, operationType, amount);
            _transactionView.OnBackToOperations += () => ShowMainMenuView(cardUID);
            
            _mainContent.Content = _transactionView;
        }

        private void ShowBalanceView(string cardUID)
        {
            if (_mainContent == null) return;

            _balanceView = new BalanceView();
            _balanceView.Initialize(cardUID);
            
            _balanceView.OnBackToMainMenu += () => ShowMainMenuView(cardUID);
            _balanceView.OnPrintReceipt += (uid) => PrintReceipt(uid);
            
            _mainContent.Content = _balanceView;
        }

        private void ProcessTransaction(string cardUID, string operationType, int amount)
        {
            var repository = new CardHolderRepository();
            bool success = false;
            string message = "";

            if (operationType == "Deposit")
            {
                success = repository.AddToBalance(cardUID, amount);
                message = success ? 
                    $"Successfully deposited {amount:N0}. New balance: {repository.GetBalance(cardUID):N2}" :
                    "Deposit failed";
            }
            else if (operationType == "Withdraw")
            {
                success = repository.AddToBalance(cardUID, -amount);
                message = success ? 
                    $"Successfully withdrawn {amount:N0}. New balance: {repository.GetBalance(cardUID):N2}" :
                    "Withdrawal failed";
            }

            // Показываем сообщение и возвращаемся к операциям
            ShowMessage(message, () => ShowMainMenuView(cardUID));
        }

        private void PrintReceipt(string cardUID)
        {
            // Здесь будет логика печати чека
            ShowMessage("Receipt printed successfully", () => ShowBalanceView(cardUID));
        }

        private async void ShowMessage(string message, Action callback)
        {
            var dialog = new Avalonia.Controls.Window
            {
                Title = "Transaction Result",
                Width = 400,
                Height = 200,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                CanResize = false
            };

            var stackPanel = new StackPanel
            {
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                Spacing = 20
            };

            stackPanel.Children.Add(new TextBlock { 
                Text = message, 
                Margin = new Thickness(20),
                TextWrapping = Avalonia.Media.TextWrapping.Wrap,
                TextAlignment = Avalonia.Media.TextAlignment.Center,
                FontSize = 16
            });

            var okButton = new Button { 
                Content = "OK", 
                Width = 100,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center
            };
            okButton.Click += (s, e) => {
                dialog.Close();
                callback?.Invoke();
            };
            
            stackPanel.Children.Add(okButton);
            dialog.Content = stackPanel;

            await dialog.ShowDialog(this);
        }
    }
}