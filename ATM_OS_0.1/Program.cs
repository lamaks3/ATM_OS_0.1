using Avalonia;
using System;
using System.Threading.Tasks;

namespace ATM_OS;

class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        AtmService.InitializeServices();
        
        BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);
    }

    private static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}