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
    private static readonly HttpClient _httpClient = new HttpClient();
    private static string _cachedUsdIn = "?";
    private static string _cachedUsdOut = "?";
    private static string _cachedEurIn = "?";
    private static string _cachedEurOut = "?";
    private static string _lastUpdate = "Not updated";

    public Action OnBackToMain;

    public static async Task PreloadRatesAsync()
    {
        try
        {
            var url = "https://belarusbank.by/api/kursExchange?city=Гродно";
            var response = await _httpClient.GetStringAsync(url);

            using var jsonDocument = JsonDocument.Parse(response);
            var root = jsonDocument.RootElement;
            var firstBranch = root[0];

            _cachedUsdIn = firstBranch.GetProperty("USD_in").GetString() ?? "?";
            _cachedUsdOut = firstBranch.GetProperty("USD_out").GetString() ?? "?";
            _cachedEurIn = firstBranch.GetProperty("EUR_in").GetString() ?? "?";
            _cachedEurOut = firstBranch.GetProperty("EUR_out").GetString() ?? "?";
            _lastUpdate = $"Updated: {DateTime.Now:HH:mm:ss}";
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to preload rates: {ex.Message}");
        }
    }

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

        usdInText.Text = _cachedUsdIn;
        usdOutText.Text = _cachedUsdOut;
        eurInText.Text = _cachedEurIn;
        eurOutText.Text = _cachedEurOut;
        lastUpdateText.Text = _lastUpdate;
        
        errorText.IsVisible = _cachedUsdIn == "?"; 
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