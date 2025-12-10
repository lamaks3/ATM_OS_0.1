using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using ATM_OS_Configuration;

namespace ATM_OS_services;

public class CurrencyUpdateService
{
    private static readonly HttpClient _httpClient = new HttpClient();
    private static Dictionary<string, string> _rates = new Dictionary<string, string>
    {
        { "USD_in", "?" },
        { "USD_out", "?" },
        { "EUR_in", "?" },
        { "EUR_out", "?" }
    };
    private static string _lastUpdate = "Not updated";
    
    public static Dictionary<string, string> Rates => _rates;

    public static string LastUpdate  => _lastUpdate;
    
    public static async Task PreloadRatesAsync()
    {
        try
        {
            var url = AtmConfiguration.CurrencyApiUrl;
            var response = await _httpClient.GetStringAsync(url);

            using var jsonDocument = JsonDocument.Parse(response);
            var root = jsonDocument.RootElement;
            var firstBranch = root[0];
            
            _rates["USD_in"] = firstBranch.GetProperty("USD_in").GetString() ?? "Loading";
            _rates["USD_out"] = firstBranch.GetProperty("USD_out").GetString() ?? "Loading";
            _rates["EUR_in"] = firstBranch.GetProperty("EUR_in").GetString() ?? "Loading";
            _rates["EUR_out"] = firstBranch.GetProperty("EUR_out").GetString() ?? "Loading";
            
            _lastUpdate = $"Updated: {DateTime.Now:HH:mm:ss}";
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to preload rates: {ex.Message}");
        }
    }
}
