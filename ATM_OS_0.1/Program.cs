using Avalonia;
using System;
using System.Threading.Tasks;

namespace ATM_OS;

class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        
        Task.Run(() => NfcScannerService.StartServer());

        _ = ExchangeCurrencyView.PreloadRatesAsync();
            
        BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);
    }

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}