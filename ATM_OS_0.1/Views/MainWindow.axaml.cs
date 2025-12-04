using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Threading;

namespace ATM_OS
{
    public partial class MainWindow : Avalonia.Controls.Window
    {
        private StartView _startView;
        private PinView _pinView;
        private HomeView _mainMenuView;
        private TransactionView _transactionView;
        private BalanceView _balanceView;
        private ContinueOperationView _continueOperationView;
        private PartingView _partingView;
        private ChangePinView _changePinView;
        private ExchangeCurrencyView _exchangeCurrencyView;
        
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
            ShowStartView();
        }
        
        private void ShowStartView()
        { 
            _startView = new StartView();

            _startView.CardDetectedAndVerified += ShowPinView;
            _startView.ExitAppRequested += OnExitAppRequested;
            _mainContent.Content = _startView;
        }

        private void OnExitAppRequested()
        {
            Environment.Exit(0);
        }

        private void ShowPinView(string cardUid)
        {
            _pinView = new PinView();
            _pinView.Initialize(cardUid);
            
            _pinView.OnPinVerified += ShowMainMenuView;
            _pinView.OnBackToMain += ShowStartView;
            
            _mainContent.Content = _pinView;
        }

        private void ShowMainMenuView(string cardUid)
        {
            _mainMenuView = new HomeView();
            _mainMenuView.Initialize(cardUid);
            
            _mainMenuView.OnExit += ShowPartingView;
            _mainMenuView.OnTransactionRequested += ShowTransactionView; 
            _mainMenuView.OnViewBalance += ShowBalanceView;
            _mainMenuView.OnChangePin += ShowChangePinView;
            _mainMenuView.OnExchangeCurrency += ShowExchangeCurrencyView;
            
            _mainContent.Content = _mainMenuView;
        }
        
        private void ShowTransactionView(string cardUid, HomeView.OperationType operationType)
        {
            _transactionView = new TransactionView();
            
            _transactionView.Initialize(cardUid, operationType);
            
            _transactionView.OnAmountConfirmed += ProcessTransaction;
            _transactionView.OnBackToOperations += ShowMainMenuView;
            
            _mainContent.Content = _transactionView;
        }

        private void ShowBalanceView(string cardUid)
        {
            _balanceView = new BalanceView();
            _balanceView.Initialize(cardUid);
            
            _balanceView.OnBackToMainMenu += ShowMainMenuView;
            
            _mainContent.Content = _balanceView;
        }

        private void ShowContinueOperationView(string cardUid, HomeView.OperationType operationType, int amount, string currency)
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
            
            _changePinView.OnBackToMain += ShowMainMenuView;
            _changePinView.OnShowPartingView += () => ShowContinueOperationView(cardUid, HomeView.OperationType.PinChange, 0, "");;
            
            _mainContent.Content = _changePinView;
        }
        
        private void ShowPartingView()
        {
            _partingView = new PartingView();
            
            DispatcherTimer.RunOnce(() => ShowStartView(), TimeSpan.FromSeconds(7));
            
            _mainContent.Content = _partingView;
        }

        private void ShowExchangeCurrencyView(string cardUid)
        {
            _exchangeCurrencyView = new ExchangeCurrencyView();
            _exchangeCurrencyView.OnBackToMain += () => ShowMainMenuView(cardUid);
            _mainContent.Content = _exchangeCurrencyView;
            
        }
        
        private void ProcessTransaction(string cardUid, HomeView.OperationType operationType, int amount)
        {
            var repository = new CardHolderRepository();
            
            if (operationType == HomeView.OperationType.Deposit)
            {
                repository.UpdateBalance(cardUid, amount);
            }
            else if (operationType == HomeView.OperationType.Withdraw)
            {
                repository.UpdateBalance(cardUid, -amount);
            }
            
            string currency = repository.GetCurrency(cardUid);
            
            ShowContinueOperationView(cardUid, operationType, amount,currency);
        }
    }
}