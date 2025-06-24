using Avalonia;
using System;

namespace Yprotect;

class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        // Allouer une console pour voir les erreurs
        if (!System.Diagnostics.Debugger.IsAttached)
        {
            AllocConsole();
        }

        try
        {
            Console.WriteLine("Démarrage de l'application...");
            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur: {ex.Message}");
            Console.WriteLine($"Stack: {ex.StackTrace}");
            Console.ReadKey();
        }
    }

    [System.Runtime.InteropServices.DllImport("kernel32.dll")]
    private static extern bool AllocConsole();

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .LogToTrace();
}