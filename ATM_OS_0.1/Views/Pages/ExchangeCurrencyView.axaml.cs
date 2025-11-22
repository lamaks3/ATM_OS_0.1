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
        private readonly HttpClient _httpClient;
        public event Action OnBackToMain;

        public ExchangeCurrencyView()
        {
            InitializeComponent();
            _httpClient = new HttpClient();
            
            // Инициализируем ErrorText
            InitializeErrorText();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void InitializeErrorText()
        {
            var errorText = this.FindControl<TextBlock>("ErrorText");
            errorText.Text = "";
            errorText.IsVisible = false;
            UpdateRates();
        }

        private async void OnReturnHomeClick(object sender, RoutedEventArgs e)
        {
            OnBackToMain?.Invoke();
        }

        private async Task UpdateRates()
        {
            try
            {
                // Получаем элементы при каждом использовании
                var errorText = this.FindControl<TextBlock>("ErrorText");
                var usdInText = this.FindControl<TextBlock>("USDInText");
                var usdOutText = this.FindControl<TextBlock>("USDOutText");
                var eurInText = this.FindControl<TextBlock>("EURInText");
                var eurOutText = this.FindControl<TextBlock>("EUROutText");
                var lastUpdateText = this.FindControl<TextBlock>("LastUpdateText");

                errorText.IsVisible = false;
                errorText.Text = "";

                var url = "https://belarusbank.by/api/kursExchange?city=Гродно";
                var response = await _httpClient.GetStringAsync(url);

                using var jsonDocument = JsonDocument.Parse(response);
                var root = jsonDocument.RootElement;

                if (root.ValueKind == JsonValueKind.Array && root.GetArrayLength() > 0)
                {
                    var firstBranch = root[0];

                    usdInText.Text = firstBranch.GetProperty("USD_in").GetString() ?? "?";
                    usdOutText.Text = firstBranch.GetProperty("USD_out").GetString() ?? "?";

                    eurInText.Text = firstBranch.GetProperty("EUR_in").GetString() ?? "?";
                    eurOutText.Text = firstBranch.GetProperty("EUR_out").GetString() ?? "?";

                    lastUpdateText.Text = $"Updated: {DateTime.Now:HH:mm:ss}";
                }
                else
                {
                    throw new Exception("No data available");
                }
            }
            catch (Exception ex)
            {
                var errorText = this.FindControl<TextBlock>("ErrorText");
                errorText.Text = $"Error: {ex.Message}";
                errorText.IsVisible = true;
                SetErrorState();
            }
        }

        private void SetErrorState()
        {
            var usdInText = this.FindControl<TextBlock>("USDInText");
            var usdOutText = this.FindControl<TextBlock>("USDOutText");
            var eurInText = this.FindControl<TextBlock>("EURInText");
            var eurOutText = this.FindControl<TextBlock>("EUROutText");

            usdInText.Text = "?";
            usdOutText.Text = "?";
            eurInText.Text = "?";
            eurOutText.Text = "?";
        }
    }
}