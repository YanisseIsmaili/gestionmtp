using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Yprotect.Modeles;
using Yprotect.Utils;
using Yprotect.Views;
using Yprotect.ViewModels;
using Yprotect.Services;
using System;

namespace Yprotect.Views
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object? sender, RoutedEventArgs e)
        {
            var email = EmailTextBox.Text?.Trim();
            var password = PasswordTextBox.Text;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ShowError("Veuillez remplir tous les champs");
                return;
            }

            try
            {
                using var context = new YprotectContext();
                var hashedPassword = HashPassword(password);

                var user = context.Utilisateurs
                    .FirstOrDefault(u => u.Email == email && u.MotDePasse == hashedPassword);

                if (user != null)
                {
                    // Assigner le rôle si manquant
                    if (string.IsNullOrEmpty(user.Role))
                    {
                        user.Role = user.Email == "admin@yprotect.com" ? "Admin" : "User";
                    }
                    
                    UserSession.SetCurrentUser(user);
                    
                    Logger.Success($"Connexion réussie pour {user.Email} (Rôle: {user.Role})");

                    var mainWindow = new MainWindow
                    {
                        DataContext = new MainWindowViewModel()
                    };

                    if (Application.Current?.ApplicationLifetime is Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop)
                    {
                        desktop.MainWindow = mainWindow;
                    }

                    mainWindow.Show();
                    this.Close();
                }
                else
                {
                    ShowError("Email ou mot de passe incorrect");
                    Logger.Error("Tentative de connexion échouée");
                }
            }
            catch (Exception ex)
            {
                ShowError("Erreur de connexion");
                Logger.Error("Erreur lors de la connexion", ex);
            }
        }

        private void ShowError(string message)
        {
            ErrorTextBlock.Text = message;
            ErrorTextBlock.IsVisible = true;
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }

        private void CreateAccountButton_Click(object? sender, RoutedEventArgs e)
        {
            var registerWindow = new RegisterWindow();

            if (Application.Current?.ApplicationLifetime is Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = registerWindow;
            }

            registerWindow.Show();
            this.Close();
        }
    }
}