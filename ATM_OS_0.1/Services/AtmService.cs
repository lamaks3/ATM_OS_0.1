using System;

namespace ATM_OS;

public class AtmService
{
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