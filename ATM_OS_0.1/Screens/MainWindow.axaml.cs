using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System;
using System.Threading.Tasks;
using Avalonia.Threading;

namespace ATM_OS
{
    public partial class MainWindow : Window
    {
        private MainMenuView _mainMenuView;
        private PinView _pinView;
        private OperationsView _operationsView;

        public MainWindow()
        {
            InitializeComponent();
            
            // Ждем полной инициализации окна
            this.Opened += OnWindowOpened;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void OnWindowOpened(object sender, EventArgs e)
        {
            // Теперь окно полностью инициализировано
            InitializeNfcListener();
            ShowMainMenu();
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

        private void ShowMainMenu()
        {
            // Проверяем что MainContent инициализирован
            if (MainContent == null)
            {
                MainContent = this.FindControl<ContentControl>("MainContent");
            }

            _mainMenuView = new MainMenuView();
            MainContent.Content = _mainMenuView;
        }

        private void ShowPinView(string cardUID)
        {
            _pinView = new PinView();
            _pinView.Initialize(cardUID);
            
            _pinView.OnPinVerified += (uid) => ShowOperationsView(uid);
            _pinView.OnBackToMain += ShowMainMenu;
            
            MainContent.Content = _pinView;
        }

        private void ShowOperationsView(string cardUID)
        {
            _operationsView = new OperationsView();
            _operationsView.Initialize(cardUID);
            
            _operationsView.OnExit += ShowMainMenu;
            
            MainContent.Content = _operationsView;
        }
    }
}