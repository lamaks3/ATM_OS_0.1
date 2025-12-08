using System;
using System.Collections.Generic;
using System.Linq;

namespace ATMProject
{
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
            
            foreach (Currency currency in System.Enum.GetValues(typeof(Currency)))
            {
                string filename = $"storage/{currency}_atm.json";
                System.IO.Directory.CreateDirectory("storage"); 
                storages[currency] = new ATMStorage(filename, denominations[currency]);
            }
        }

        // Добавить купюры в указанную валюту
        public static void AddBanknotes(Currency currency, int denomination, int count)
        {
            storages[currency].AddBanknotes(denomination, count);
        }

        // Снять купюры из указанной валюты
        public static void WithdrawBanknotes(Currency currency, int denomination, int count)
        {
            storages[currency].WithdrawBanknotes(denomination, count);
        }

        // Снять сумму денег (с подбором купюр)
        public static Dictionary<int, int> WithdrawAmount(Currency currency, decimal amount)
        {
            return storages[currency].WithdrawAmount(amount);
        }

        // Получить все купюры для валюты
        public static Dictionary<int, int> GetBanknotes(Currency currency)
        {
            return storages[currency].GetAllBanknotes();
        }

        // Снять сумму, используя купюры которых больше всего
        public static Dictionary<int, int> WithdrawAmountGreedy(Currency currency, decimal amount)
        {
            var availableBanknotes = storages[currency].GetAllBanknotes();
            var result = new Dictionary<int, int>();
            decimal remainingAmount = amount;
            
            // Сортируем номиналы по убыванию количества купюр
            var denominations = availableBanknotes
                .Where(kvp => kvp.Value > 0)
                .OrderByDescending(kvp => kvp.Value)
                .ThenByDescending(kvp => kvp.Key)
                .Select(kvp => kvp.Key)
                .ToList();
            
            foreach (int denom in denominations)
            {
                if (remainingAmount <= 0) break;
                
                if (denom <= remainingAmount && availableBanknotes[denom] > 0)
                {
                    int maxPossible = (int)(remainingAmount / denom);
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
                return result;
            }
            
            throw new System.InvalidOperationException($"Cannot withdraw {amount}");
        }
    }
}