using System;
using System.Threading.Tasks;
using Avalonia.Threading;

namespace ATM_OS.Services
{
    public class CardAuthentication
    {
        public event Action<string> CardAuthenticated;
        private bool _isScanningActive = false;

        public void StartScanning()
        {
            if (_isScanningActive) return;
            
            _isScanningActive = true;
            Task.Run(async () => await CheckForCards());
            Console.WriteLine("[Scanner status] Scanning started");
        }

        public void StopScanning()
        {
            _isScanningActive = false;
            Console.WriteLine("[Scanner status] Scanning stopped");
        }

        private async Task CheckForCards()
        {
            while (_isScanningActive)
            {
                string cardUid = NfcScannerService.GetCardUid();
                NfcScannerService.ClearCardUid();
                
                if (!string.IsNullOrEmpty(cardUid))
                {
                    var repository = new CardHolderRepository();
                    if (repository.CardExists(cardUid))
                    {
                        Console.WriteLine("[Work with DB] Card found in database");
                        
                        StopScanning(); 
                        
                        await Dispatcher.UIThread.InvokeAsync(() =>
                        {
                            CardAuthenticated?.Invoke(cardUid);
                        }); 
                    }
                    else
                    {
                        Console.WriteLine("[Operations with DB] Card NOT found in database");
                    }
                }
                
                if (_isScanningActive)
                {
                    await Task.Delay(500);
                }
            }
        }
    }
}