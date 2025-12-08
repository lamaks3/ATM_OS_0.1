using Avalonia;
using System;
using System.Threading.Tasks;
using ATMProject;

namespace ATM_OS;

class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        Task.Run(() => NfcScannerService.StartServer());
        AtmOperations.LoadRates();
        
        BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);
    }

    private static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}