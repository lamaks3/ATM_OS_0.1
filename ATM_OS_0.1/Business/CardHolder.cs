using ATM_OS;

public class CardHolder
{
    public string CardUid { get; }
    public string HolderName { get; } 
    public string NumberOfAccount { get; } 
    public string PaymentSystem { get;} 
    public string Currency { get; } 
    public string ExpireDate { get; } 
    private double _Balance { get; set; }

    public double Balance
    {
        get { return _Balance; }
        set { if (value > 0) _Balance = value; }
    }
    
    public CardHolder(string cardUid,string holderName, string number, string paymentSystem, string currency, 
        string expireDate, double balance)
    {
        this.CardUid = cardUid;
        this.HolderName = holderName;
        this.NumberOfAccount = number;
        this.PaymentSystem = paymentSystem;
        this.Currency = currency;
        this.ExpireDate = expireDate;
        this.Balance = balance;
    }
}