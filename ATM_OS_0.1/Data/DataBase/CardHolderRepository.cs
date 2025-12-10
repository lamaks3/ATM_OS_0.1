using Microsoft.Data.Sqlite;
using System;
using ATM_OS.Business.Interfaces.Repositories;

namespace ATM_OS_DB;

public class CardHolderRepository : ICardHolderRepository
{
    private readonly string _connectionString;

    public CardHolderRepository(string dbPath = DbScheme.DatabaseFile)
    {
        _connectionString = $"Data Source={dbPath}";
    }
    
    private T GetUserData<T>(string columnName, string cardUid)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = $"SELECT [{columnName}] FROM {DbScheme.UsersTable} WHERE [{DbScheme.CardUid}] = @cardUid";
        command.Parameters.AddWithValue("@cardUid", cardUid);

        var result = command.ExecuteScalar();
        if (result == null || result == DBNull.Value)
            return default;
            
        return (T)Convert.ChangeType(result, typeof(T));
    }

    private void UpdateUserData<T>(string columnName, string cardUid, T newValue)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
    
        var command = connection.CreateCommand();
        command.CommandText = $"UPDATE {DbScheme.UsersTable} SET [{columnName}] = @newValue WHERE [{DbScheme.CardUid}] = @cardUid";
        command.Parameters.AddWithValue("@newValue", newValue);
        command.Parameters.AddWithValue("@cardUid", cardUid);
        command.ExecuteNonQuery();
    }
    
    public CardHolder GetCardHolder(string cardUid)
    {
        if (!CardExists(cardUid)) return null;
        
        string cardUidData = GetUserData<string>(DbScheme.CardUid, cardUid);
        string holderName = GetUserData<string>(DbScheme.UserName, cardUid);
        string numberOfAccount = GetUserData<string>(DbScheme.AccountNumber, cardUid);
        string paymentSystem = GetUserData<string>(DbScheme.PaymentSystem, cardUid);
        string currency = GetUserData<string>(DbScheme.Currency, cardUid);
        string expireDate = GetUserData<string>(DbScheme.ExpireDate, cardUid);
        string balanceStr = GetUserData<string>(DbScheme.BalanceCents, cardUid);
        
        double balanceCents = double.Parse(balanceStr);
        
        var cardHolder = new CardHolder(
            cardUid: cardUidData,
            holderName: holderName,
            number: numberOfAccount,
            paymentSystem: paymentSystem,
            currency: currency,
            expireDate: expireDate,
            balance: balanceCents
        );
    
        return cardHolder;
    }

    public void UpdateBalance(string cardUid, double newBalance)
    {
        int balanceCents = (int)(newBalance * 100);
        UpdateUserData(DbScheme.BalanceCents, cardUid, balanceCents);
    }

    public void UpdatePin(string cardUid, string newPin)
    {
        UpdateUserData(DbScheme.PinCode, cardUid, newPin);
    }

    public double GetBalance(string cardUid)
    {
        int balanceCents = GetUserData<int>(DbScheme.BalanceCents, cardUid);
        double balance = balanceCents / 100.0;
        return balance;
    }
    
    public string GetCurrency(string cardUid)
    {
        string currency = GetUserData<string>(DbScheme.Currency, cardUid);
        return currency;
    }
    
    public bool VerifyPin(string cardUid, string enteredPin)
    {
        string pinCode = GetUserData<string>(DbScheme.PinCode, cardUid);
        return pinCode == enteredPin;
    }
    
    public bool CardExists(string cardUid)
    {
        string userData = GetUserData<string>(DbScheme.CardUid, cardUid);
        return userData != null;
    }
    
}