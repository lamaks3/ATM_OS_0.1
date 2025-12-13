using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ATM_OS.Business.Interfaces.Services;

public interface ICurrencyService
{
    Dictionary<string, string> GetRates();
    string GetLastUpdateTime();
    Task LoadRatesAsync();
    
}