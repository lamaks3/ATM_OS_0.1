using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using ATM_OS_services;

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

            
            usdInText.Text = AtmService.GetRates()["USD_in"];
            usdOutText.Text = AtmService.GetRates()["USD_out"];
            eurInText.Text = AtmService.GetRates()["EUR_in"];
            eurOutText.Text = AtmService.GetRates()["EUR_out"];
            lastUpdateText.Text = AtmService.GetUpdateRatesTime();
            
            errorText.IsVisible = usdInText.Text == "?"; 
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