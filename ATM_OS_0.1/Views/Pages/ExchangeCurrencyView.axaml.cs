using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace ATM_OS
{
    public partial class ExchangeCurrencyView : UserControl
    {
        public Action OnBackToMain;
        
        public ExchangeCurrencyView()
        {
            InitializeComponent();
            DisplayCachedRates();
        }

        private void DisplayCachedRates()
        {
            var usdInText = this.FindControl<TextBlock>("USDInText");
            var usdOutText = this.FindControl<TextBlock>("USDOutText");
            var eurInText = this.FindControl<TextBlock>("EURInText");
            var eurOutText = this.FindControl<TextBlock>("EUROutText");
            var lastUpdateText = this.FindControl<TextBlock>("LastUpdateText");
            var errorText = this.FindControl<TextBlock>("ErrorText");

            
            usdInText.Text = CurrencyUpdateService.Rates["USD_in"];
            usdOutText.Text = CurrencyUpdateService.Rates["USD_out"];
            eurInText.Text = CurrencyUpdateService.Rates["EUR_in"];
            eurOutText.Text = CurrencyUpdateService.Rates["EUR_out"];
            lastUpdateText.Text = CurrencyUpdateService.LastUpdate;
            
            errorText.IsVisible = CurrencyUpdateService.Rates["USD_in"] == "?"; 
            if (errorText.IsVisible)
            {
                errorText.Text = "Failed to load exchange rates";
            }
        }
        
        private void OnReturnHomeClick(object sender, RoutedEventArgs e)
        {
            OnBackToMain?.Invoke();
        }
    }
}