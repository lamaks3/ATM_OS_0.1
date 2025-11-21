using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System;
using System.Threading.Tasks;
using Avalonia.Threading;

namespace ATM_OS
{
    public partial class MainWindow : Avalonia.Controls.Window
    {
        private StartView _startView;
        private PinView _pinView;
        private MainMenuView _mainMenuView;
        private TransactionView _transactionView;
        private BalanceView _balanceView;
        private ContinueOperationView _continueOperationView;
        private PartingView _partingView;
        private ChangePinView _changePinView;
        
        private ContentControl _mainContent;

        public MainWindow()
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
                    string cardUid = NfcScannerService.GetCardUID();
                    
                    cardUid = "210bc299"; //for test
                
                    if (!string.IsNullOrEmpty(cardUid))
                    {
                        var repository = new CardHolderRepository();
                        if (repository.CardExists(cardUid))
                        {
                            Console.WriteLine("[Work with DB] Card found in database");
                            Console.WriteLine("[Operations with DB] Access granted");
                            await Dispatcher.UIThread.InvokeAsync(() =>
                            {
                                ShowPinView(cardUid);
                            });
                        }
                        else
                        {
                            NfcScannerService.SetCardUID(string.Empty);
                            Console.WriteLine("[Operations with DB] Card NOT found in database");
                            Console.WriteLine("[Operations with DB] Access denied");
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
                _mainContent?.Content?.GetType().Name == nameof(StartView)
            );
        }
        
        private void ShowStartView()
        { 
            _startView = new StartView();
            _mainContent.Content = _startView;
        }

        private void ShowPinView(string cardUid)
        {
            _pinView = new PinView();
            _pinView.Initialize(cardUid);
            
            _pinView.OnPinVerified += (uid) => ShowMainMenuView(uid);
            _pinView.OnBackToMain += ShowStartView;
            
            _mainContent.Content = _pinView;
        }

        private void ShowMainMenuView(string cardUid)
        {
            _mainMenuView = new MainMenuView();
            _mainMenuView.Initialize(cardUid);
            
            _mainMenuView.OnExit += ShowPartingView;
            _mainMenuView.OnTransactionRequested += (uid, operationType) => ShowTransactionView(uid, operationType);
            _mainMenuView.OnViewBalance += (uid) => ShowBalanceView(uid);
            _mainMenuView.OnChangePin += (uid) => ShowChangePinView(uid);
            
            _mainContent.Content = _mainMenuView;
        }

        private void ShowTransactionView(string cardUid, string operationType)
        {
            _transactionView = new TransactionView();
            
            string title = operationType == "Deposit" ? "Enter deposit amount" : "Enter withdrawal amount";
            _transactionView.Initialize(cardUid, operationType, title);
            
            _transactionView.OnAmountConfirmed += (uid, amount) => ProcessTransaction(uid, operationType, amount);
            _transactionView.OnBackToOperations += () => ShowMainMenuView(cardUid);
            
            _mainContent.Content = _transactionView;
        }

        private void ShowBalanceView(string cardUid)
        {
            _balanceView = new BalanceView();
            _balanceView.Initialize(cardUid);
            
            _balanceView.OnBackToMainMenu += () => ShowMainMenuView(cardUid);
            
            _mainContent.Content = _balanceView;
        }

        private void ShowContinueOperationView(string cardUid, string operationType, int amount, string currency)
        {
            _continueOperationView = new ContinueOperationView();
            _continueOperationView.Initialize(operationType, amount,currency);
            
            _continueOperationView.OnBackToMainMenu += () => ShowMainMenuView(cardUid);
            _continueOperationView.OnShowPartingView += ShowPartingView;
            
            _mainContent.Content = _continueOperationView;
        }
        
        private void ShowChangePinView(string cardUid)
        {
            _changePinView = new ChangePinView();
            _changePinView.Initialize(cardUid);
            
            _changePinView.OnBackToMain += () => ShowMainMenuView(cardUid);
            _changePinView.OnShowPartingView += () => ShowMainMenuView(cardUid);
            
            _mainContent.Content = _changePinView;
        }
        
        private void ShowPartingView()
        {
            _partingView = new PartingView();
            
            DispatcherTimer.RunOnce(() => ShowStartView(), TimeSpan.FromSeconds(7));
            
            _mainContent.Content = _partingView;
        }
        
        private void ProcessTransaction(string cardUid, string operationType, int amount)
        {
            var repository = new CardHolderRepository();
            
            if (operationType == "Deposit")
            {
                repository.AddToBalance(cardUid, amount);
            }
            else if (operationType == "Withdraw")
            {
                repository.AddToBalance(cardUid, -amount);
            }
            
            string currency = repository.GetCurrency(cardUid);
            
            ShowContinueOperationView(cardUid, operationType, amount,currency);
        }
    }
}