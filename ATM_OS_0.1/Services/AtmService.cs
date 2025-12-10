using System;
using System.Threading.Tasks;
using ATM_OS_services;

namespace ATM_OS;

public class AtmService
{
    public static void InitializeServices()
    {
        Task.Run(() => NfcScannerService.StartServer());
        LoadRates();
    }
    
    public static string GetCardUid()
    {
        return NfcScannerService.GetCardUid();
    }

    public static void ClearCardUid()
    {
        NfcScannerService.ClearCardUid();
    }
    
    public static void LoadRates()
    {
        _ =CurrencyUpdateService.PreloadRatesAsync();
    }
}