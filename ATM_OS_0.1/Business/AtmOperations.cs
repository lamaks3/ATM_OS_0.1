using System;
using ATM_OS_DB;

namespace ATM_OS;

public class AtmOperations
{
    private readonly CardHolderRepository _repository;

    public AtmOperations(string dbPath = DbScheme.DatabaseFile)
    {
        _repository = new CardHolderRepository(dbPath);
    }

    public bool TryProceedTransaction(string cardUid, double amount, HomeView.OperationType type)
    {
        if (type == HomeView.OperationType.Deposit)
        {
            return TryDeposit(cardUid, amount);
        }
        else
        {
            return TryWithdraw(cardUid, amount);
        }
    }
    
    private bool TryWithdraw(string cardUid, double amount)
    {
        double currentBalance = _repository.GetBalance(cardUid);
        
        if (currentBalance >= amount)
        {
            double newBalance = currentBalance - amount;
            _repository.UpdateBalance(cardUid, newBalance);
            return true;
        }
        
        return false;
    }

    private bool TryDeposit(string cardUid, double amount)
    {
        double currentBalance = _repository.GetBalance(cardUid);
        double newBalance = currentBalance + amount;
        _repository.UpdateBalance(cardUid, newBalance);
        return true;
    }

    public bool VerifyPin(string cardUid, string pin)
    {
        if (!_repository.CardExists(cardUid))
            return false;
            
        return _repository.VerifyPin(cardUid, pin);
    }

    public void ChangePin(string cardUid, string newPin)
    {
        _repository.UpdatePin(cardUid, newPin);
    }

    public double GetBalance(string cardUid)
    {
        return _repository.GetBalance(cardUid);
    }

    public string GetCurrency(string cardUid)
    {
        return _repository.GetCurrency(cardUid);
    }

    public CardHolder GetCardHolderInfo(string cardUid)
    {
        return _repository.GetCardHolder(cardUid);
    }
    
    public string GetUserName(string cardUid)
    {
        var user = _repository.GetCardHolder(cardUid);
        return user.HolderName.Split(" ")[0]; 
    }
    
}