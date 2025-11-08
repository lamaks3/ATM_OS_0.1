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
        private ContinueOperationView _continueOperationView;
        private PartingView _partingView;

        // Добавляем поле для ContentControl
        private ContentControl _mainContent;
        private string _currentCardUID;
        private string _currentOperationType;
        private bool _nfcPaused = false; 

        public Window()
        {
            InitializeComponent();
            
            _mainContent = this.FindControl<ContentControl>("MainContent");
            
            this.Loaded += OnWindowLoaded;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void OnWindowLoaded(object sender, EventArgs e)
        {
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
                if (IsStartViewActive())
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
                    }
                }
                else
                {
                    NfcScannerService.SetCardUID(string.Empty);
                }
            
                await Task.Delay(500);
            }
        }
        
        private bool IsStartViewActive()
        {
            return Dispatcher.UIThread.Invoke(() =>
            {

                if (_mainContent.Content.GetType().Name == nameof(StartView)) return true;
                
                return false;
            });
        }
        
        
        private void ShowStartView()
        { 
            _startView = new StartView();
            _mainContent.Content = _startView;
            _currentCardUID = null;
        }

        private void ShowPinView(string cardUID)
        {
            _pinView = new PinView();
            _pinView.Initialize(cardUID);
            
            _pinView.OnPinVerified += (uid) => ShowMainMenuView(uid);
            _pinView.OnBackToMain += ShowStartView;
            
            _mainContent.Content = _pinView;
        }

        private void ShowMainMenuView(string cardUID)
        {
            _mainMenuView = new MainMenuView();
            _mainMenuView.Initialize(cardUID);
            
            _mainMenuView.OnExit += ShowPartingView;
            _mainMenuView.OnTransactionRequested += (uid, operationType) => ShowTransactionView(uid, operationType);
            _mainMenuView.OnViewBalance += (uid) => ShowBalanceView(uid);
            
            _mainContent.Content = _mainMenuView;
            _currentCardUID = cardUID;
        }

        private void ShowTransactionView(string cardUID, string operationType)
        {
            _transactionView = new TransactionView();
            
            string title = operationType == "Deposit" ? "Enter deposit amount" : "Enter withdrawal amount";
            _transactionView.Initialize(cardUID, operationType, title);
            
            _transactionView.OnAmountConfirmed += (uid, amount) => ProcessTransaction(uid, operationType, amount);
            _transactionView.OnBackToOperations += () => ShowMainMenuView(cardUID);
            
            _mainContent.Content = _transactionView;
            _currentCardUID = cardUID;
            _currentOperationType = operationType;
        }

        private void ShowBalanceView(string cardUID)
        {
            _balanceView = new BalanceView();
            _balanceView.Initialize(cardUID);
            
            _balanceView.OnBackToMainMenu += () => ShowMainMenuView(cardUID);
            
            _mainContent.Content = _balanceView;
            _currentCardUID = cardUID;
        }

        private void ShowContinueOperationView(string cardUID, string operationType, int amount, bool success, string currency)
        {
            _continueOperationView = new ContinueOperationView();
            _continueOperationView.Initialize(operationType, amount, success,currency);
            
            _continueOperationView.OnBackToMainMenu += () => ShowMainMenuView(cardUID);
            _continueOperationView.OnShowPartingView += ShowPartingView;
            
            _mainContent.Content = _continueOperationView;
            _currentCardUID = cardUID;
        }
        
        private void ShowPartingView()
        {
            _partingView = new PartingView();
            
            DispatcherTimer.RunOnce(() => ShowStartView(), TimeSpan.FromSeconds(5));
            
            _mainContent.Content = _partingView;
            _currentCardUID = null;
        }

        private void ProcessTransaction(string cardUID, string operationType, int amount)
        {
            var repository = new CardHolderRepository();
            bool success = false;

            if (operationType == "Deposit")
            {
                success = repository.AddToBalance(cardUID, amount);
            }
            else if (operationType == "Withdraw")
            {
                success = repository.AddToBalance(cardUID, -amount);
            }
            
            string currency = repository.GetCurrency(cardUID);
            
            if (success)
            {
                ShowContinueOperationView(cardUID, operationType, amount, true,currency);
            }
            else
            {
                ShowContinueOperationView(cardUID, operationType, amount, false,currency);
            }
        }
    }
}