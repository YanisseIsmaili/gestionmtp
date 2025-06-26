using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Yprotect.ViewModels;
using Yprotect.Views;
using SQLitePCL;
using Yprotect.Modeles;
using Yprotect.Utils;
using System;

namespace Yprotect;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        Logger.Info("Démarrage de l'application Yprotect");
        
        try
        {
            Batteries.Init();
            Logger.Success("SQLite initialisé");
            
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow
                {
                    DataContext = new MainWindowViewModel()
                };
                Logger.Info("Fenêtre principale créée");
            }

            base.OnFrameworkInitializationCompleted();

            // Initialize the SQLite database connection
            Logger.Database("Création de la base de données...");
            
            YprotectContext olocalDatabase = new YprotectContext();
            bool created = olocalDatabase.Database.EnsureCreated();
            
            if (created)
            {
                Logger.Success("Base de données créée avec succès");
            }
            else
            {
                Logger.Info("Base de données existante trouvée");
            }
            
            Logger.Database($"Base dans: {AppDomain.CurrentDomain.BaseDirectory}");
            
            olocalDatabase.Dispose();
            Logger.Info("Application démarrée avec succès");
        }
        catch (Exception ex)
        {
            Logger.Error("Erreur critique au démarrage", ex);
            throw;
        }
    }
}