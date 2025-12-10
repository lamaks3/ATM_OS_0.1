using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ATM_OS_services;
using ATM_OS.Business.Interfaces.Repositories;
using ATM_OS.Business.Interfaces.Services;

namespace ATM_OS;

public class AtmService
{
    private static INfcScannerService _nfcScanner;
    private static ICurrencyService _currencyUpdateService;
    public static void InitializeServices()
    {
        _nfcScanner = new NfcScannerService();
        _currencyUpdateService = new CurrencyUpdateService();
        Task.Run(() => _nfcScanner.StartScanner());
        LoadRates();
    }
    
    public static string GetCardUid()
    {
        return _nfcScanner.GetCardUid();
    }

    public static void ClearCardUid()
    {
        _nfcScanner.ClearCardUid();
    }
    
    private static void LoadRates()
    {
        _ = _currencyUpdateService.LoadRatesAsync();
    }

    public static Dictionary<string, string> GetRates()
    {
        return _currencyUpdateService.GetRates();
    }

    public static string GetUpdateRatesTime()
    {
        return _currencyUpdateService.GetLastUpdateTime();
    }
}