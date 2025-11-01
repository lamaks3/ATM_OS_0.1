using System;

namespace ATM_OS
{
    public class User
    {
        public string FirstName { get; set; } 
        public string LastName { get; set; }
        public string CardNumber {get; set;}
        public string Currency { get; set; } 
        public DateTime CardExpiryDate { get; set; } 
        public string PinCode { get; set; } 
        public decimal MoneyAmount { get; set; }

        public User()
        {
            CardNumber = NfcScannerService.GetCardUID();
        }
    }
}