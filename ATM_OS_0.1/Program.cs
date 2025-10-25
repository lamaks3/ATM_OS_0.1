using Avalonia;
using System;
using System.Threading.Tasks;

namespace ATM_OS;

class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        User user = new User();
        Task.Run(() => NfcScannerService.StartServer());
        
        BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);
    }

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}