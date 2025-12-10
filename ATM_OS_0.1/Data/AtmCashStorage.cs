using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace ATM_OS_Cash_Storage;

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
                banknotes[kvp.Key] = kvp.Value;
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
    
    public Dictionary<int, int> GetAllBanknotes()
    {
        return new Dictionary<int, int>(banknotes);
    }
}
