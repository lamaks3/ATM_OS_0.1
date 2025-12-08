namespace ATM_OS;
    
public static class AtmConfiguration
{
    public const int PinLength = 4;
    public const int MaxPinAttempts = 3;
    
    public const int MaxTransactionAmount = 10000;
    public const int TransactionInputLength = 5;
    
    public const int CardCheckIntervalMs = 500;
    public const string NfcServerUrl = "http://127.0.0.1:5055/api/nfc/scan/";
    
    public const string CurrencyApiUrl = "https://belarusbank.by/api/kursExchange?city=Гродно";
    
    public const int PartingViewDelaySeconds = 7;
    
    public const string DefaultDatabasePath = "CardHolders.db";
    
}
