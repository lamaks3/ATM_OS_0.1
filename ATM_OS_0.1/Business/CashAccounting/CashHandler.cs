using System;
using System.Collections.Generic;
using System.Linq;
using ATM_OS_Cash_Storage;

namespace ATM_OS;

public static class CashHandler
{
    public enum Currency
    {
        BYN = 0,
        USD = 1,
        EUR = 2
    }
    
    private static readonly Dictionary<Currency, ATMStorage> storages = new();
    
    static CashHandler()
    {
        var denominations = new Dictionary<Currency, int[]>
        {
            { Currency.BYN, new[] { 5, 10, 20, 50, 100, 200, 500 } },
            { Currency.USD, new[] { 1, 5, 10, 20, 50, 100 } },
            { Currency.EUR, new[] { 5, 10, 20, 50, 100, 200} }
        };
        
        foreach (Currency currency in Enum.GetValues(typeof(Currency)))
        {
            string filename = $"storage/{currency}_atm.json";
            System.IO.Directory.CreateDirectory("storage"); 
            storages[currency] = new ATMStorage(filename, denominations[currency]);
        }
    }

    public static void SaveInfo(Currency currency)
    {
        storages[currency].SaveToFile();
    }

    public static void AddBanknotes(Currency currency, Dictionary<int, int> banknotes)
    {
        foreach (KeyValuePair<int, int> banknote in banknotes)
        {
            storages[currency].AddBanknotes(banknote.Key, banknote.Value);
        }
        SaveInfo(currency);
    }
    
    public static void WithdrawBanknotes(Currency currency, int denomination, int count)
    {
        storages[currency].WithdrawBanknotes(denomination, count);
    }
    
    public static Dictionary<int, int> GetBanknotes(Currency currency)
    {
        return storages[currency].GetAllBanknotes();
    }
    
    public static Dictionary<int, int> WithdrawAmountGreedy(Currency currency, int amount)
    {
        var availableBanknotes = storages[currency].GetAllBanknotes();
        var result = new Dictionary<int, int>();
        int remainingAmount = amount;

        var denominations = availableBanknotes
            .Where(kvp => kvp.Value > 0)
            .OrderByDescending(kvp => kvp.Key) 
            .ThenByDescending(kvp => kvp.Value)
            .Select(kvp => kvp.Key)
            .ToList();
        
        
        foreach (int denom in denominations)
        {
            if (remainingAmount <= 0) break;
    
            if (denom <= remainingAmount && availableBanknotes[denom] > 0)
            {
                int maxPossible = remainingAmount / denom;
                int toTake = Math.Min(maxPossible, availableBanknotes[denom]);
        
                if (toTake > 0)
                {
                    result[denom] = toTake;
                    remainingAmount -= denom * toTake;
                }
            }
        }

        if (remainingAmount == 0)
        {
            foreach (var kvp in result)
            {
                WithdrawBanknotes(currency, kvp.Key, kvp.Value);
            }
            SaveInfo(currency);
            return result;
        }
        else
        {
            return null;
        }
    }
}
