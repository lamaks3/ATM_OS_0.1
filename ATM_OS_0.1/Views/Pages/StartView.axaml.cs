using System;
using ATM_OS.Services;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace ATM_OS
{
    public partial class StartView : UserControl
    {
        public event Action ExitAppRequested;
        public event Action<string> CardDetectedAndVerified; 

        private readonly CardAuthentication _scanner;

        public StartView()
        {
            InitializeComponent();
            _scanner = new CardAuthentication();
            
            _scanner.CardAuthenticated += OnCardAuthenticated;
            
            _scanner.StartScanning(); 
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void OnCardAuthenticated(string cardUid)
        {
            CardDetectedAndVerified?.Invoke(cardUid);
        }
        
        public void ExitAppButton_OnClick(object sender, RoutedEventArgs e)
        {
            _scanner.StopScanning();
            ExitAppRequested?.Invoke();
        }
    }
}