using ATM_OS_Configuration;

namespace ATM_OS_DB;

public static class DbScheme
{
    public const string DatabaseFile = AtmConfiguration.DefaultDatabasePath;
    public const string UsersTable = "Users";
    public const string CardUid = "card_uid";
    public const string UserName = "user_name";
    public const string AccountNumber = "account_number";
    public const string PaymentSystem = "payment_system";
    public const string Currency = "currency";
    public const string ExpireDate = "expire_date";
    public const string BalanceCents = "balance_cents";
    public const string PinCode = "pin_code";
}