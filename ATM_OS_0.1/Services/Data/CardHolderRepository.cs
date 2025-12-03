using Microsoft.Data.Sqlite;
using System;
namespace ATM_OS;

public class CardHolderRepository
{
    private readonly string _connectionString;

    public CardHolderRepository(string dbPath = "CardHolders.db")
    {
        _connectionString = $"Data Source={dbPath}";
    }
    
    private T GetUserData<T>(string columnName, string cardUid)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = $"SELECT [{columnName}] FROM Users WHERE [card_uid] = @cardUid";
        command.Parameters.AddWithValue("@cardUid", cardUid);

        var result = command.ExecuteScalar();
        return (T)Convert.ChangeType(result, typeof(T));
    }

    private void UpdateUserData<T>(string columnName, string cardUid, T newValue)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
    
        var command = connection.CreateCommand();
        command.CommandText = $"UPDATE Users SET [{columnName}] = @newValue WHERE [card_uid] = @cardUid";
        command.Parameters.AddWithValue("@newValue", newValue);
        command.Parameters.AddWithValue("@cardUid", cardUid);
        command.ExecuteNonQuery();
    }
    
    public CardHolder GetUser(string cardUid)
    {
        if (!CardExists(cardUid)) return null;
        
        string cardUidData = GetUserData<string>("card_uid", cardUid);
        string holderName = GetUserData<string>("user_name", cardUid);
        string numberOfAccount = GetUserData<string>("account_number", cardUid);
        string paymentSystem = GetUserData<string>("payment_system", cardUid);
        string currency = GetUserData<string>("currency", cardUid);
        string expireDate = GetUserData<string>("expire_date", cardUid);
        string balanceStr = GetUserData<string>("balance_cents", cardUid);
        
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
        string userName = GetUserData<string>("user_name", cardUid).Split(" ")[0];
        return userName;
    }
    public void UpdateBalance(string cardUid, double amount)
    {
        int newBalance = (int)((GetBalance(cardUid) + amount)*100);
        if(newBalance < 0) return;
        
        UpdateUserData("balance_cents", cardUid, newBalance.ToString());
    }

    public void ChangePin(string cardUid, string newPinCode)
    {
        UpdateUserData("pin_code", cardUid, newPinCode);
    }
    public double GetBalance(string cardUid)
    {
        int balance = GetUserData<int>("balance_cents", cardUid);
        double balanceCents = balance/100.0;
        return balanceCents;
    }
    
    public string GetCurrency(string cardUid)
    {
        string currency = GetUserData<string>("currency", cardUid);
        return currency;
    }
    public bool VerifyPin(string cardUid, string enteredPin)
    {
        string pinCode = GetUserData<string>("pin_code", cardUid);
        return pinCode == enteredPin;
    }
    
    public bool CardExists(string cardUid)
    {
        string userData = GetUserData<string>("card_uid", cardUid);
        return userData != null;
    }
}