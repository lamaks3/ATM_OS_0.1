using Microsoft.Data.Sqlite;
using System;

namespace ATM_OS;

public class CardHolderRepository
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
    
    public CardHolder GetUser(string cardUid)
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

    public string GetUserName(string cardUid)
    {
        string userName = GetUserData<string>(DbScheme.UserName, cardUid).Split(" ")[0];
        return userName;
    }
    public void UpdateBalance(string cardUid, double amount)
    {
        int newBalance = (int)((GetBalance(cardUid) + amount)*100);
        if (newBalance < 0) return;
        
        UpdateUserData(DbScheme.BalanceCents, cardUid, newBalance.ToString());
    }

    public void ChangePin(string cardUid, string newPinCode)
    {
        UpdateUserData(DbScheme.PinCode, cardUid, newPinCode);
    }
    public double GetBalance(string cardUid)
    {
        int balance = GetUserData<int>(DbScheme.BalanceCents, cardUid);
        double balanceCents = balance / 100.0;
        return balanceCents;
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