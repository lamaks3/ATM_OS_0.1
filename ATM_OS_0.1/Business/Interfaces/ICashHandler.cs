using System.Collections.Generic;

namespace ATM_OS_Cash_Storage
{
    public interface ICashHandler
    {
        void AddBanknotes(Currency currency, int denomination, int count);
        
        void WithdrawBanknotes(Currency currency, int denomination, int count);
        Dictionary<int, int> GetBanknotes(Currency currency);
        
        Dictionary<int, int> WithdrawAmountGreedy(Currency currency, int amount);
        
        enum Currency
        {
            BYN = 0,
            USD = 1,
            EUR = 2
        }
    }
}