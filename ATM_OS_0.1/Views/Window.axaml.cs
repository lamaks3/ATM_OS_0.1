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
        private PartingView _partingView; // Добавляем PartingView

        // Добавляем поле для ContentControl
        private ContentControl _mainContent;
        private string _currentCardUID;
        private string _currentOperationType;

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
            _currentCardUID = null;
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
            
            _mainMenuView.OnExit += ShowPartingView;
            _mainMenuView.OnTransactionRequested += (uid, operationType) => ShowTransactionView(uid, operationType);
            _mainMenuView.OnViewBalance += (uid) => ShowBalanceView(uid);
            
            _mainContent.Content = _mainMenuView;
            _currentCardUID = cardUID;
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
            _currentCardUID = cardUID;
            _currentOperationType = operationType;
        }

        private void ShowBalanceView(string cardUID)
        {
            if (_mainContent == null) return;

            _balanceView = new BalanceView();
            _balanceView.Initialize(cardUID);
            
            _balanceView.OnBackToMainMenu += () => ShowMainMenuView(cardUID);
            
            _mainContent.Content = _balanceView;
            _currentCardUID = cardUID;
        }

        private void ShowContinueOperationView(string cardUID, string operationType, int amount, bool success)
        {
            if (_mainContent == null) return;

            // Создаем новый экземпляр каждый раз для актуальных данных
            _continueOperationView = new ContinueOperationView();
            
            // Инициализируем с данными о транзакции
            _continueOperationView.Initialize(operationType, amount, success);
            
            // Подписываемся на события
            _continueOperationView.OnBackToStartMenuView += ShowStartView;
            _continueOperationView.OnShowPartingView += ShowPartingView; // Новый обработчик
            
            _mainContent.Content = _continueOperationView;
            _currentCardUID = cardUID;
        }

        // Новый метод для показа PartingView
        private void ShowPartingView()
        {
            if (_mainContent == null) return;

            _partingView = new PartingView();
            
            // Через несколько секунд автоматически возвращаемся к StartView
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

            // Вместо ShowMessage показываем ContinueOperationView
            if (success)
            {
                ShowContinueOperationView(cardUID, operationType, amount, true);
            }
            else
            {
                ShowContinueOperationView(cardUID, operationType, amount, false);
            }
        }
    }
}