using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace ATMProject
{
    public class ATMStorage
    {
        private Dictionary<int, int> banknotes;
        private readonly string filename;

        public ATMStorage(string filename, int[] denominations)
        {
            this.filename = filename;
            banknotes = new Dictionary<int, int>();
            
            foreach (int denom in denominations)
            {
                banknotes[denom] = 0;
            }
            
            LoadFromFile();
        }

        private void LoadFromFile()
        {
            if (File.Exists(filename))
            {
                string json = File.ReadAllText(filename);
                var loadedData = JsonSerializer.Deserialize<Dictionary<int, int>>(json);
                
                foreach (var kvp in loadedData)
                {
                    if (banknotes.ContainsKey(kvp.Key))
                    {
                        banknotes[kvp.Key] = kvp.Value;
                    }
                }
            }
            else
            {
                SaveToFile();
            }
        }

        private void SaveToFile()
        {
            string json = JsonSerializer.Serialize(banknotes, new JsonSerializerOptions
            {
                WriteIndented = true
            });
            File.WriteAllText(filename, json);
        }

        public void AddBanknotes(int denomination, int count)
        {
            banknotes[denomination] += count;
            SaveToFile();
        }

        public void WithdrawBanknotes(int denomination, int count)
        {
            banknotes[denomination] -= count;
            SaveToFile();
        }

        public Dictionary<int, int> WithdrawAmount(decimal amount)
        {
            var available = new Dictionary<int, int>(banknotes);
            var result = new Dictionary<int, int>();
            decimal remaining = amount;

            var denominations = banknotes.Keys.OrderByDescending(x => x).ToList();

            foreach (int denom in denominations)
            {
                if (remaining <= 0) break;

                if (available[denom] > 0 && denom <= remaining)
                {
                    int countNeeded = (int)(remaining / denom);
                    int countToTake = Math.Min(countNeeded, available[denom]);

                    if (countToTake > 0)
                    {
                        result[denom] = countToTake;
                        available[denom] -= countToTake;
                        remaining -= denom * countToTake;
                    }
                }
            }

            if (remaining > 0)
            {
                throw new InvalidOperationException($"Невозможно выдать сумму {amount}");
            }

            banknotes = available;
            SaveToFile();

            return result;
        }

        public decimal GetTotalAmount()
        {
            return banknotes.Sum(kvp => kvp.Key * kvp.Value);
        }

        public Dictionary<int, int> GetAllBanknotes()
        {
            return new Dictionary<int, int>(banknotes);
        }
    }
}