using Microsoft.Data.Sqlite;
using System;
namespace ATM_OS;

public class CardHolderRepository
{
    private readonly string _connectionString;

    public CardHolderRepository(string dbPath = "CardHolder.db")
    {
        _connectionString = $"Data Source={dbPath}";
    }
    
    public CardHolder GetCardHolderByUid(string cardUid)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        
        var command = connection.CreateCommand();
        command.CommandText = """
            SELECT [Card UID], [Holder name], [Number of account ], [Payment system], 
                   [Currency], [Expire date], [PIN code], [Balance] 
            FROM Users 
            WHERE [Card UID] = @cardUid
            """;
        command.Parameters.AddWithValue("@cardUid", cardUid);

        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            return new CardHolder
            {
                CardUid = reader.GetString(0),
                HolderName = reader.GetString(1),
                NumberOfAccount = reader.GetString(2),
                PaymentSystem = reader.GetString(3),
                Currency = reader.GetString(4),
                ExpireDate = reader.GetString(5),
                PinCode = reader.GetInt32(6),
                Balance = reader.GetInt32(7)
            };
        }

        return null;
    }

    public bool AddToBalance(string cardUid, int amount)
    {
        
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        
        if(GetBalance(cardUid) + amount < 0) return false;

        amount *= 100;
        var command = connection.CreateCommand();
        command.CommandText = "UPDATE Users SET [Balance] = [Balance] + @amount WHERE [Card UID] = @cardUid";
        command.Parameters.AddWithValue("@amount", amount);
        command.Parameters.AddWithValue("@cardUid", cardUid);

        command.ExecuteNonQuery();
        return true;
    }
    
    public double GetBalance(string cardUid)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        
        var command = connection.CreateCommand();
        command.CommandText = "SELECT [Balance] FROM Users WHERE [Card UID] = @cardUid";
        command.Parameters.AddWithValue("@cardUid", cardUid);

        return Convert.ToInt32(command.ExecuteScalar())/100.0;
    }
    
    public string GetCurrency(string cardUid)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        
        var command = connection.CreateCommand();
        command.CommandText = "SELECT [Currency] FROM Users WHERE [Card UID] = @cardUid";
        command.Parameters.AddWithValue("@cardUid", cardUid);
        var result = command.ExecuteScalar();
        
        return result.ToString();
    }
    public bool checkPIN(string cardUid, string password)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        
        var command = connection.CreateCommand();
        command.CommandText = "SELECT [PIN Code] FROM Users WHERE [Card UID] = @cardUid";
        command.Parameters.AddWithValue("@cardUid", cardUid);
            
        return command.ExecuteScalar().ToString()==password;
    }
    
    public bool CardExists(string cardUid)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
    
        var command = connection.CreateCommand();
        command.CommandText = "SELECT 1 FROM Users WHERE [Card UID] = @cardUid";
        command.Parameters.AddWithValue("@cardUid", cardUid);

        return command.ExecuteScalar() != null;
    }
}