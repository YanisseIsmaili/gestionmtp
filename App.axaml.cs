using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Yprotect.ViewModels;
using Yprotect.Views;
using SQLitePCL;
using Yprotect.Modeles;
using Yprotect.Utils;
using System;
using Microsoft.EntityFrameworkCore;

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
               desktop.MainWindow = new LoginWindow();
               Logger.Info("Fenêtre principale créée");
           }

           base.OnFrameworkInitializationCompleted();

           Logger.Database("Création de la base de données...");
           
           using (YprotectContext olocalDatabase = new YprotectContext())
           {
               Logger.Database("Application des migrations...");
               olocalDatabase.Database.Migrate();
               
               // AdminSeeder désactivé temporairement
               AdminSeeder.SeedSuperAdmin(olocalDatabase);
               
               Logger.Success("Base de données et migrations appliquées avec succès");
               Logger.Database($"Base dans: {AppDomain.CurrentDomain.BaseDirectory}");
           }
           
           Logger.Info("Application démarrée avec succès");
       }
       catch (Exception ex)
       {
           Logger.Error("Erreur critique au démarrage", ex);
           throw;
       }
   }
}