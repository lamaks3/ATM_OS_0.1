namespace ATM_OS.Business.Interfaces.Repositories;

public interface ICardHolderRepository
{
    CardHolder GetCardHolder(string cardUid);
    double GetBalance(string cardUid);
    void UpdateBalance(string cardUid, double amount);
    bool VerifyPin(string cardUid, string pin);
    bool CardExists(string cardUid);
    void UpdatePin(string cardUid, string newPin);
    string GetCurrency(string cardUid);
    
}