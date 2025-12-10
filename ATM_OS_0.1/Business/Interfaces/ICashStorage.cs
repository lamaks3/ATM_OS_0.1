using System.Collections.Generic;

namespace ATM_OS.Business.Interfaces.Storage;

public interface IATMStorage
{
    void AddBanknotes(int denomination, int count);
    void WithdrawBanknotes(int denomination, int count);
    Dictionary<int, int> GetAllBanknotes();
    
}