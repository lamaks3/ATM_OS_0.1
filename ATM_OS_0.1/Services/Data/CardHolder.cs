using ATM_OS;

public class CardHolder
{
    public string CardUid { get; set; } = string.Empty;
    public string HolderName { get; set; } = string.Empty;
    public string NumberOfAccount { get; set; } = string.Empty;
    public string PaymentSystem { get; set; } = string.Empty;
    public string Currency { get; set; } = string.Empty;
    public string ExpireDate { get; set; } = string.Empty;
    public double Balance { get; set; }

    public CardHolder()
    {
        
    }
    
    public CardHolder(string cardUid,string holderName, string number, string paymentSystem, string currency, string expireDate,
        double balance)
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